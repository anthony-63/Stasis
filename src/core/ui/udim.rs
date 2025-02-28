pub struct UDim {
    pub scale: f32,
    pub offset: f32,
}

impl UDim {
    pub fn new(scale: f32, offset: f32) -> Self {
        Self {
            scale, offset
        }
    }

    pub fn zero() -> Self {
        Self {
            scale: 0.,
            offset: 0.,
        }
    } 
}

pub struct UDim2 {
    pub x: UDim,
    pub y: UDim,
}

impl UDim2 {
    pub fn new(x_scale: f32, x_offset: f32, y_scale: f32, y_offset: f32) -> Self {
        Self {
            x: UDim::new(x_scale, x_offset),
            y: UDim::new(y_scale, y_offset),
        }
    }

    pub fn zero() -> Self {
        Self {
            x: UDim::zero(),
            y: UDim::zero(),
        }
    }

    pub fn fill() -> Self {
        Self::new(1., 0., 1., 0.)
    }
}