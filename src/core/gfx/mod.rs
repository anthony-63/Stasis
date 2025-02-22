use color::Color;
use objects::quad::QuadObject;
use pipeline::GraphicsPipeline;

pub mod pipeline;
pub mod color;
pub mod shader_compiler;
pub mod objects;

pub struct Graphics {
    pipeline: GraphicsPipeline,
}

impl Graphics {
    pub fn new(window: sdl3::video::Window) -> Self {
        return Self {
            pipeline: GraphicsPipeline::new(window),
        }
    }

    pub fn add_quad(&mut self, quad: QuadObject) {
        self.pipeline.quads.quads.push(quad);
    }

    pub fn set_clear_color(&mut self, color: Color) {
        self.pipeline.clear_color = color;
    }

    pub fn update(&mut self) {
        self.pipeline.begin_upload();
        self.pipeline.update_object_list();
        self.pipeline.end_upload();
    }

    pub fn render(&mut self) {
        self.pipeline.render();
    }

    pub fn reset(&mut self) {
        self.pipeline.reset();
    }
}