use std::path::Path;

use sdl3::{gpu::{Buffer, BufferRegion, BufferUsageFlags, CopyPass, Device, Texture, TextureCreateInfo, TextureFormat, TextureRegion, TextureTransferInfo, TextureType, TextureUsage, TransferBuffer, TransferBufferLocation, TransferBufferUsage}, image, surface::Surface, Error};
use tracing::info;
use crate::core::{Vector2, Vector3};

use super::color::Color;

pub mod quad;
pub mod textured_quad;

#[repr(packed)]
#[derive(Copy, Clone, Debug)]
pub struct ColorVertex {
    pub vertex: Vector3,
    pub color: [f32; 4],
}

impl ColorVertex {
    pub fn new(vertex: Vector3, color: Color) -> Self {
        Self {
            vertex,
            color: color.get_floats(),
        }
    }
}

#[repr(packed)]
#[derive(Clone, Debug, Copy)]
pub struct TexturedVertex {
    pub vertex: Vector3,
    pub coord: Vector2,
}

impl TexturedVertex {
    pub fn new(vertex: Vector3, coord: Vector2) -> Self {
        Self {
            vertex,
            coord,
        }
    }
}

const RIGHT: f32 = 1920.;
const LEFT: f32 = 0.;
const TOP: f32 = 0.;
const BOTTOM: f32 = 1080.;

pub fn get_local_coords2(c: Vector2) -> Vector2 {
    Vector2::new(
        (2.) / (RIGHT - LEFT) * c.x - 1.,
        (2.) / (TOP - BOTTOM) * c.y + 1.,
    )
}

pub fn get_local_coords3(c: Vector3) -> Vector3 {
    Vector3::new(
        (2.) / (RIGHT - LEFT) * c.x - 1.,
        (2.) / (TOP - BOTTOM) * c.y + 1.,
        c.z,
    )
}

pub fn create_buffer_with_data<T: Copy>(
    gpu: &Device,
    transfer_buffer: &TransferBuffer,
    copy_pass: &CopyPass,
    usage: BufferUsageFlags,
    data: &[T],
) -> Result<Buffer, Error> {
    // Figure out the length of the data in bytes
    let len_bytes = std::mem::size_of_val(data);

    // Create the buffer with the size and usage we want
    let buffer = gpu
        .create_buffer()
        .with_size(len_bytes as u32)
        .with_usage(usage)
        .build()?;

    let mut map = transfer_buffer.map::<T>(gpu, true);
    let mem = map.mem_mut();
    for (index, &value) in data.iter().enumerate() {
        mem[index] = value;
    }

    map.unmap();

    copy_pass.upload_to_gpu_buffer(
        TransferBufferLocation::new()
            .with_offset(0)
            .with_transfer_buffer(transfer_buffer),
        BufferRegion::new()
            .with_offset(0)
            .with_size(len_bytes as u32)
            .with_buffer(&buffer),
        true,
    );

    Ok(buffer)
}

pub fn set_buffer_data<T: Copy>(
    gpu: &Device,
    buffer: &Buffer,
    transfer_buffer: &TransferBuffer,
    copy_pass: &CopyPass,
    data: &[T],
) -> Result<(), Error> {
    // Figure out the length of the data in bytes
    let len_bytes = std::mem::size_of_val(data);

    let mut map = transfer_buffer.map::<T>(gpu, true);
    let mem = map.mem_mut();
    for (index, &value) in data.iter().enumerate() {
        mem[index] = value;
    }

    map.unmap();

    copy_pass.upload_to_gpu_buffer(
        TransferBufferLocation::new()
            .with_offset(0)
            .with_transfer_buffer(transfer_buffer),
        BufferRegion::new()
            .with_offset(0)
            .with_size(len_bytes as u32)
            .with_buffer(buffer),
        true,
    );

    Ok(())
}

fn create_texture_from_image(
    gpu: &Device,
    image_path: impl AsRef<Path>,
    copy_pass: &CopyPass
) -> Result<(Texture<'static>, Vec<u8>), Error> {
    let image: Surface = image::LoadSurface::from_file(image_path.as_ref()).unwrap();
    let pixels = unsafe { image.without_lock() }.unwrap();
    
    let (width, height) = image.size();

    let size_bytes = 4 * width * height;

    info!("Loading texture: '{}' with size: {}", image_path.as_ref().to_str().unwrap(), size_bytes);
    
    let texture = gpu.create_texture(
        TextureCreateInfo::new()
        .with_format(TextureFormat::R8g8b8a8Unorm)
        .with_type(TextureType::_2D)
        .with_width(width)
            .with_height(height)
            .with_layer_count_or_depth(1)
            .with_num_levels(1)
            .with_usage(TextureUsage::Sampler | TextureUsage::ColorTarget)
        )?;
        
    let transfer_buffer = gpu
        .create_transfer_buffer()
        .with_size(size_bytes)
        .with_usage(TransferBufferUsage::Upload)
        .build()?;
    
    let mut buffer_mem = transfer_buffer.map::<u8>(gpu, false);
    let mut alphaplus = 0;
    for (i, _) in pixels.iter().enumerate().step_by(3) {
        buffer_mem.mem_mut()[i + alphaplus] = pixels[i];
        buffer_mem.mem_mut()[i + 1 + alphaplus] = pixels[i + 1];
        buffer_mem.mem_mut()[i + 2 + alphaplus] = pixels[i + 2];
        buffer_mem.mem_mut()[i + 3 + alphaplus] = 255;
        alphaplus += 1;
    }
    buffer_mem.unmap();

    copy_pass.upload_to_gpu_texture(
        TextureTransferInfo::new()
            .with_offset(0)
            .with_transfer_buffer(&transfer_buffer),
        TextureRegion::new()
            .with_texture(&texture)
            .with_width(width)
            .with_height(height)
            .with_depth(1),
        false,
    );
    
    Ok((texture, pixels.to_vec()))
}
