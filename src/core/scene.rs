use tracing::info;

use super::gfx::Graphics;

pub trait Scene {
    fn update(&mut self, dt: f64) -> Option<Box<dyn Scene + 'static>>;
    fn render(&mut self, gfx: &mut Graphics);
}

pub struct SceneSwapper {
    pub current_scene: Box<dyn Scene + 'static>,
}

impl SceneSwapper {
    pub fn new<T>(initial_scene: T) -> Self
    where
        T: Scene + 'static,
    {
        Self {
            current_scene: Box::new(initial_scene),
        }
    }

    pub fn update(&mut self, dt: f64) {
        self.current_scene.update(dt);

        if let Some(new_scene) = self.current_scene.update(dt) {
            info!("Swapping Scene");
            self.current_scene = new_scene;
        }
    }

    pub fn render(&mut self, gfx: &mut Graphics) {
        self.current_scene.render(gfx);
    }
}
