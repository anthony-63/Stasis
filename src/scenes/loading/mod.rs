use crate::core::{gfx::{color::Color, Graphics}, scene::Scene};

pub struct LoadingScene {

}

impl LoadingScene {
    pub fn new() -> Self {
        Self {}
    }
}

impl Scene for LoadingScene {
    fn update(&mut self, _dt: f64) -> Option<Box<dyn Scene + 'static>> {
        None
    }

    fn render(&mut self, gfx: &mut Graphics) {
        gfx.set_clear_color(Color::from_frgb(0.3, 0.2, 0.5));
    }
}