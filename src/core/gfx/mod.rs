use color::Color;
use objects::{camera::CameraObject, quad::QuadObject, sprite3d::Sprite3dObject, text::TextObject, textured_quad::TexturedQuadObject};
use pipeline::RenderPipeline;
use sdl3::video::Window;

use super::Vector2;

pub mod color;
pub mod objects;
pub mod pipeline;
pub mod shader_compiler;

pub type ObjectId = usize;

static mut WINDOW_SIZE: Option<Vector2> = None;

pub struct Graphics {
    pipeline: RenderPipeline,

    z_index: f32,
}

impl Graphics {
    pub fn new(window: &sdl3::video::Window) -> Self {
        Self {
            pipeline: RenderPipeline::new(window),
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

    pub fn add_sprite(&mut self, sprite: Sprite3dObject) -> usize {        
        self.pipeline.sprites.add_sprite(sprite, &self.pipeline.gpu, self.pipeline.copy_pass.as_ref().unwrap())
    }

    pub fn add_text(&mut self, text: TextObject) -> usize {
        self.z_index -= 0.0001;
        
        self.pipeline.text_objects.add_text(text, self.z_index, &self.pipeline.gpu, self.pipeline.copy_pass.as_ref().unwrap())
    }

    pub fn bind_camera(&mut self, cam: CameraObject) {
        self.pipeline.bound_camera = Some(cam);
    }

    pub fn bound_camera(&self) -> &CameraObject {
        self.pipeline.bound_camera.as_ref().unwrap()
    }

    pub fn bound_camera_mut(&mut self) -> &mut CameraObject {
        self.pipeline.bound_camera.as_mut().unwrap()
    } 

    pub fn window_size() -> Vector2 {
        unsafe { WINDOW_SIZE.unwrap() }
    }

    pub fn quads(&self) -> &Vec<QuadObject> {
        &self.pipeline.quads.quads
    }

    pub fn sprite3ds(&self) -> &Vec<Sprite3dObject> {
        &self.pipeline.sprites.sprites
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

    pub fn sprite3ds_mut(&mut self) -> &mut Vec<Sprite3dObject> {
        &mut self.pipeline.sprites.sprites
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
 
    pub fn render(&mut self, window: &Window) {
        self.pipeline.render(window);
    }

    pub fn reset(&mut self) {
        self.pipeline.reset();
    }
}

