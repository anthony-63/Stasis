use sdl3::{gpu::{BlendFactor, BlendOp, ColorTargetBlendState, ColorTargetDescription, ColorTargetInfo, CommandBuffer, CompareOp, CopyPass, DepthStencilState, DepthStencilTargetInfo, Device, GraphicsPipeline, GraphicsPipelineTargetInfo, LoadOp, PrimitiveType, SampleCount, Shader, ShaderFormat, ShaderStage, StoreOp, Texture, TextureCreateInfo, TextureFormat, TextureType, TextureUsage, VertexAttribute, VertexBufferDescription, VertexElementFormat, VertexInputRate, VertexInputState}, video::Window};
use tracing::{debug, error, info, warn};

use crate::core::Vector2;

use super::{
    color::Color,
    objects::{camera::CameraObject, quad::QuadsContainer, sprite3d::Sprite3dObjectsContainer, text::TextObjectContainer, textured_quad::TexturedQuadsContainer, ColorVertex, TexturedVertex},
    shader_compiler::ShaderCompiler, WINDOW_SIZE,
};

pub struct RenderPipeline {
    pub gpu: Device,
    pub clear_color: Color,

    pub copy_pass: Option<CopyPass>,
    pub copy_cmd_buffer: Option<CommandBuffer>,
    
    pub depth_texture: Texture<'static>,

    pub basic_pipeline: GraphicsPipeline,
    pub quads: QuadsContainer,

    pub textured_quad_pipeline: GraphicsPipeline,
    pub textured_quads: TexturedQuadsContainer,

    pub text_object_pipeline: GraphicsPipeline,
    pub text_objects: TextObjectContainer<'static>,

    pub sprite_object_pipeline: GraphicsPipeline,
    pub sprites: Sprite3dObjectsContainer,

    pub bound_camera: Option<CameraObject>,
}

impl RenderPipeline {
    pub fn new(window: &sdl3::video::Window) -> Self {
        setup_logger();

        let clear_color = Color::from_rgb(0, 0, 0);
        let gpu = Device::new(ShaderFormat::SpirV, false)
            .expect("Failed to create GPU Device")
            .with_window(window)
            .expect("Failed to claim window");
        let shader_compiler = ShaderCompiler::new();

        let (width, height) = window.size();

        unsafe {
            WINDOW_SIZE = Some(Vector2::new(width as f32, height as f32));
        }

        Self {
            bound_camera: None,
            basic_pipeline: Self::make_basic_pipeline(
                &gpu,
                window.clone(),
                &shader_compiler,
            ),
            quads: QuadsContainer::new(gpu.clone()),

            textured_quad_pipeline: Self::make_textured_quad_pipeline(
                &gpu,
                window.clone(),
                &shader_compiler,
            ),
            textured_quads: TexturedQuadsContainer::new(&gpu),

            text_object_pipeline: Self::make_text_pipeline(
                &gpu,
                window.clone(),
                &shader_compiler,
            ),
            text_objects: TextObjectContainer::new(&gpu),

            sprite_object_pipeline: Self::make_sprite_pipeline(
                &gpu,
                window.clone(),
                &shader_compiler,
            ),
            sprites: Sprite3dObjectsContainer::new(&gpu),

            depth_texture: gpu.create_texture(
                TextureCreateInfo::new()
                    .with_type(TextureType::_2D)
                    .with_width(width)
                    .with_height(height)
                    .with_layer_count_or_depth(1)
                    .with_num_levels(1)
                    .with_sample_count(SampleCount::NoMultiSampling)
                    .with_format(TextureFormat::D16Unorm)
                    .with_usage(TextureUsage::Sampler | TextureUsage::DepthStencilTarget),
            ).unwrap(),
            gpu,

            clear_color,

            copy_pass: None,
            copy_cmd_buffer: None,
        }
    }

