use crate::core::gfx::{color::Color, Graphics};

use super::{create_buffer_with_data, get_local_coords3, BasicVertex};

pub struct QuadObject {
    x: f32,
    y: f32,
    w: f32,
    h: f32,
    color: Color,

    buffers: Option<QuadBuffers>
}

impl QuadObject {
    pub fn new(x: f32, y: f32, w: f32, h: f32, color: Color) -> Self {
        Self {
            x, y, w, h, color,
            buffers: None,
        }
    }
}

pub struct QuadsContainer {
    pub quads: Vec<QuadObject>,
    transfer_buffer: sdl3::gpu::TransferBuffer,
}

impl QuadsContainer {
    pub fn new(gpu: sdl3::gpu::Device) -> Self {
        Self {
            quads: vec![],
            transfer_buffer: 
                gpu.create_transfer_buffer()
                    .with_size((size_of::<BasicVertex>() * 4 + size_of::<u16>() * 6) as u32)
                    .with_usage(sdl3::gpu::TransferBufferUsage::Upload)
                    .build().unwrap(),
        }
    }

    pub fn update(&mut self, gpu: &sdl3::gpu::Device, copy_pass: &sdl3::gpu::CopyPass) {
        for quad in self.quads.iter_mut() {
            if quad.buffers.is_none() {
                quad.buffers = Some(
                    QuadBuffers::setup(quad, self.transfer_buffer.clone(), gpu, copy_pass)
                );
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

    pub fn clear(&mut self) {
        self.quads.clear();
    }
}

pub struct QuadBuffers {
    vbo: sdl3::gpu::Buffer,
    ibo: sdl3::gpu::Buffer,
}

impl QuadBuffers {
    pub fn setup(obj: &QuadObject, transfer_buffer: sdl3::gpu::TransferBuffer, gpu: &sdl3::gpu::Device, copy_pass: &sdl3::gpu::CopyPass) -> Self {
        let vertices = [
            BasicVertex::new(get_local_coords3([obj.x, obj.y, 0.]), obj.color.clone()),
            BasicVertex::new(get_local_coords3([obj.x + obj.w, obj.y, 0.]), obj.color.clone()),
            BasicVertex::new(get_local_coords3([obj.x + obj.w, obj.y + obj.h, 0.]), obj.color.clone()),
            BasicVertex::new(get_local_coords3([obj.x, obj.y + obj.h, 0.]), obj.color.clone()),
        ];
        let indices: &[u16] = &[0, 1, 2, 0, 2, 3];
        
        let vbo = create_buffer_with_data(&gpu, &transfer_buffer, &copy_pass, sdl3::gpu::BufferUsageFlags::Vertex, &vertices).unwrap();
        let ibo = create_buffer_with_data(&gpu, &transfer_buffer, &copy_pass, sdl3::gpu::BufferUsageFlags::Index, &indices).unwrap();
        return Self {
            vbo, ibo,
        }
    }
}