#[derive(Clone)]
pub struct Color {
    pub r: u8, pub g: u8, pub b: u8, pub a: u8,
}

impl Color {
    pub fn from_rgb(r: u8, g: u8, b: u8) -> Self {
        Self {
            r, g, b,
            a: 255,
        }
    }

    pub fn from_rgba(r: u8, g: u8, b: u8, a: u8) -> Self {
        Self {
            r, g, b, a,
        }
    }

    pub fn from_frgb(r: f32, g: f32, b: f32) -> Self {
        Self {
            r: (r * 255.0) as u8,
            g: (g * 255.0) as u8,
            b: (b * 255.0) as u8,
            a: 255,
        }
    }

    pub fn from_frgba(r: f32, g: f32, b: f32, a: f32) -> Self {
        Self {
            r: (r * 255.0) as u8,
            g: (g * 255.0) as u8,
            b: (b * 255.0) as u8,
            a: (a * 255.0) as u8,
        }
    }

    pub fn get_floats(&self) -> [f32; 4] {
        [self.r as f32 / 255., self.g as f32 / 255., self.b as f32 / 255., self.a as f32 / 255.]
    }

    pub fn sdl_color(&self) -> sdl3::pixels::Color {
        sdl3::pixels::Color::RGBA(self.r, self.g, self.b, self.a)
    }
}