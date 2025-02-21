use tracing::{debug, error, info, warn};

use super::color::Color;

pub struct GraphicsPipeline {
    gpu: sdl3::gpu::Device,
    window: sdl3::video::Window,
    pub clear_color: Color,
}

impl GraphicsPipeline {
    pub fn new(window: sdl3::video::Window) -> Self {
        setup_logger();

        let clear_color = Color::from_rgb(0, 0, 0);
        let gpu = sdl3::gpu::Device::new(sdl3::gpu::ShaderFormat::SpirV, true)
            .expect("Failed to create GPU Device").with_window(&window).expect("Failed to claim window");

        return Self {
            window,
            gpu,
            clear_color,
        };
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

            self.gpu.end_render_pass(render_pass);

            cmd_buffer.submit().unwrap();
        } else {
            cmd_buffer.cancel();
        }
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