
use tracing::info;

use crate::core::scene::Scene;

pub struct LoadingScene {

}

impl LoadingScene {
    pub fn new() -> Self {
        Self {}
    }
}

impl Scene for LoadingScene {
    fn update(&mut self, _dt: f64) -> Option<Box<dyn Scene + 'static>> {
        None
    }

    fn render(&mut self) {
        
    }
}