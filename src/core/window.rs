use sdl3::event::Event;

pub struct Window {
    window: sdl3::video::Window,
    context: sdl3::Sdl,
    video: sdl3::VideoSubsystem,
}

impl Window {
    pub fn new(title: &str, width: u32, height: u32) -> Self {
        let context = sdl3::init().expect("Failed to initialize SDL3");
        let video = context.video().expect("Failed to initialize SDL3 Video");

        return Self {
            context,
            window: video.window(title, width, height).build().expect("Failed to create SDL3 Window"),
            video,
        }
    }

    pub fn run(&mut self) {
        let mut canvas = self.window.clone().into_canvas();
        let mut event_pump = self.context.event_pump().map_err(|e| e.to_string()).unwrap();

        'running: loop {
            for ev in event_pump.poll_iter() {
                match ev {
                    Event::Quit { .. } => break 'running,
                    _ => {},
                }
            }

        }
    }
}