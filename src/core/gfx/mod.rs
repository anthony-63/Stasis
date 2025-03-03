use color::Color;
use objects::{quad::QuadObject, textured_quad::TexturedQuadObject};
use pipeline::GraphicsPipeline;

pub mod color;
pub mod objects;
pub mod pipeline;
pub mod shader_compiler;
pub mod text;

pub struct Graphics {
    pipeline: GraphicsPipeline,

    z_index: f32,
}

impl Graphics {
    pub fn new(window: sdl3::video::Window) -> Self {
        Self {
            pipeline: GraphicsPipeline::new(window),
            z_index: 1.,
        }
    }

    pub fn add_quad(&mut self, quad: QuadObject) -> usize {
        self.z_index -= 0.00001;
        
        self.pipeline.quads.add_quad(quad, self.z_index)
    }

    pub fn add_textured_quad(&mut self, quad: TexturedQuadObject) -> usize {
        self.z_index -= 0.00001;
        
        self.pipeline.textured_quads.add_quad(quad, self.z_index)
    }

    pub fn get_quads(&mut self) -> &mut Vec<QuadObject> {
        &mut self.pipeline.quads.quads
    }

    pub fn set_clear_color(&mut self, color: Color) {
        self.pipeline.clear_color = color;
    }

    pub fn update(&mut self) {
        self.pipeline.begin_upload();
        self.pipeline.update_object_list();
        self.pipeline.end_upload();
    }

    pub fn resize(&mut self, w: i32, h: i32) {
        self.pipeline.resize(w as u32, h as u32);
    }
 
    pub fn render(&mut self) {
        self.pipeline.render();
    }

    pub fn reset(&mut self) {
        self.pipeline.reset();
    }
}

