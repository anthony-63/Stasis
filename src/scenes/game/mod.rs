use tracing::info;

use crate::{content::maps::beatmapset::BeatmapSet, core::{gfx::{color::Color, Graphics}, scene::Scene}};

use super::global::fps_counter::FpsCounter;

pub struct GameScene {
    map: BeatmapSet,

    fps_counter: Option<FpsCounter>,
}

impl GameScene {
    pub fn new(map: BeatmapSet) -> Self {
        Self {
            map,
            fps_counter: None,
        }
    }
}

impl Scene for GameScene {
    fn init(&mut self, gfx: &mut Graphics) {
        gfx.set_clear_color(Color::from_frgb(0.01, 0.01, 0.01));

        self.map.load_difficulties();

        info!("Playing map: {}[{}]", self.map.title, self.map.difficulties[0].name);
        self.fps_counter = Some(FpsCounter::new(gfx));
    }

    fn update(&mut self, gfx: &mut Graphics, dt: f64) -> Option<Box<dyn Scene + 'static>> {
        self.fps_counter.as_mut().unwrap().update(gfx, dt);
        
        None
    }

}