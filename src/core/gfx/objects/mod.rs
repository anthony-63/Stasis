use sdl3::{gpu::{Buffer, BufferRegion, BufferUsageFlags, CopyPass, Device, TransferBuffer, TransferBufferLocation}, Error};

use super::color::Color;

pub mod quad;

#[repr(packed)]
#[derive(Copy, Clone)]
pub struct BasicVertex {
    pub vertex: [f32; 3],
    pub color: [f32; 4],
}

impl BasicVertex {
    pub fn new(vertex: [f32; 3], color: Color) -> Self {
        return Self {
            vertex,
            color: color.get_floats(),
        }
    }
}


pub fn get_local_coords2(c: [f32; 2]) -> [f32; 2] {
    [
        (1.0 / 1920.) * c[0] - 1.,
        1. - (1.0 / 1080.) * c[1],
    ]
}

pub fn get_local_coords3(c: [f32; 3]) -> [f32; 3] {
    [
        (1.0 / 1920.) * c[0] - 1.,
        1. - (1.0 / 1080.) * c[1],
        c[2]
    ]
}

pub fn create_buffer_with_data<T: Copy>(
    gpu: &Device,
    transfer_buffer: &TransferBuffer,
    copy_pass: &CopyPass,
    usage: BufferUsageFlags,
    data: &[T],
) -> Result<Buffer, Error> {
    // Figure out the length of the data in bytes
    let len_bytes = data.len() * std::mem::size_of::<T>();

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