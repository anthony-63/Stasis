
use chrono::Local;
use tracing::info;

use crate::{content::maploader::MapLoader, core::{gfx::{color::Color, objects::{quad::QuadObject, text::TextObject, textured_quad::TexturedQuadObject}, Graphics}, scene::Scene, Vector2}};

use super::{global::fps_counter::FpsCounter, menu::MenuScene};


pub struct LoadingScene {
    loading_text: usize,
    fps: Option<FpsCounter>,
}

impl LoadingScene {
    pub fn new() -> Self {
        Self {
            loading_text: 0,
            fps: None,
        }
    }
}

impl Scene for LoadingScene {
    fn init(&mut self, gfx: &mut Graphics) {
        gfx.set_clear_color(Color::from_frgb(0.05, 0.0025, 0.1));

        let window_size = Graphics::window_size();
        self.loading_text = gfx.add_text(TextObject::new(window_size.x / 2., 0., "Loading Stasis...".into(), 58.,  Color::from_rgb(199, 199, 199), "Assets/Game/font.ttf", ));
        
        self.fps = Some(FpsCounter::new(gfx));

        let icon_size = Vector2::new(200., 200.);
        gfx.add_textured_quad(TexturedQuadObject::new(window_size.x / 2. - icon_size.x / 1.5, window_size.y / 2. - icon_size.y, icon_size.x, icon_size.y, "Assets/Game/Icon.png"));

        let loading_text = &mut gfx.text_objects_mut()[self.loading_text];
        loading_text.x = window_size.x / 2. - loading_text.w / 2.;
        loading_text.y = window_size.y / 2. - loading_text.y / 2.;
        loading_text.update();
    }

    fn update(&mut self, gfx: &mut Graphics, dt: f64) -> Option<Box<dyn Scene + 'static>> {
        self.fps.as_mut().unwrap().update(gfx, dt);
        
        None
        // Some(Box::new(MenuScene::new()))
    }
    
}