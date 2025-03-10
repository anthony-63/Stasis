use crate::{content::{maps::beatmapset::BeatmapSet, settings::Settings}, core::{gfx::{color::Color, Graphics}, input::Input, scene::Scene}};

use super::{game::GameScene, global::fps_counter::FpsCounter};

#[derive(Default)]
pub struct MenuScene {
    maps: Vec<BeatmapSet>,

    fps_counter: Option<FpsCounter>,
    settings: Settings,
}

impl MenuScene {
    pub fn new(maps: Vec<BeatmapSet>) -> Self {
        Self {
            maps,
            fps_counter: None,
            settings: Settings::load("settings.toml"),
            ..Default::default()
        }
    }
}

impl Scene for MenuScene {
    fn init(&mut self, gfx: &mut Graphics, _input: &mut Input) {
        gfx.set_clear_color(Color::from_frgb(0.01, 0.01, 0.01));
        self.settings.save("settings.toml");
        self.fps_counter = Some(FpsCounter::new(gfx));
    }

    fn update(&mut self, gfx: &mut Graphics, _input: &mut Input, dt: f64) -> Option<Box<dyn Scene + 'static>> {
        self.fps_counter.as_mut().unwrap().update(gfx, dt);

        Some(Box::new(GameScene::new(self.maps[3].clone(), self.settings.clone())))
    }

}