    fn make_basic_pipeline(
        gpu: &Device,
        window: sdl3::video::Window,
        compiler: &ShaderCompiler,
    ) -> GraphicsPipeline {
        let vertex_source = compiler.compile(
            include_str!("../../shaders/basic.vert"),
            super::shader_compiler::ShaderKind::Vertex,
        ).expect("Failed to compile basic.vert");
        let fragment_source = compiler.compile(
            include_str!("../../shaders/basic.frag"),
            super::shader_compiler::ShaderKind::Fragment,
        ).expect("Failed to compile basic.frag");
        
        let vertex = load_shader(
            gpu.clone(),
            &vertex_source,
            ShaderStage::Vertex,
            0,
            0,
            0,
            0,
        );
        let fragment = load_shader(
            gpu.clone(),
            &fragment_source,
            ShaderStage::Fragment,
            0,
            0,
            0,
            0,
        );

        let pipeline = gpu
            .create_graphics_pipeline()
            .with_vertex_shader(&vertex)
            .with_fragment_shader(&fragment)
            .with_vertex_input_state(
                VertexInputState::new()
                    .with_vertex_buffer_descriptions(&[VertexBufferDescription::new()
                        .with_slot(0)
                        .with_pitch(size_of::<ColorVertex>() as u32)
                        .with_input_rate(VertexInputRate::Vertex)
                        .with_instance_step_rate(0)])
                    .with_vertex_attributes(&[
                        VertexAttribute::new()
                            .with_location(0)
                            .with_format(VertexElementFormat::Float4)
                            .with_offset((size_of::<f32>() * 3) as u32),
                        VertexAttribute::new()
                            .with_location(1)
                            .with_format(VertexElementFormat::Float3)
                            .with_offset(0),
                    ]),
            )
            .with_depth_stencil_state(
                DepthStencilState::new()
                    .with_enable_depth_test(true)
                    .with_enable_depth_write(true)
                    .with_compare_op(CompareOp::Less),
            )
            .with_target_info(
                GraphicsPipelineTargetInfo::new()
                        .with_has_depth_stencil_target(true)
                        .with_depth_stencil_format(TextureFormat::D16Unorm)
                        .with_color_target_descriptions(&[ColorTargetDescription::new()
                        .with_blend_state(ColorTargetBlendState::new()
                            .with_enable_blend(true)
                            .with_alpha_blend_op(BlendOp::Add)
                            .with_color_blend_op(BlendOp::Add)
                            .with_dst_alpha_blendfactor(BlendFactor::OneMinusSrcAlpha)
                            .with_dst_color_blendfactor(BlendFactor::OneMinusSrcAlpha)
                            .with_src_alpha_blendfactor(BlendFactor::SrcAlpha)
                            .with_src_color_blendfactor(BlendFactor::SrcAlpha))
                        .with_format(gpu.get_swapchain_texture_format(&window))]),
            )
            .with_primitive_type(PrimitiveType::TriangleList)
            .build()
            .unwrap();

        drop(vertex);
        drop(fragment);

        pipeline
    }

    fn make_textured_quad_pipeline(
        gpu: &Device,
        window: sdl3::video::Window,
        compiler: &ShaderCompiler,
    ) -> GraphicsPipeline {
        let vertex_source = compiler.compile(
            include_str!("../../shaders/textured.vert"),
            super::shader_compiler::ShaderKind::Vertex,
        ).expect("Failed to compile textured.vert");
        let fragment_source = compiler.compile(
            include_str!("../../shaders/textured.frag"),
            super::shader_compiler::ShaderKind::Fragment,
        ).expect("Failed to compile textured.frag");

        let vertex = load_shader(
            gpu.clone(),
            &vertex_source,
            ShaderStage::Vertex,
            0,
            0,
            0,
            0,
        );
        let fragment = load_shader(
            gpu.clone(),
            &fragment_source,
            ShaderStage::Fragment,
            1,
            0,
            0,
            0,
        );

        let pipeline = gpu
            .create_graphics_pipeline()
            .with_vertex_shader(&vertex)
            .with_fragment_shader(&fragment)
            .with_vertex_input_state(
                VertexInputState::new()
                    .with_vertex_buffer_descriptions(&[VertexBufferDescription::new()
                        .with_slot(0)
                        .with_pitch(size_of::<TexturedVertex>() as u32)
                        .with_input_rate(VertexInputRate::Vertex)
                        .with_instance_step_rate(0)])
                    .with_vertex_attributes(&[
                        VertexAttribute::new()
                            .with_buffer_slot(0)
                            .with_location(0)
                            .with_format(VertexElementFormat::Float3)
                            .with_offset(0),
                        VertexAttribute::new()
                            .with_buffer_slot(0)
                            .with_location(1)
                            .with_format(VertexElementFormat::Float2)
                            .with_offset((size_of::<f32>() * 3) as u32),
                    ]),
            )
            .with_depth_stencil_state(
                DepthStencilState::new()
                    .with_enable_depth_test(true)
                    .with_enable_depth_write(true)
                    .with_compare_op(CompareOp::Less),
            )
            .with_target_info(
                GraphicsPipelineTargetInfo::new()
                        .with_has_depth_stencil_target(true)
                        .with_depth_stencil_format(TextureFormat::D16Unorm)
                        .with_color_target_descriptions(&[ColorTargetDescription::new()
                        .with_blend_state(ColorTargetBlendState::new()
                            .with_enable_blend(true)
                            .with_alpha_blend_op(BlendOp::Add)
                            .with_color_blend_op(BlendOp::Add)
                            .with_dst_alpha_blendfactor(BlendFactor::OneMinusSrcAlpha)
                            .with_dst_color_blendfactor(BlendFactor::OneMinusSrcAlpha)
                            .with_src_alpha_blendfactor(BlendFactor::SrcAlpha)
                            .with_src_color_blendfactor(BlendFactor::SrcAlpha))
                        .with_format(gpu.get_swapchain_texture_format(&window))]),
            )
            .with_primitive_type(PrimitiveType::TriangleList)
            .build()
            .unwrap();

        drop(vertex);
        drop(fragment);

        pipeline
    }

