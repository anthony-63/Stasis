use color::Color;
use pipeline::GraphicsPipeline;

pub mod pipeline;
pub mod color;
pub mod shader_compiler;

pub struct Graphics {
    pipeline: GraphicsPipeline,
}

impl Graphics {
    pub fn new(window: sdl3::video::Window) -> Self {
        return Self {
            pipeline: GraphicsPipeline::new(window),
        }
    }

    pub fn set_clear_color(&mut self, color: Color) {
        self.pipeline.clear_color = color;
    }

    pub fn render(&mut self) {
        self.pipeline.render();
    }
}