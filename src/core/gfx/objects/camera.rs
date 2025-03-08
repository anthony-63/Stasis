
use glam::{Mat4, Vec3};
use sdl3::gpu::CommandBuffer;

use crate::core::{gfx::Graphics, Vector3};

pub struct CameraObject {
    pub fov: f32,
    pub position: Vector3,
    pub rotation: Vector3,
    mvp: Mat4,
}

impl CameraObject {
    pub fn new(fov: f32, position: Vector3, rotation: Vector3) -> Self {
        Self {
            fov,
            position,
            rotation,
            mvp: Mat4::default(),
        }
    }

    pub fn push_matrices(&self, cmd_buffer: &CommandBuffer) {
        cmd_buffer.push_vertex_uniform_data(0, &self.mvp);
    }

    pub fn update_matrices(&mut self) {
        let aspect_ratio = Graphics::window_size().x / Graphics::window_size().y;
        let proj = Mat4::perspective_infinite_lh(self.fov.to_radians(), aspect_ratio, 0.001);
        
        let mut model = Mat4::IDENTITY;
        model *= Mat4::from_rotation_x(self.rotation.y);
        model *= Mat4::from_rotation_y(self.rotation.x);
        model *= Mat4::from_rotation_z(self.rotation.z);

        model *= Mat4::from_translation(Vec3::new(self.position.x, self.position.y, self.position.z));

        let view = Mat4::IDENTITY;

        self.mvp = (proj * view) * model;
    }
}