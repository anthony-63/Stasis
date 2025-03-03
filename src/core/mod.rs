pub mod window;
pub mod scene;
pub mod gfx;
pub mod ui;

#[repr(packed)]
#[derive(Debug, Clone, Copy, Default)]
pub struct Vector2 {
    pub x: f32,
    pub y: f32
}

impl Vector2 {
    pub fn new(x: f32, y: f32) -> Self {
        Self {
            x, y
        }
    }
}

#[repr(packed)]
#[derive(Debug, Clone, Copy, Default)]
pub struct Vector3 {
    pub x: f32,
    pub y: f32,
    pub z: f32,
}

impl Vector3 {
    pub fn new(x: f32, y: f32, z: f32) -> Self {
        Self {
            x, y, z
        }
    }
}