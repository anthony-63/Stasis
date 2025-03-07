use crate::{content::maps::beatmapset::BeatmapSet, core::{gfx::{color::Color, Graphics}, scene::Scene}};

use super::global::fps_counter::FpsCounter;

pub struct MenuScene {
    maps: Vec<BeatmapSet>,

    fps_counter: Option<FpsCounter>,
}

impl MenuScene {
    pub fn new(maps: Vec<BeatmapSet>) -> Self {
        Self {
            maps,
            fps_counter: None,
        }
    }
}

impl Scene for MenuScene {
    fn init(&mut self, gfx: &mut Graphics) {
        gfx.set_clear_color(Color::from_frgb(0.01, 0.01, 0.01));
        
        self.fps_counter = Some(FpsCounter::new(gfx));
    }

    fn update(&mut self, gfx: &mut Graphics, dt: f64) -> Option<Box<dyn Scene + 'static>> {
        self.fps_counter.as_mut().unwrap().update(gfx, dt);
        
        None
    }

}