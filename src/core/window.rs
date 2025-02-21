use sdl3::event::Event;

use super::{gfx::Graphics, scene::{Scene, SceneSwapper}};

pub struct Window {
    gfx: Graphics,
    context: sdl3::Sdl,
    video: sdl3::VideoSubsystem,
    swapper: SceneSwapper,
}

impl Window {
    pub fn new<T>(title: &str, width: u32, height: u32, initial_scene: T) -> Self
    where T: Scene + 'static {
        let context = sdl3::init().expect("Failed to initialize SDL3");
        let video = context.video().expect("Failed to initialize SDL3 Video");
        let window = video.window(title, width, height).build().expect("Failed to create SDL3 Window");
        let gfx = Graphics::new(window);
        
        return Self {
            context,
            gfx,
            video,
            swapper: SceneSwapper::new(initial_scene),
        }
    }

    pub fn run(&mut self) {
        let mut event_pump = self.context.event_pump().map_err(|e| e.to_string()).unwrap();

        'running: loop {
            for ev in event_pump.poll_iter() {
                match ev {
                    Event::Quit { .. } => break 'running,
                    _ => {},
                }
            }
            self.swapper.update(0.);
            self.swapper.render(&mut self.gfx);
            self.gfx.render();
        }
    }
}