    fn make_sprite_pipeline(
        gpu: &Device,
        window: sdl3::video::Window,
        compiler: &ShaderCompiler,
    ) -> GraphicsPipeline {
        let vertex_source = compiler.compile(
            include_str!("../../shaders/three-d.vert"),
            super::shader_compiler::ShaderKind::Vertex,
        ).expect("Failed to compile three-d.vert");
        let fragment_source = compiler.compile(
            include_str!("../../shaders/three-d.frag"),
            super::shader_compiler::ShaderKind::Fragment,
        ).expect("Failed to compile three-d.frag");

        let vertex = load_shader(
            gpu.clone(),
            &vertex_source,
            ShaderStage::Vertex,
            0,
            1,
            0,
            0,
        );
        let fragment = load_shader(
            gpu.clone(),
            &fragment_source,
            ShaderStage::Fragment,
            1,
            0,
            0,
            0,
        );

        let pipeline = gpu
            .create_graphics_pipeline()
            .with_vertex_shader(&vertex)
            .with_fragment_shader(&fragment)
            .with_vertex_input_state(
                VertexInputState::new()
                    .with_vertex_buffer_descriptions(&[VertexBufferDescription::new()
                        .with_slot(0)
                        .with_pitch(size_of::<TexturedVertex>() as u32)
                        .with_input_rate(VertexInputRate::Vertex)
                        .with_instance_step_rate(0)])
                    .with_vertex_attributes(&[
                        VertexAttribute::new()
                            .with_buffer_slot(0)
                            .with_location(0)
                            .with_format(VertexElementFormat::Float3)
                            .with_offset(0),
                        VertexAttribute::new()
                            .with_buffer_slot(0)
                            .with_location(1)
                            .with_format(VertexElementFormat::Float2)
                            .with_offset((size_of::<f32>() * 3) as u32),
                    ]),
            )
            .with_depth_stencil_state(
                DepthStencilState::new()
                    .with_enable_depth_test(true)
                    .with_enable_depth_write(true)
                    .with_compare_op(CompareOp::Less),
            )
            .with_target_info(
                GraphicsPipelineTargetInfo::new()
                        .with_has_depth_stencil_target(true)
                        .with_depth_stencil_format(TextureFormat::D16Unorm)
                        .with_color_target_descriptions(&[ColorTargetDescription::new()
                        .with_blend_state(ColorTargetBlendState::new()
                            .with_enable_blend(true)
                            .with_alpha_blend_op(BlendOp::Add)
                            .with_color_blend_op(BlendOp::Add)
                            .with_dst_alpha_blendfactor(BlendFactor::OneMinusSrcAlpha)
                            .with_dst_color_blendfactor(BlendFactor::OneMinusSrcAlpha)
                            .with_src_alpha_blendfactor(BlendFactor::SrcAlpha)
                            .with_src_color_blendfactor(BlendFactor::SrcAlpha))
                        .with_format(gpu.get_swapchain_texture_format(&window))]),
            )
            .with_primitive_type(PrimitiveType::TriangleList)
            .build()
            .unwrap();

        drop(vertex);
        drop(fragment);

        pipeline
    }

