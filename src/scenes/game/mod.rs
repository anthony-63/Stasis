use tracing::info;

use crate::{content::maps::beatmapset::BeatmapSet, core::{gfx::{color::Color, objects::{camera::CameraObject, sprite3d::Sprite3dObject}, Graphics, ObjectId}, scene::Scene, Vector3}};

use super::global::fps_counter::FpsCounter;


#[derive(Default)]
pub struct GameScene {
    map: BeatmapSet,

    grid: ObjectId,
    cursor: ObjectId,
    fps_counter: Option<FpsCounter>,
}

impl GameScene {
    pub fn new(map: BeatmapSet) -> Self {
        Self {
            map,
            fps_counter: None,
            ..Default::default()
        }
    }
}

impl Scene for GameScene {
    fn init(&mut self, gfx: &mut Graphics) {
        gfx.set_clear_color(Color::from_frgb(0.01, 0.01, 0.01));

        self.map.load_difficulties();
        info!("Playing map: {}[{}]", self.map.title, self.map.difficulties[0].name);

        let cursor_scale = 0.6;

        self.cursor = gfx.add_sprite(Sprite3dObject::new_plane(-cursor_scale / 2., -cursor_scale / 2., 0., cursor_scale, cursor_scale, "Assets/Game/Cursor.png"));
        self.grid = gfx.add_sprite(Sprite3dObject::new_plane(-3., -3., 0., 6., 6., "Assets/Game/Grid.png"));
        
        self.fps_counter = Some(FpsCounter::new(gfx));

        gfx.bind_camera(CameraObject::new(70.0, Vector3::new(0., 0., 7.), Vector3::zero()));
    }

    fn update(&mut self, gfx: &mut Graphics, dt: f64) -> Option<Box<dyn Scene + 'static>> {
        self.fps_counter.as_mut().unwrap().update(gfx, dt);
        
        None
    }

}