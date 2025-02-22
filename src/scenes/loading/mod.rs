use tracing::info;

use crate::core::{gfx::{color::Color, objects::quad::QuadObject, Graphics}, scene::Scene};

pub struct LoadingScene {
    test_quad_id: usize,
    timer: f64,
}

impl LoadingScene {
    pub fn new() -> Self {
        Self {
            test_quad_id: 0,
            timer: 0.,
        }
    }
}

impl Scene for LoadingScene {
    fn init(&mut self, gfx: &mut Graphics) {
        self.test_quad_id = gfx.add_quad(QuadObject::new(10., 10., 200., 200., Color::from_rgb(255, 0, 0)));
        gfx.set_clear_color(Color::from_frgb(0.3, 0.2, 0.5));
    }

    fn update(&mut self, gfx: &mut Graphics, dt: f64) -> Option<Box<dyn Scene + 'static>> {
        let quads = gfx.get_quads();
        self.timer += dt;
        quads[self.test_quad_id].color.r = ((self.timer * 63.75) % 255.) as u8;
        quads[self.test_quad_id].color.g = ((self.timer * 127.5) % 255.) as u8;
        quads[self.test_quad_id].color.b = ((self.timer * 191.25) % 255.) as u8;
        quads[self.test_quad_id].update();
        
        None
    }
}