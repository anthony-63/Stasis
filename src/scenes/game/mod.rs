use player::Player;
use tracing::info;

use crate::{content::maps::beatmapset::BeatmapSet, core::{gfx::{color::Color, objects::{camera::CameraObject, sprite3d::Sprite3dObject}, Graphics, ObjectId}, input::Input, scene::Scene, Vector3}};

use super::global::fps_counter::FpsCounter;
pub mod player;

#[derive(Default)]
pub struct GameScene {
    map: BeatmapSet,

    player: Option<Player>,
    fps_counter: Option<FpsCounter>,
}

impl GameScene {
    pub fn new(map: BeatmapSet) -> Self {
        Self {
            map,
            fps_counter: None,
            player: None,
            ..Default::default()
        }
    }
}

impl Scene for GameScene {
    fn init(&mut self, gfx: &mut Graphics) {
        gfx.set_clear_color(Color::from_frgb(0.01, 0.01, 0.01));

        self.map.load_difficulties();
        info!("Playing map: {}[{}]", self.map.title, self.map.difficulties[0].name);

        self.player = Some(Player::new(gfx, 0.6));
        self.fps_counter = Some(FpsCounter::new(gfx));

        gfx.bind_camera(CameraObject::new(70.0, Vector3::new(0., 0., 7.), Vector3::zero()));
    }

    fn update(&mut self, gfx: &mut Graphics, input: &mut Input, dt: f64) -> Option<Box<dyn Scene + 'static>> {
        input.lock_cursor();

        self.fps_counter.as_mut().unwrap().update(gfx, dt);
        self.player.as_mut().unwrap().update(gfx, input);
        
        None
    }

}