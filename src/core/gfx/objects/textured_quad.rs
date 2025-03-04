
use sdl3::gpu::TransferBuffer;

use crate::core::{Vector2, Vector3};

use super::{
    create_buffer_with_data, create_texture_from_image, get_local_coords3, set_buffer_data, TexturedVertex
};

pub struct TexturedQuadObject {
    pub x: f32,
    pub y: f32,
    pub w: f32,
    pub h: f32,

    z: f32,

    texture: Option<sdl3::gpu::Texture<'static>>,
    texture_path: String,

    pub should_update: bool,
    buffers: Option<TexturedQuadBuffers>,
}

impl TexturedQuadObject {
    pub fn new(x: f32, y: f32, w: f32, h: f32, image_path: &str) -> Self {
        Self {
            x,
            y,
            w,
            h,
            z: 0.0,
            buffers: None,
            should_update: false,
            texture: None,
            texture_path: image_path.to_string(),
        }
    }

    pub fn update(&mut self) {
        self.should_update = true;
    }
}

pub struct TexturedQuadsContainer {
    pub quads: Vec<TexturedQuadObject>,
    quad_id: usize,

    sampler: Option<sdl3::gpu::Sampler>,
    transfer_buffer: sdl3::gpu::TransferBuffer,
}

impl TexturedQuadsContainer {
    pub fn new(gpu: &sdl3::gpu::Device) -> Self {
        Self {
            quads: vec![],
            quad_id: 0,
            sampler: None,
            transfer_buffer: gpu.create_transfer_buffer()
                .with_size((size_of::<TexturedVertex>() * 4 + size_of::<u16>() * 6) as u32)
                .with_usage(sdl3::gpu::TransferBufferUsage::Upload)
                .build().unwrap(),
        }
    }

    pub fn update(&mut self, gpu: &sdl3::gpu::Device, copy_pass: &sdl3::gpu::CopyPass) {
        if self.sampler.is_none() {
            self.sampler = Some(
                gpu.create_sampler(
                    sdl3::gpu::SamplerCreateInfo::new()
                        .with_min_filter(sdl3::gpu::Filter::Linear)
                        .with_mag_filter(sdl3::gpu::Filter::Linear)
                        .with_mipmap_mode(sdl3::gpu::SamplerMipmapMode::Linear)
                        .with_address_mode_u(sdl3::gpu::SamplerAddressMode::Repeat)
                        .with_address_mode_v(sdl3::gpu::SamplerAddressMode::Repeat)
                        .with_address_mode_w(sdl3::gpu::SamplerAddressMode::Repeat),
                )
                .unwrap(),
            );
        }

        for quad in self.quads.iter_mut() {
            if quad.should_update {
                quad.buffers.as_ref().unwrap().update(
                    quad,
                    &self.transfer_buffer,
                    gpu,
                    copy_pass,
                );
                quad.should_update = false;
            }
        }
    }

    pub fn render(
        &mut self,
        pipeline: sdl3::gpu::GraphicsPipeline,
        render_pass: &sdl3::gpu::RenderPass,
    ) {
        render_pass.bind_graphics_pipeline(&pipeline);

        for quad in self.quads.iter() {
            if quad.buffers.is_none() {
                continue;
            }
            render_pass.bind_vertex_buffers(
                0,
                &[
                    sdl3::gpu::BufferBinding::new()
                        .with_buffer(&quad.buffers.as_ref().unwrap().vbo),
                ],
            );
            render_pass.bind_index_buffer(
                &sdl3::gpu::BufferBinding::new()
                    .with_buffer(&quad.buffers.as_ref().unwrap().ibo),
                    sdl3::gpu::IndexElementSize::_16Bit
            );
            render_pass.bind_fragment_sampler(
                0,
                &[sdl3::gpu::TextureSamplerBinding::new()
                    .with_texture(quad.texture.as_ref().unwrap())
                    .with_sampler(self.sampler.as_ref().unwrap())],
            );
            render_pass.draw_indexed_primitives(6, 1, 0, 0, 0);
        }
    }

    pub fn add_quad(&mut self, obj: TexturedQuadObject, z: f32, gpu: &sdl3::gpu::Device, copy_pass: &sdl3::gpu::CopyPass) -> usize {
        let mut t = obj;
        t.z = z;

        let texture_result =
            create_texture_from_image(gpu, t.texture_path.clone(), copy_pass).unwrap();
        t.texture = Some(texture_result.0);
        t.buffers = Some(TexturedQuadBuffers::setup(
            &t,
            &self.transfer_buffer,
            gpu,
            copy_pass,
        ));

        self.quads.push(t);

        self.quad_id += 1;
        self.quad_id - 1
    }

    pub fn clear(&mut self) {
        self.quads.clear();
    }
}

pub struct TexturedQuadBuffers {
    vbo: sdl3::gpu::Buffer,
    ibo: sdl3::gpu::Buffer,
}

impl TexturedQuadBuffers {
    pub fn setup(
        obj: &TexturedQuadObject,
        transfer_buffer: &TransferBuffer,
        gpu: &sdl3::gpu::Device,
        copy_pass: &sdl3::gpu::CopyPass,
    ) -> Self {
        let vertices = [
            TexturedVertex::new(get_local_coords3(Vector3::new(obj.x, obj.y, obj.z)), Vector2::new(0., 0.)),
            TexturedVertex::new(get_local_coords3(Vector3::new(obj.x + obj.w, obj.y, obj.z)), Vector2::new(1., 0.)),
            TexturedVertex::new(get_local_coords3(Vector3::new(obj.x + obj.w, obj.y + obj.h, obj.z)), Vector2::new(1., 1.)),
            TexturedVertex::new(get_local_coords3(Vector3::new(obj.x, obj.y + obj.h, obj.z)), Vector2::new(0., 1.)),
        ];

        let indices: &[u16] = &[0, 1, 2, 0, 2, 3];

        let vbo = create_buffer_with_data(
            gpu,
            transfer_buffer,
            copy_pass,
            sdl3::gpu::BufferUsageFlags::Vertex,
            &vertices,
        )
        .unwrap();

        let ibo = create_buffer_with_data(
            gpu,
            transfer_buffer,
            copy_pass,
            sdl3::gpu::BufferUsageFlags::Index,
            indices,
        )
        .unwrap();

        Self { vbo, ibo }
    }

    pub fn update(
        &self,
        obj: &TexturedQuadObject,
        transfer_buffer: &TransferBuffer,
        gpu: &sdl3::gpu::Device,
        copy_pass: &sdl3::gpu::CopyPass,
    ) {
        let vertices = [
            TexturedVertex::new(get_local_coords3(Vector3::new(obj.x, obj.y, obj.z)), Vector2::new(0., 0.)),
            TexturedVertex::new(get_local_coords3(Vector3::new(obj.x + obj.w, obj.y, obj.z)), Vector2::new(4., 0.)),
            TexturedVertex::new(
                get_local_coords3(Vector3::new(obj.x + obj.w, obj.y + obj.h, obj.z)),
                Vector2::new(4., 4.),
            ),
            TexturedVertex::new(get_local_coords3(Vector3::new(obj.x, obj.y + obj.h, obj.z)), Vector2::new(0., 4.)),
        ];
        set_buffer_data(
            gpu,
            &self.vbo,
            transfer_buffer,
            copy_pass,
            &vertices,
        )
        .unwrap();
    }
}

