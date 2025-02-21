
use tracing::info;

use crate::core::state::State;

pub struct LoadingScene {

}

impl LoadingScene {
    pub fn new() -> Self {
        Self {}
    }
}

impl State for LoadingScene {
    fn update(&mut self, _dt: f64) -> Option<Box<dyn State + 'static>> {
        None
    }

    fn render(&mut self) {
        
    }
}