    fn make_text_pipeline(
        gpu: &Device,
        window: sdl3::video::Window,
        compiler: &ShaderCompiler,
    ) -> GraphicsPipeline {
        let vertex_source = compiler.compile(
            include_str!("../../shaders/textured.vert"),
            super::shader_compiler::ShaderKind::Vertex,
        ).expect("Failed to compile textured.vert");
        let fragment_source = compiler.compile(
            include_str!("../../shaders/textured.frag"),
            super::shader_compiler::ShaderKind::Fragment,
        ).expect("Failed to compile textured.frag");

        let vertex = load_shader(
            gpu.clone(),
            &vertex_source,
            ShaderStage::Vertex,
            0,
            0,
            0,
            0,
        );
        let fragment = load_shader(
            gpu.clone(),
            &fragment_source,
            ShaderStage::Fragment,
            1,
            0,
            0,
            0,
        );

        let pipeline = gpu
            .create_graphics_pipeline()
            .with_vertex_shader(&vertex)
            .with_fragment_shader(&fragment)
            .with_vertex_input_state(
                VertexInputState::new()
                    .with_vertex_buffer_descriptions(&[VertexBufferDescription::new()
                        .with_slot(0)
                        .with_pitch(size_of::<TexturedVertex>() as u32)
                        .with_input_rate(VertexInputRate::Vertex)
                        .with_instance_step_rate(0)])
                    .with_vertex_attributes(&[
                        VertexAttribute::new()
                            .with_buffer_slot(0)
                            .with_location(0)
                            .with_format(VertexElementFormat::Float3)
                            .with_offset(0),
                        VertexAttribute::new()
                            .with_buffer_slot(0)
                            .with_location(1)
                            .with_format(VertexElementFormat::Float2)
                            .with_offset((size_of::<f32>() * 3) as u32),
                    ]),
            )
            .with_depth_stencil_state(
                DepthStencilState::new()
                    .with_enable_depth_test(true)
                    .with_enable_depth_write(true)
                    .with_compare_op(CompareOp::Less),
            )
            .with_target_info(
                GraphicsPipelineTargetInfo::new()
                    .with_has_depth_stencil_target(true)
                    .with_depth_stencil_format(TextureFormat::D16Unorm)
                    .with_color_target_descriptions(&[ColorTargetDescription::new()
                        .with_blend_state(ColorTargetBlendState::new()
                            .with_enable_blend(true)
                            .with_alpha_blend_op(BlendOp::Add)
                            .with_color_blend_op(BlendOp::Add)
                            .with_dst_alpha_blendfactor(BlendFactor::OneMinusSrcAlpha)
                            .with_dst_color_blendfactor(BlendFactor::OneMinusSrcAlpha)
                            .with_src_alpha_blendfactor(BlendFactor::SrcAlpha)
                            .with_src_color_blendfactor(BlendFactor::SrcAlpha))
                        .with_format(gpu.get_swapchain_texture_format(&window))]),
            )
            .with_primitive_type(PrimitiveType::TriangleList)
            .build()
            .unwrap();

        drop(vertex);
        drop(fragment);

        pipeline
    }
    

    /*
        .with_blend_state(ColorTargetBlendState::new()
                            .with_enable_blend(true)
                            .with_alpha_blend_op(BlendOp::Add)
                            .with_color_blend_op(BlendOp::Add)
                            .with_dst_alpha_blendfactor(BlendFactor::OneMinusSrcAlpha)
                            .with_dst_color_blendfactor(BlendFactor::OneMinusSrcColor)
                            .with_src_alpha_blendfactor(BlendFactor::SrcAlpha)
                            .with_src_color_blendfactor(BlendFactor::SrcAlpha))
     */

    pub fn begin_upload(&mut self) {
        self.copy_cmd_buffer = Some(self.gpu.acquire_command_buffer().unwrap());
        self.copy_pass = Some(
            self.gpu
                .begin_copy_pass(self.copy_cmd_buffer.as_ref().unwrap())
                .unwrap(),
        );
    }

