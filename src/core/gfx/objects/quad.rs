
use std::fmt::Debug;

use crate::core::{gfx::color::Color, Vector3};

use super::{create_buffer_with_data, get_local_coords3, set_buffer_data, ColorVertex};

#[derive(Debug)]
pub struct QuadObject {
    pub x: f32,
    pub y: f32,
    pub w: f32,
    pub h: f32,

    z: f32,

    pub color: Color,
    
    pub should_update: bool,
    buffers: Option<QuadBuffers>,
}

impl QuadObject {
    pub fn new(x: f32, y: f32, w: f32, h: f32, color: Color) -> Self {
        Self {
            x, y, w, h, color,
            buffers: None,
            z: 0.,
            should_update: false
        }
    }

    pub fn update(&mut self) {
        self.should_update = true;
    }
}

pub struct QuadsContainer {
    pub quads: Vec<QuadObject>,
    quad_id: usize,
    transfer_buffer: sdl3::gpu::TransferBuffer,
}

impl QuadsContainer {
    pub fn new(gpu: sdl3::gpu::Device) -> Self {
        Self {
            quads: vec![],
            quad_id: 0,
            transfer_buffer: 
                gpu.create_transfer_buffer()
                    .with_size((size_of::<ColorVertex>() * 4 + size_of::<u16>() * 6) as u32)
                    .with_usage(sdl3::gpu::TransferBufferUsage::Upload)
                    .build().unwrap(),
        }
    }

    pub fn update(&mut self, gpu: &sdl3::gpu::Device, copy_pass: &sdl3::gpu::CopyPass) {
        for quad in self.quads.iter_mut() {
            if quad.should_update {
                quad.buffers.as_ref().unwrap().update(quad, self.transfer_buffer.clone(), gpu, copy_pass);
                quad.should_update = false;
            }
        }
    }

    pub fn render(&mut self, pipeline: sdl3::gpu::GraphicsPipeline, render_pass: &sdl3::gpu::RenderPass) {
        render_pass.bind_graphics_pipeline(&pipeline);
        for quad in self.quads.iter() {
            if quad.buffers.is_none() {
                continue;
            }
            render_pass.bind_vertex_buffers(0, &[
                sdl3::gpu::BufferBinding::new()
                    .with_buffer(&quad.buffers.as_ref().unwrap().vbo)
            ]);
            render_pass.bind_index_buffer(
                &sdl3::gpu::BufferBinding::new()
                    .with_buffer(&quad.buffers.as_ref().unwrap().ibo),
                    sdl3::gpu::IndexElementSize::_16Bit
            );
            render_pass.draw_indexed_primitives(6, 1, 0, 0, 0);
        }
    }

    pub fn add_quad(&mut self, obj: QuadObject, z: f32, gpu: &sdl3::gpu::Device, copy_pass: &sdl3::gpu::CopyPass) -> usize {
        let mut t = obj;
        t.z = z;

        t.buffers = Some(
            QuadBuffers::setup(&t, self.transfer_buffer.clone(), gpu, copy_pass)
        );

        self.quads.push(t);
        self.quad_id += 1;
        self.quad_id - 1
    }

    pub fn clear(&mut self) {
        self.quads.clear();
        self.quad_id = 0;
    }
}

pub struct QuadBuffers {
    vbo: sdl3::gpu::Buffer,
    ibo: sdl3::gpu::Buffer,
}

impl Debug for QuadBuffers {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        f.debug_struct("QuadBuffers").finish()
    }
}

impl QuadBuffers {
    pub fn setup(obj: &QuadObject, transfer_buffer: sdl3::gpu::TransferBuffer, gpu: &sdl3::gpu::Device, copy_pass: &sdl3::gpu::CopyPass) -> Self {
        let vertices = [
            ColorVertex::new(get_local_coords3(Vector3::new(obj.x, obj.y, obj.z)), obj.color.clone()),
            ColorVertex::new(get_local_coords3(Vector3::new(obj.x + obj.w, obj.y, obj.z)), obj.color.clone()),
            ColorVertex::new(get_local_coords3(Vector3::new(obj.x + obj.w, obj.y + obj.h, obj.z)), obj.color.clone()),
            ColorVertex::new(get_local_coords3(Vector3::new(obj.x, obj.y + obj.h, obj.z)), obj.color.clone()),
        ];

        let indices: &[u16] = &[0, 1, 2, 0, 2, 3];
        
        let vbo = create_buffer_with_data(gpu, &transfer_buffer, copy_pass, sdl3::gpu::BufferUsageFlags::Vertex, &vertices).unwrap();
        let ibo = create_buffer_with_data(gpu, &transfer_buffer, copy_pass, sdl3::gpu::BufferUsageFlags::Index, indices).unwrap();
        Self {
            vbo, ibo,
        }
    }

    pub fn update(&self, obj: &QuadObject, transfer_buffer: sdl3::gpu::TransferBuffer, gpu: &sdl3::gpu::Device, copy_pass: &sdl3::gpu::CopyPass) {
        let vertices = [
            ColorVertex::new(get_local_coords3(Vector3::new(obj.x, obj.y, 0.)), obj.color.clone()),
            ColorVertex::new(get_local_coords3(Vector3::new(obj.x + obj.w, obj.y, 0.)), obj.color.clone()),
            ColorVertex::new(get_local_coords3(Vector3::new(obj.x + obj.w, obj.y + obj.h, 0.)), obj.color.clone()),
            ColorVertex::new(get_local_coords3(Vector3::new(obj.x, obj.y + obj.h, 0.)), obj.color.clone()),
        ];
        set_buffer_data(gpu, &self.vbo, &transfer_buffer, copy_pass, &vertices).unwrap();
    }
}