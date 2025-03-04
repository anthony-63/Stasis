use tracing::info;

use super::gfx::Graphics;

pub trait Scene {
    fn init(&mut self, gfx: &mut Graphics);
    fn update(&mut self, gfx: &mut Graphics, dt: f64) -> Option<Box<dyn Scene + 'static>>;
}

pub struct SceneSwapper {
    pub current_scene: Box<dyn Scene + 'static>,
}

impl SceneSwapper {
    pub fn new<T>(initial_scene: T) -> Self
    where T: Scene + 'static {
        Self {
            current_scene: Box::new(initial_scene),
        }
    }

    pub fn init(&mut self, gfx: &mut Graphics) {
        gfx.begin_upload();
        self.current_scene.init(gfx);
        gfx.end_upload();
    }

    pub fn update(&mut self, gfx: &mut Graphics, dt: f64) {
        if let Some(new_scene) = self.current_scene.update(gfx, dt) {
            info!("Swapping Scene");
            gfx.reset();
            self.current_scene = new_scene;
            gfx.begin_upload();
            self.current_scene.init(gfx);
            gfx.end_upload();
        }
    }
}
