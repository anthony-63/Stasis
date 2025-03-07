


use rusttype::{gpu_cache::{Cache, CacheBuilder}, point, Font, PositionedGlyph, Scale};
use sdl3::gpu::TransferBuffer;
use tracing::info;

use crate::core::{gfx::color::Color, Vector2, Vector3};

use super::{
    create_buffer_with_data, get_local_coords3, set_buffer_data, update_texture_with_data, TexturedVertex
};

pub struct CachedFont<'a> {
    size: f32,
    id: usize,
    path: String,
    font: rusttype::Font<'a>,

}

const TEXTURE_WIDTH: u32 = 2048;
const TEXTURE_HEIGHT: u32 = 2048;

const MAX_TEXT: usize = 131072;

pub struct TextObject {
    pub x: f32,
    pub y: f32,
    
    pub text: String,
    pub font_size: f32,

    pub w: f32,
    pub h: f32,

    z: f32,

    font_path: String,

    color: Color,

    pub should_update: bool,
    buffers: Option<TextObjectBuffers>,

    verts: Vec<TexturedVertex>,
}


fn layout_paragraph<'a>(
    font: &Font<'a>,
    scale: Scale,
    width: u32,
    text: &str,
) -> Vec<PositionedGlyph<'a>> {
    let mut result = Vec::new();
    let v_metrics = font.v_metrics(scale);
    let advance_height = v_metrics.ascent - v_metrics.descent + v_metrics.line_gap;
    let mut caret = point(0.0, v_metrics.ascent);
    let mut last_glyph_id = None;
    for c in text.chars() {
        if c.is_control() {
            match c {
                '\r' => {
                    caret = point(0.0, caret.y + advance_height);
                }
                '\n' => {}
                _ => {}
            }
            continue;
        }
        let base_glyph = font.glyph(c);
        if let Some(id) = last_glyph_id.take() {
            caret.x += font.pair_kerning(scale, id, base_glyph.id());
        }
        last_glyph_id = Some(base_glyph.id());
        let mut glyph = base_glyph.scaled(scale).positioned(caret);
        if let Some(bb) = glyph.pixel_bounding_box() {
            if bb.max.x > width as i32 {
                caret = point(0.0, caret.y + advance_height);
                glyph.set_position(caret);
                last_glyph_id = None;
            }
        }
        caret.x += glyph.unpositioned().h_metrics().advance_width;
        result.push(glyph);
    }
    result
}


impl TextObject {
    pub fn new(x: f32, y: f32, text: String, font_size: f32, color: Color, font_path: &str) -> Self {
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
            color,
            verts: vec![],
            font_path: font_path.to_string(),
        }
    }

    pub fn upload_text<'a>(&mut self, current_id: &mut usize, cache: &mut Cache<'a>, data: &mut [u8], cache_texture: &sdl3::gpu::Texture<'static>, transfer_buffer: &TransferBuffer, cached_fonts: &mut Vec<CachedFont<'a>>, gpu: &sdl3::gpu::Device, copy_pass: &sdl3::gpu::CopyPass) {
        let font;
        let id;
        if let Some(cached) = cached_fonts.iter().find(|f| f.size == self.font_size && f.path == self.font_path) {
            font = cached.font.clone();
            id = cached.id;
        } else {
            info!("Loading new font: '{}' ({}px)", self.font_path, self.font_size);
            font = Font::try_from_vec(std::fs::read(self.font_path.clone()).expect("Failed to open font file")).expect("Error constructing Font");
            *current_id += 1;
            id = *current_id;
            cached_fonts.push(CachedFont {
                font: font.clone(),
                id: *current_id,
                path: self.font_path.clone(),
                size: self.font_size,
            });
        }

        let scale = Scale::uniform(self.font_size);
        let text_str = &self.text;
        let color = (self.color.r, self.color.g, self.color.b);
        let v_metrics = font.v_metrics(scale);
        let glyphs: Vec<_> = font
            .layout(text_str, scale, point(0., 0. + v_metrics.ascent))
            .collect();

        for glyph in &glyphs {
            cache.queue_glyph(id, glyph.clone());
        }

        cache.cache_queued(|r, d| {
            let glyph_width = r.width() as usize;
            let glyph_height = r.height() as usize;
            for y in 0..glyph_height {
                for x in 0..glyph_width {
                    let pixel_index = y * glyph_width + x;
                    let alpha = d[pixel_index];
                    let atlas_index = (r.min.y as usize + y) * TEXTURE_WIDTH as usize + (r.min.x as usize + x);
    
                    if atlas_index < data.len() {
                        let base_index = atlas_index * 4;
                        data[base_index] = color.0;
                        data[base_index + 1] = color.1;
                        data[base_index + 2] = color.2;
                        data[base_index + 3] = alpha;
                    }
                }
            }
        }).unwrap();
        
        update_texture_with_data(gpu, cache_texture, transfer_buffer, data, TEXTURE_WIDTH, TEXTURE_HEIGHT, copy_pass);

        let mut x = self.x;
        let mut ci = 0;

        self.w = 0.;
        self.h = 0.;
        
        self.verts = glyphs
            .iter()
            .filter_map(|g| cache.rect_for(id, g).ok().flatten())
            .flat_map(|(coord, glyph_size)| {
                let w = glyph_size.width() as f32;
                self.w += w + 2.;

                let h = glyph_size.height() as f32;
                self.h = h;

                let y = self.y - (h - glyph_size.max.y as f32);
                let c = text_str.chars().nth(ci).unwrap();

                if c == ' ' {
                    x += w / 2.;
                }
                
                let r = [
                    TexturedVertex::new(get_local_coords3(Vector3::new(x, y, self.z)), Vector2::new(coord.min.x, coord.min.y)),
                    TexturedVertex::new(get_local_coords3(Vector3::new(x + w, y, self.z)), Vector2::new(coord.max.x, coord.min.y)),
                    TexturedVertex::new(get_local_coords3(Vector3::new(x + w, y + h, self.z)), Vector2::new(coord.max.x, coord.max.y)),
                    TexturedVertex::new(get_local_coords3(Vector3::new(x, y + h, self.z)), Vector2::new(coord.min.x, coord.max.y)),
                ];
                x += w + 2.;
                ci += 1;

                r
            }).collect();
    }

    pub fn update(&mut self) {
        self.should_update = true;
    }
}

