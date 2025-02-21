mod core;
use core::window::*;

fn main() {
    let mut window = Window::new("Stasis", 1280, 720);
    window.run();
}
