
use crate::{content::maploader::MapLoader, core::{gfx::{color::Color, objects::{quad::QuadObject, textured_quad::TexturedQuadObject}, Graphics}, scene::Scene}};

use super::menu::MenuScene;

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
        gfx.set_clear_color(Color::from_frgb(0.3, 0.2, 0.5));

        gfx.add_quad(QuadObject::new(30., 30., 50., 50., Color::from_rgb(255, 0, 0)));
        self.test_quad_id = gfx.add_quad(QuadObject::new(40., 40., 50., 50., Color::from_rgba(255, 255, 0, 100)));
        gfx.add_textured_quad(TexturedQuadObject::new(50., 50., 100., 100., "Assets/Game/cat.png"));

        // let maps: Vec<crate::content::maps::beatmapset::BeatmapSet> = MapLoader::load_all_from_dir("Assets/Maps".into());
    }

    fn update(&mut self, gfx: &mut Graphics, dt: f64) -> Option<Box<dyn Scene + 'static>> {
        let quads = gfx.get_quads();
        self.timer += dt;

        // quads[self.test_quad_id].color.r = ((self.timer * 63.75) % 255.) as u8;
        // quads[self.test_quad_id].color.g = ((self.timer * 127.5) % 255.) as u8;
        // quads[self.test_quad_id].color.b = ((self.timer * 191.25) % 255.) as u8;
        // quads[self.test_quad_id].update();
        
        Some(Box::new(MenuScene::new()))
    }
}