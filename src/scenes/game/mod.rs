use crate::{content::maps::beatmapset::BeatmapSet, core::{gfx::{color::Color, Graphics}, scene::Scene}};

pub struct GameScene {
}

impl GameScene {
    pub fn new(map: BeatmapSet) -> Self {
        Self {
        }
    }
}

impl Scene for GameScene {
    fn init(&mut self, gfx: &mut Graphics) {
        gfx.set_clear_color(Color::from_frgb(0.01, 0.01, 0.01));
        
    }

    fn update(&mut self, gfx: &mut Graphics, _dt: f64) -> Option<Box<dyn Scene + 'static>> {
        // let quads = gfx.get_quads();
        
        None
    }
}