    pub fn end_upload(&mut self) {
        self.gpu.end_copy_pass(self.copy_pass.take().unwrap());
        self.copy_cmd_buffer.take().unwrap().submit().unwrap();
    }

    pub fn resize(&mut self, w: u32, h: u32) {
        self.depth_texture = self.gpu.create_texture(
            TextureCreateInfo::new()
                .with_type(TextureType::_2D)
                .with_width(w)
                .with_height(h)
                .with_layer_count_or_depth(1)
                .with_num_levels(1)
                .with_sample_count(SampleCount::NoMultiSampling)
                .with_format(TextureFormat::D16Unorm)
                .with_usage(TextureUsage::Sampler | TextureUsage::DepthStencilTarget),
        ).unwrap()
    }

    pub fn update_object_list(&mut self) {
        self.quads
            .update(&self.gpu, self.copy_pass.as_ref().unwrap());
        self.textured_quads
            .update(&self.gpu, self.copy_pass.as_ref().unwrap());
        self.sprites
            .update(&self.gpu, self.copy_pass.as_ref().unwrap());
        self.text_objects
            .update(&self.gpu, self.copy_pass.as_ref().unwrap());
    }

    pub fn render(&mut self, window: &Window) {
        let mut cmd_buffer = self.gpu.acquire_command_buffer().unwrap();

        if let Ok(swapchain_texture) = cmd_buffer.wait_and_acquire_swapchain_texture(window) {
            let color_targets = [ColorTargetInfo::default()
                .with_texture(&swapchain_texture)
                .with_clear_color(self.clear_color.sdl_color())
                .with_load_op(LoadOp::Clear)
                .with_store_op(StoreOp::Store)];
            let depth_target = DepthStencilTargetInfo::new()
                .with_texture(&mut self.depth_texture)
                .with_cycle(false)
                .with_clear_depth(1.0)
                .with_clear_stencil(0)
                .with_load_op(LoadOp::Clear)
                .with_store_op(StoreOp::Store)
                .with_stencil_load_op(LoadOp::Clear)
                .with_stencil_store_op(StoreOp::Store);

            let render_pass = self
                .gpu
                .begin_render_pass(&cmd_buffer, &color_targets, Some(&depth_target))
                .unwrap();

            if self.bound_camera.is_some() {
                self.bound_camera.as_mut().unwrap().update_matrices();
                self.bound_camera.as_ref().unwrap().push_matrices(&cmd_buffer);
            }

            self.quads.render(self.basic_pipeline.clone(), &render_pass);
            self.textured_quads.render(self.textured_quad_pipeline.clone(), &render_pass);
            self.sprites.render(self.sprite_object_pipeline.clone(), &render_pass);
            self.text_objects.render(self.text_object_pipeline.clone(), &render_pass);


            self.gpu.end_render_pass(render_pass);

            cmd_buffer.submit().unwrap();
        } else {
            cmd_buffer.cancel();
        }
    }

    pub fn reset(&mut self) {
        self.quads.clear();
        self.textured_quads.clear();
        self.text_objects.clear();
        self.sprites.clear();
    }
}

fn setup_logger() {
    sdl3::log::set_output_function(|priority, category, msg| match priority {
        sdl3::log::Priority::Verbose | sdl3::log::Priority::Debug => {
            debug!("{:?}: {}", category, msg)
        }
        sdl3::log::Priority::Info => info!("{:?}: {}", category, msg),
        sdl3::log::Priority::Warn => warn!("{:?}: {}", category, msg),
        sdl3::log::Priority::Critical | sdl3::log::Priority::Error => {
            error!("{:?}: {}", category, msg)
        }
    });
    sdl3::log::set_log_priorities(sdl3::log::Priority::Verbose);
}

fn load_shader(
    gpu: Device,
    code: &[u8],
    stage: ShaderStage,
    sampler_count: u32,
    ubo_count: u32,
    sbo_count: u32,
    storage_tex_count: u32,
) -> Shader {
    gpu.create_shader()
        .with_code(ShaderFormat::SpirV, code, stage)
        .with_entrypoint("main")
        .with_samplers(sampler_count)
        .with_uniform_buffers(ubo_count)
        .with_storage_buffers(sbo_count)
        .with_storage_textures(storage_tex_count)
        .build()
        .unwrap()
}