pub struct TextObjectContainer<'a> {
    pub texts: Vec<TextObject>,
    text_id: usize,

    cached_fonts: Vec<CachedFont<'a>>,
    cache_texture: sdl3::gpu::Texture<'static>,
    cache_transfer_buffer: sdl3::gpu::TransferBuffer,
    data: Vec<u8>,
    cache: Cache<'a>,

    font_id: usize,

    sampler: Option<sdl3::gpu::Sampler>,
    transfer_buffer: sdl3::gpu::TransferBuffer,
}

impl TextObjectContainer<'_> {
    pub fn new(gpu: &sdl3::gpu::Device) -> Self {
        Self {
            texts: vec![],
            text_id: 0,
            font_id: 0,
            data: vec![0_u8; (TEXTURE_WIDTH * TEXTURE_HEIGHT) as usize * 4],
            sampler: None,
            cached_fonts: vec![],
            cache_texture: gpu.create_texture(
                sdl3::gpu::TextureCreateInfo::new()
                .with_format(sdl3::gpu::TextureFormat::R8g8b8a8Unorm)
                .with_type(sdl3::gpu::TextureType::_2D)
                .with_width(TEXTURE_WIDTH)
                .with_height(TEXTURE_HEIGHT)
                .with_layer_count_or_depth(1)
                .with_num_levels(1)
                .with_usage(sdl3::gpu::TextureUsage::Sampler | sdl3::gpu::TextureUsage::ColorTarget)).unwrap(),
            cache_transfer_buffer: gpu
                .create_transfer_buffer()
                .with_size(4 * TEXTURE_WIDTH * TEXTURE_HEIGHT)
                .with_usage(sdl3::gpu::TransferBufferUsage::Upload)
                .build().unwrap(),
            cache: CacheBuilder::default().dimensions(TEXTURE_WIDTH, TEXTURE_HEIGHT).build(),
            
            transfer_buffer: gpu.create_transfer_buffer()
                .with_size(((size_of::<TexturedVertex>() * 4 + size_of::<u16>() * 6) * MAX_TEXT) as u32)
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
            if text.should_update {
                text.upload_text(&mut self.font_id, &mut self.cache, &mut self.data, &self.cache_texture, &self.cache_transfer_buffer, &mut self.cached_fonts, gpu, copy_pass);
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
                    .with_texture(&self.cache_texture)
                    .with_sampler(self.sampler.as_ref().unwrap())],
            );
            render_pass.draw_indexed_primitives(6 * (text.verts.len() as u32 / 4), 1, 0, 0, 0);
        }
    }

    pub fn add_text(&mut self, obj: TextObject, z: f32, gpu: &sdl3::gpu::Device, copy_pass: &sdl3::gpu::CopyPass) -> usize {
        let mut text = obj;
        text.z = z;
        
        text.upload_text(&mut self.font_id, &mut self.cache, &mut self.data, &self.cache_texture, &self.cache_transfer_buffer, &mut self.cached_fonts, gpu, copy_pass);

        text.buffers = Some(TextObjectBuffers::setup(
            &text,
            &self.transfer_buffer,
            gpu,
            copy_pass,
        ));
    
        self.texts.push(text);

        self.text_id += 1;
        self.text_id - 1
    }

    pub fn clear(&mut self) {
        self.texts.clear();
        self.text_id = 0;
    }
}

pub struct TextObjectBuffers {
    vbo: sdl3::gpu::Buffer,
    ibo: sdl3::gpu::Buffer,
}

impl TextObjectBuffers {
    pub fn setup(
        obj: &TextObject,
        transfer_buffer: &TransferBuffer,
        gpu: &sdl3::gpu::Device,
        copy_pass: &sdl3::gpu::CopyPass,
    ) -> Self {
        let ind: Vec<u16> = vec![0, 1, 2, 0, 2, 3];
        let mut indices = vec![];

        let mut i = 0;

        for _ in obj.verts.iter().step_by(4) {
            let n: Vec<u16> = ind.iter().map(|f| f + i).collect();
            indices.extend_from_slice(&n);
            i += 4;
        }

        let vbo = create_buffer_with_data(
            gpu,
            transfer_buffer,
            copy_pass,
            sdl3::gpu::BufferUsageFlags::Vertex,
            &obj.verts,
        )
        .unwrap();

        let ibo = create_buffer_with_data(
            gpu,
            transfer_buffer,
            copy_pass,
            sdl3::gpu::BufferUsageFlags::Index,
            &indices,
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
        let ind: Vec<u16> = vec![0, 1, 2, 0, 2, 3];
        let mut indices = vec![];

        let mut i = 0;

        for _ in obj.verts.iter().step_by(4) {
            let n: Vec<u16> = ind.iter().map(|f| f + i).collect();
            indices.extend_from_slice(&n);
            i += 4;
        }

        set_buffer_data(
            gpu,
            &self.vbo,
            transfer_buffer,
            copy_pass,
            &obj.verts,
        )
        .unwrap();

        set_buffer_data(
            gpu,
            &self.ibo,
            transfer_buffer,
            copy_pass,
            &indices,
        )
        .unwrap();
    }
}