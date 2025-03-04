use color::Color;
use objects::{quad::QuadObject, text::TextObject, textured_quad::TexturedQuadObject};
use pipeline::GraphicsPipeline;

use super::Vector2;

pub mod color;
pub mod objects;
pub mod pipeline;
pub mod shader_compiler;

static mut WINDOW_SIZE: Option<Vector2> = None;

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
        self.z_index -= 0.0001;
        
        self.pipeline.quads.add_quad(quad, self.z_index, &self.pipeline.gpu, self.pipeline.copy_pass.as_ref().unwrap())
    }

    pub fn add_textured_quad(&mut self, quad: TexturedQuadObject) -> usize {
        self.z_index -= 0.0001;
        
        self.pipeline.textured_quads.add_quad(quad, self.z_index, &self.pipeline.gpu, self.pipeline.copy_pass.as_ref().unwrap())
    }

    pub fn add_text(&mut self, text: TextObject) -> usize {
        self.z_index -= 0.0001;
        
        self.pipeline.text_objects.add_text(text, self.z_index, &self.pipeline.gpu, self.pipeline.copy_pass.as_ref().unwrap())
    }

    pub fn window_size() -> Vector2 {
        unsafe { WINDOW_SIZE.unwrap() }
    }

    pub fn quads(&self) -> &Vec<QuadObject> {
        &self.pipeline.quads.quads
    }

    pub fn textured_quads(&self) -> &Vec<TexturedQuadObject> {
        &self.pipeline.textured_quads.quads
    }

    pub fn text_objects(&self) -> &Vec<TextObject> {
        &self.pipeline.text_objects.texts
    }

    pub fn quads_mut(&mut self) -> &mut Vec<QuadObject> {
        &mut self.pipeline.quads.quads
    }

    pub fn textured_quads_mut(&mut self) -> &mut Vec<TexturedQuadObject> {
        &mut self.pipeline.textured_quads.quads
    }

    pub fn text_objects_mut(&mut self) -> &mut Vec<TextObject> {
        &mut self.pipeline.text_objects.texts
    }

    pub fn set_clear_color(&mut self, color: Color) {
        self.pipeline.clear_color = color;
    }

    pub fn update(&mut self) {
        self.pipeline.begin_upload();
        self.pipeline.update_object_list();
        self.pipeline.end_upload();
    }

    pub fn begin_upload(&mut self) {
        self.pipeline.begin_upload();
    }

    pub fn end_upload(&mut self) {
        self.pipeline.end_upload();
    }

    pub fn resize(&mut self, w: i32, h: i32) {
        unsafe {
            WINDOW_SIZE = Some(Vector2::new(w as f32, h as f32));
        }
        self.pipeline.resize(w as u32, h as u32);
    }
 
    pub fn render(&mut self) {
        self.pipeline.render();
    }

    pub fn reset(&mut self) {
        self.pipeline.reset();
    }
}

