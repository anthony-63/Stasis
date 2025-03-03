

use rusttype::{point, Font, Scale};
use sdl3::gpu::TransferBuffer;
use tracing::info;

use crate::core::{Vector2, Vector3};

use super::{
    create_buffer_with_data, create_texture_from_data, get_local_coords3, set_buffer_data, TexturedVertex
};

pub struct CachedFont<'a> {
    size: f32,
    path: String,
    font: rusttype::Font<'a>,
}

pub struct TextObject {
    pub x: f32,
    pub y: f32,
    
    pub text: String,
    pub font_size: f32,

    w: f32,
    h: f32,

    z: f32,

    texture: Option<sdl3::gpu::Texture<'static>>,
    font_path: String,

    pub should_update: bool,
    buffers: Option<TexturedQuadBuffers>,
}

impl TextObject {
    pub fn new(x: f32, y: f32, text: String, font_size: f32, font_path: &str) -> Self {
        Self {
            x,
            y,
            text,
            w: 0.,
            h: 0.,
            font_size,
            z: 0.0,
            buffers: None,
            should_update: false,
            texture: None,
            font_path: font_path.to_string(),
        }
    }

    pub fn update(&mut self) {
        self.should_update = true;
    }
}

pub struct TextObjectContainer<'a> {
    pub texts: Vec<TextObject>,
    text_id: usize,

    cached_fonts: Vec<CachedFont<'a>>,

    sampler: Option<sdl3::gpu::Sampler>,
    transfer_buffer: sdl3::gpu::TransferBuffer,
}

impl TextObjectContainer<'_> {
    pub fn new(gpu: &sdl3::gpu::Device) -> Self {
        Self {
            texts: vec![],
            text_id: 0,
            sampler: None,
            cached_fonts: vec![],
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

        for text in self.texts.iter_mut() {
            if text.texture.is_none() {
                let font;
                if let Some(cached) = self.cached_fonts.iter().find(|f| f.size == text.font_size && f.path == text.font_path) {
                    info!("Loading cached font: '{}' ({}px)", cached.path, cached.size);
                    font = cached.font.clone();
                } else {
                    info!("Loading new font: '{}' ({}px)", text.font_path, text.font_size);
                    font = Font::try_from_vec(std::fs::read(text.font_path.clone()).expect("Failed to open font file")).expect("Error constructing Font");
                    self.cached_fonts.push(CachedFont {
                        font: font.clone(),
                        path: text.font_path.clone(),
                        size: text.font_size,
                    })
                }

                let scale = Scale::uniform(text.font_size);
                let text_str = &text.text;
                let color = (255, 0, 0);
                let v_metrics = font.v_metrics(scale);
                let glyphs: Vec<_> = font
                    .layout(text_str, scale, point(0., 0. + v_metrics.ascent))
                    .collect();

                // work out the layout size
                let glyphs_height = (v_metrics.ascent - v_metrics.descent).ceil() as u32;
                let glyphs_width = {
                    let min_x = glyphs
                        .first()
                        .map(|g| g.pixel_bounding_box().unwrap().min.x)
                        .unwrap();
                    let max_x = glyphs
                        .last()
                        .map(|g| g.pixel_bounding_box().unwrap().max.x)
                        .unwrap();
                    (max_x - min_x) as u32
                } + glyphs_height;

                let mut data = vec![[0_u8; 4]; (glyphs_width * glyphs_height) as usize];

                for glyph in &glyphs {
                    if let Some(bounding_box) = glyph.pixel_bounding_box() {
                        glyph.draw(|x, y, v| {
                            let pos = (((y + bounding_box.min.y as u32) * glyphs_width) + (x + bounding_box.min.x as u32)) as usize;
                            // info!("{}", pos);
                            data[pos][0] = color.0;
                            data[pos][1] = color.1;
                            data[pos][2] = color.2;
                            data[pos][3] = (v * 255.) as u8;
                        });
                    }
                }

                let mut data_flattened = vec![0_u8; (glyphs_width * glyphs_height) as usize * 4];

                for (i, pixel) in data.iter().enumerate() {
                    let real_index = i * 4;
                    data_flattened[real_index] = pixel[0];
                    data_flattened[real_index + 1] = pixel[1];
                    data_flattened[real_index + 2] = pixel[2];
                    data_flattened[real_index + 3] = pixel[3];
                }

                text.w = glyphs_width as f32;
                text.h = glyphs_height as f32;
                let texture_result = create_texture_from_data(gpu, &data_flattened, glyphs_width, glyphs_height, copy_pass).unwrap();
                text.texture = Some(texture_result.0);
            }
            if text.buffers.is_none() {
                text.buffers = Some(TexturedQuadBuffers::setup(
                    text,
                    &self.transfer_buffer,
                    gpu,
                    copy_pass,
                ));
            } else if text.should_update {
                text.buffers.as_ref().unwrap().update(
                    text,
                    &self.transfer_buffer,
                    gpu,
                    copy_pass,
                );
                text.should_update = false;
            }
        }
    }

    pub fn render(
        &mut self,
        pipeline: sdl3::gpu::GraphicsPipeline,
        render_pass: &sdl3::gpu::RenderPass,
    ) {
        render_pass.bind_graphics_pipeline(&pipeline);
        for text in self.texts.iter() {
            if text.buffers.is_none() {
                continue;
            }
            render_pass.bind_vertex_buffers(
                0,
                &[
                    sdl3::gpu::BufferBinding::new()
                        .with_buffer(&text.buffers.as_ref().unwrap().vbo),
                ],
            );
            render_pass.bind_index_buffer(
                &sdl3::gpu::BufferBinding::new()
                    .with_buffer(&text.buffers.as_ref().unwrap().ibo),
                    sdl3::gpu::IndexElementSize::_16Bit
            );
            render_pass.bind_fragment_sampler(
                0,
                &[sdl3::gpu::TextureSamplerBinding::new()
                    .with_texture(text.texture.as_ref().unwrap())
                    .with_sampler(self.sampler.as_ref().unwrap())],
            );
            render_pass.draw_indexed_primitives(6, 1, 0, 0, 0);
        }
    }

    pub fn add_text(&mut self, obj: TextObject, z: f32) -> usize {
        let mut t = obj;
        t.z = z;
        self.texts.push(t);

        self.text_id += 1;
        self.text_id - 1
    }

    pub fn clear(&mut self) {
        self.texts.clear();
    }
}

pub struct TexturedQuadBuffers {
    vbo: sdl3::gpu::Buffer,
    ibo: sdl3::gpu::Buffer,
}

impl TexturedQuadBuffers {
    pub fn setup(
        obj: &TextObject,
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
        obj: &TextObject,
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

