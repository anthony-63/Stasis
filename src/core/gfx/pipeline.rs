use tracing::{debug, error, info, warn};

use super::{color::Color, objects::{quad::QuadsContainer, BasicVertex}, shader_compiler::ShaderCompiler};

pub struct GraphicsPipeline {
    window: sdl3::video::Window,
    pub gpu: sdl3::gpu::Device,
    pub clear_color: Color,
    
    pub quads: QuadsContainer,

    pub copy_pass: Option<sdl3::gpu::CopyPass>,
    pub copy_cmd_buffer: Option<sdl3::gpu::CommandBuffer>,

    pub basic_pipeline: sdl3::gpu::GraphicsPipeline,
}

impl GraphicsPipeline {
    pub fn new(window: sdl3::video::Window) -> Self {
        setup_logger();

        let clear_color = Color::from_rgb(0, 0, 0);
        let gpu = sdl3::gpu::Device::new(sdl3::gpu::ShaderFormat::SpirV, true)
            .expect("Failed to create GPU Device").with_window(&window).expect("Failed to claim window");
        let shader_compiler = ShaderCompiler::new();

        return Self {
            basic_pipeline: Self::make_basic_pipeline(gpu.clone(), window.clone(), shader_compiler),
            quads: QuadsContainer::new(gpu.clone()),

            window,
            gpu,

            clear_color,
            

            copy_pass: None,
            copy_cmd_buffer: None,
        };
    }

    fn make_basic_pipeline(gpu: sdl3::gpu::Device, window: sdl3::video::Window, compiler: ShaderCompiler) -> sdl3::gpu::GraphicsPipeline {
        let vertex_source = compiler.compile(include_str!("../../shaders/basic.hlsl.vert"), super::shader_compiler::ShaderKind::Vertex);
        let fragment_source = compiler.compile(include_str!("../../shaders/basic.hlsl.frag"), super::shader_compiler::ShaderKind::Fragment);
        
        let vertex = load_shader(gpu.clone(), &vertex_source, sdl3::gpu::ShaderStage::Vertex, 0, 0, 0, 0);
        let fragment = load_shader(gpu.clone(), &fragment_source, sdl3::gpu::ShaderStage::Fragment, 0, 0, 0, 0);
    
        let pipeline= gpu.create_graphics_pipeline()
            .with_vertex_shader(&vertex)
            .with_fragment_shader(&fragment)
            .with_vertex_input_state(
                sdl3::gpu::VertexInputState::new()
                    .with_vertex_buffer_descriptions(&[
                        sdl3::gpu::VertexBufferDescription::new()
                            .with_slot(0)
                            .with_pitch(size_of::<BasicVertex>() as u32)
                            .with_input_rate(sdl3::gpu::VertexInputRate::Vertex)
                            .with_instance_step_rate(0)
                    ])
                    .with_vertex_attributes(&[
                        sdl3::gpu::VertexAttribute::new()
                            .with_location(0)
                            .with_format(sdl3::gpu::VertexElementFormat::Float4)
                            .with_offset((size_of::<f32>() * 3) as u32),
                        sdl3::gpu::VertexAttribute::new()
                            .with_location(1)
                            .with_format(sdl3::gpu::VertexElementFormat::Float3)
                            .with_offset(0)
                    ])
            )
            .with_target_info(
                sdl3::gpu::GraphicsPipelineTargetInfo::new()
                    .with_color_target_descriptions(&[
                        sdl3::gpu::ColorTargetDescription::new()
                            .with_format(gpu.get_swapchain_texture_format(&window))
                    ])
            )
            .with_primitive_type(sdl3::gpu::PrimitiveType::TriangleList)
            .build().unwrap();

        drop(vertex);
        drop(fragment);

        return pipeline;
    }

    pub fn begin_upload(&mut self) {
        self.copy_cmd_buffer = Some(self.gpu.acquire_command_buffer().unwrap());
        self.copy_pass = Some(self.gpu.begin_copy_pass(self.copy_cmd_buffer.as_ref().unwrap()).unwrap());
    }

    pub fn end_upload(&mut self) {
        self.gpu.end_copy_pass(self.copy_pass.take().unwrap());
        _ = self.copy_cmd_buffer.take().unwrap().submit().unwrap();
    }

    pub fn update_object_list(&mut self) {
        self.quads.update(&self.gpu, self.copy_pass.as_ref().unwrap());
    }

    pub fn render(&mut self) {
        let mut cmd_buffer = self.gpu.acquire_command_buffer().unwrap();

        if let Ok(swapchain_texture) = cmd_buffer.wait_and_acquire_swapchain_texture(&self.window) {
            let color_targets = [
                sdl3::gpu::ColorTargetInfo::default()
                    .with_texture(&swapchain_texture)
                    .with_clear_color(self.clear_color.sdl_color())
                    .with_load_op(sdl3::gpu::LoadOp::Clear)
                    .with_store_op(sdl3::gpu::StoreOp::Store)];          
                  
            let render_pass = self.gpu.begin_render_pass(&cmd_buffer, &color_targets, None).unwrap();
            self.quads.render(self.basic_pipeline.clone(), &render_pass);

            self.gpu.end_render_pass(render_pass);

            cmd_buffer.submit().unwrap();
        } else {
            cmd_buffer.cancel();
        }
    }

    pub fn reset(&mut self) {
        self.quads.clear();
    }
}

fn setup_logger() {
    sdl3::log::set_output_function(|priority, category, msg| {
        match priority {
            sdl3::log::Priority::Verbose | sdl3::log::Priority::Debug => debug!("{:?}: {}", category, msg),
            sdl3::log::Priority::Info => info!("{:?}: {}", category, msg),
            sdl3::log::Priority::Warn => warn!("{:?}: {}", category, msg),
            sdl3::log::Priority::Critical | sdl3::log::Priority::Error => error!("{:?}: {}", category, msg),
        }
    });
    sdl3::log::set_log_priorities(sdl3::log::Priority::Verbose);
}

fn load_shader(gpu: sdl3::gpu::Device, code: &[u8], stage: sdl3::gpu::ShaderStage, _sampler_count: u32, _ubo_count: u32, _sbo_count: u32, _storage_tex_count: u32) -> sdl3::gpu::Shader {
    gpu.create_shader()
        .with_code(sdl3::gpu::ShaderFormat::SpirV, code, stage)
        .with_entrypoint("main")
        .build().unwrap()
}