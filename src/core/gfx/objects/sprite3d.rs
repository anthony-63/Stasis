
use crate::core::{Vector2, Vector3};

use sdl3::gpu::{Buffer, BufferBinding, BufferUsageFlags, CopyPass, Device, Filter, GraphicsPipeline, IndexElementSize, RenderPass, Sampler, SamplerAddressMode, SamplerCreateInfo, SamplerMipmapMode, Texture, TextureSamplerBinding, TransferBuffer, TransferBufferUsage};

use super::{
    create_buffer_with_data, create_texture_from_image, set_buffer_data, TexturedVertex
};

pub struct Sprite3dObject {
    pub x: f32,
    pub y: f32,
    pub w: f32,
    pub h: f32,

    z: f32,

    texture: Option<Texture<'static>>,
    texture_path: String,

    pub should_update: bool,
    buffers: Option<Sprite3dObjectBuffers>,
}

impl Sprite3dObject {
    pub fn new_plane(x: f32, y: f32, z: f32, w: f32, h: f32, image_path: &str) -> Self {
        Self {
            x,
            y,
            w,
            h,
            z,
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

pub struct Sprite3dObjectsContainer {
    pub sprites: Vec<Sprite3dObject>,
    sprite_id: usize,

    sampler: Option<Sampler>,
    transfer_buffer: TransferBuffer,
}

impl Sprite3dObjectsContainer {
    pub fn new(gpu: &Device) -> Self {
        Self {
            sprites: vec![],
            sprite_id: 0,
            sampler: None,
            transfer_buffer: gpu.create_transfer_buffer()
                .with_size((size_of::<TexturedVertex>() * 4 + size_of::<u16>() * 6) as u32)
                .with_usage(TransferBufferUsage::Upload)
                .build().unwrap(),
        }
    }

    pub fn update(&mut self, gpu: &Device, copy_pass: &CopyPass) {
        if self.sampler.is_none() {
            self.sampler = Some(
                gpu.create_sampler(
                    SamplerCreateInfo::new()
                        .with_min_filter(Filter::Linear)
                        .with_mag_filter(Filter::Linear)
                        .with_mipmap_mode(SamplerMipmapMode::Linear)
                        .with_address_mode_u(SamplerAddressMode::Repeat)
                        .with_address_mode_v(SamplerAddressMode::Repeat)
                        .with_address_mode_w(SamplerAddressMode::Repeat),
                )
                .unwrap(),
            );
        }

        for sprite in self.sprites.iter_mut() {
            if sprite.should_update {
                sprite.buffers.as_ref().unwrap().update(
                    sprite,
                    &self.transfer_buffer,
                    gpu,
                    copy_pass,
                );
                sprite.should_update = false;
            }
        }
    }

    pub fn render(
        &mut self,
        pipeline: GraphicsPipeline,
        render_pass: &RenderPass,
    ) {
        render_pass.bind_graphics_pipeline(&pipeline);

        for sprite in self.sprites.iter() {
            if sprite.buffers.is_none() {
                continue;
            }
            render_pass.bind_vertex_buffers(
                0,
                &[
                    BufferBinding::new()
                        .with_buffer(&sprite.buffers.as_ref().unwrap().vbo),
                ],
            );
            render_pass.bind_index_buffer(
                &BufferBinding::new()
                    .with_buffer(&sprite.buffers.as_ref().unwrap().ibo),
                    IndexElementSize::_16Bit
            );
            render_pass.bind_fragment_sampler(
                0,
                &[TextureSamplerBinding::new()
                    .with_texture(sprite.texture.as_ref().unwrap())
                    .with_sampler(self.sampler.as_ref().unwrap())],
            );
            render_pass.draw_indexed_primitives(6, 1, 0, 0, 0);
        }
    }

    pub fn add_sprite(&mut self, obj: Sprite3dObject, gpu: &Device, copy_pass: &CopyPass) -> usize {
        let mut t: Sprite3dObject = obj;

        let texture_result =
            create_texture_from_image(gpu, t.texture_path.clone(), copy_pass).unwrap();
        t.texture = Some(texture_result.0);
        t.buffers = Some(Sprite3dObjectBuffers::setup(
            &t,
            &self.transfer_buffer,
            gpu,
            copy_pass,
        ));

        self.sprites.push(t);

        self.sprite_id += 1;
        self.sprite_id - 1
    }

    pub fn clear(&mut self) {
        self.sprites.clear();
        self.sprite_id = 0;
    }
}

pub struct Sprite3dObjectBuffers {
    vbo: Buffer,
    ibo: Buffer,
}

impl Sprite3dObjectBuffers {
    pub fn setup(
        obj: &Sprite3dObject,
        transfer_buffer: &TransferBuffer,
        gpu: &Device,
        copy_pass: &CopyPass,
    ) -> Self {
        let vertices = [
            TexturedVertex::new(Vector3::new(obj.x, obj.y, obj.z), Vector2::new(0., 0.)),
            TexturedVertex::new(Vector3::new(obj.x + obj.w, obj.y, obj.z), Vector2::new(1., 0.)),
            TexturedVertex::new(Vector3::new(obj.x + obj.w, obj.y + obj.h, obj.z), Vector2::new(1., 1.)),
            TexturedVertex::new(Vector3::new(obj.x, obj.y + obj.h, obj.z), Vector2::new(0., 1.)),
        ];

        let indices: &[u16] = &[0, 1, 2, 0, 2, 3];

        let vbo = create_buffer_with_data(
            gpu,
            transfer_buffer,
            copy_pass,
            BufferUsageFlags::Vertex,
            &vertices,
        )
        .unwrap();

        let ibo = create_buffer_with_data(
            gpu,
            transfer_buffer,
            copy_pass,
            BufferUsageFlags::Index,
            indices,
        )
        .unwrap();

        Self { vbo, ibo }
    }

    pub fn update(
        &self,
        obj: &Sprite3dObject,
        transfer_buffer: &TransferBuffer,
        gpu: &Device,
        copy_pass: &CopyPass,
    ) {
        let vertices = [
            TexturedVertex::new(Vector3::new(obj.x, obj.y, obj.z), Vector2::new(0., 0.)),
            TexturedVertex::new(Vector3::new(obj.x + obj.w, obj.y, obj.z), Vector2::new(4., 0.)),
            TexturedVertex::new(
                Vector3::new(obj.x + obj.w, obj.y + obj.h, obj.z),
                Vector2::new(4., 4.),
            ),
            TexturedVertex::new(Vector3::new(obj.x, obj.y + obj.h, obj.z), Vector2::new(0., 4.)),
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

