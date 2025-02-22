use crate::core::{gfx::{color::Color, objects::quad::QuadObject, Graphics}, scene::Scene};

pub struct LoadingScene {

}

impl LoadingScene {
    pub fn new() -> Self {
        Self {}
    }
}

impl Scene for LoadingScene {
    fn init(&mut self, gfx: &mut Graphics) {
        gfx.set_clear_color(Color::from_frgb(0.3, 0.2, 0.5));
        gfx.add_quad(QuadObject::new(10., 10., 200., 200., Color::from_rgb(255, 0, 0)));
        gfx.add_quad(QuadObject::new(400., 400., 200., 900., Color::from_rgb(255, 133, 0)));
    }

    fn update(&mut self, _dt: f64) -> Option<Box<dyn Scene + 'static>> {
        None
    }
}