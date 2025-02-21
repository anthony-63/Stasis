pub trait State {
    fn update(&mut self, dt: f64) -> Option<Box<dyn State + 'static>>;
    fn render(&mut self);
}

pub struct StateSwapper {
    pub current_state: Box<dyn State + 'static>,
}

impl StateSwapper {
    pub fn new<T>(initial_state: T) -> Self
    where
        T: State + 'static,
    {
        Self {
            current_state: Box::new(initial_state),
        }
    }

    pub fn update(&mut self, dt: f64) {
        self.current_state.update(dt);

        if let Some(new_state) = self.current_state.update(dt) {
            self.current_state = new_state;
        }
    }

    pub fn render(&mut self) {
        self.current_state.render();
    }
}
