

use crate::{content::maploader::MapLoader, core::{gfx::{color::Color, objects::{text::TextObject, textured_quad::TexturedQuadObject}, Graphics, ObjectId}, input::Input, scene::Scene, Vector2}};

use super::{global::fps_counter::FpsCounter, menu::MenuScene};

#[derive(Default)]
pub struct LoadingScene {
    loading_text: ObjectId,
    fps: Option<FpsCounter>,
    first_render: bool,
}

impl LoadingScene {
    pub fn new() -> Self {
        Self {
            first_render: false,
            fps: None,
            ..Default::default()
        }
    }
}

impl Scene for LoadingScene {
    fn init(&mut self, gfx: &mut Graphics, _input: &mut Input) {
        gfx.set_clear_color(Color::from_frgb(0.05, 0.0025, 0.1));

        let window_size = Graphics::window_size();
        self.loading_text = gfx.add_text(TextObject::new(window_size.x / 2., 0., "Loading Stasis".into(), 58.,  Color::from_rgb(199, 199, 199), "Assets/Game/font.ttf", ));
        
        self.fps = Some(FpsCounter::new(gfx));

        let icon_size = Vector2::new(200., 200.);
        gfx.add_textured_quad(TexturedQuadObject::new(window_size.x / 2. - icon_size.x / 2., window_size.y / 2. - icon_size.y, icon_size.x, icon_size.y, "Assets/Game/Icon.png"));

        let loading_text = &mut gfx.text_objects_mut()[self.loading_text];
        loading_text.x = window_size.x / 2. - loading_text.w / 2.;
        loading_text.y = window_size.y / 2. - loading_text.y / 2.;
        loading_text.update();
    }

    fn update(&mut self, _gfx: &mut Graphics, _input: &mut Input, _dt: f64) -> Option<Box<dyn Scene + 'static>> {
        if self.first_render {
            let maps = MapLoader::load_all_from_dir("Assets/Maps".to_string());
            return Some(Box::new(MenuScene::new(maps)))
        }

        self.first_render = true;

        None
    }
    
}