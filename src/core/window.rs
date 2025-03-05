use std::time::SystemTime;

use sdl3::event::Event;

use super::{gfx::Graphics, scene::{Scene, SceneSwapper}};

pub struct Window {
    gfx: Graphics,
    context: sdl3::Sdl,
    swapper: SceneSwapper,
}

impl Window {
    pub fn new<T>(title: &str, width: u32, height: u32, initial_scene: T) -> Self
    where T: Scene + 'static {
        sdl3::hint::set(sdl3::hint::names::RENDER_VSYNC, "0");
        sdl3::hint::set(sdl3::hint::names::RENDER_GPU_DEBUG, "0");
        sdl3::hint::set(sdl3::hint::names::RENDER_VULKAN_DEBUG, "0");
        let context = sdl3::init().expect("Failed to initialize SDL3");
        let video = context.video().expect("Failed to initialize SDL3 Video");
        let window = video.window(title, width, height).fullscreen().build().expect("Failed to create SDL3 Window");
        let gfx = Graphics::new(window);
        
        Self {
            context,
            gfx,
            swapper: SceneSwapper::new(initial_scene),
        }
    }

    pub fn run(&mut self) {
        let mut event_pump = self.context.event_pump().map_err(|e| e.to_string()).unwrap();

        self.swapper.init(&mut self.gfx);

        let mut current_time =  SystemTime::now();
        let mut last_time: SystemTime;

        'running: loop {
            last_time = current_time;
            current_time = SystemTime::now();
            let dt = current_time.duration_since(last_time).unwrap().as_secs_f64();

            for ev in event_pump.poll_iter() {
                if let Event::Quit { .. } = ev { break 'running }
                if let Event::Window { win_event, .. } = ev {
                    if let sdl3::event::WindowEvent::Resized(w, h) = win_event { self.gfx.resize(w, h) }
                } 
            }

            self.gfx.update();
            self.swapper.update(&mut self.gfx, dt);
            self.gfx.render();
        }
    }
}