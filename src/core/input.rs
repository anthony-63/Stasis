use sdl3::{event::Event, mouse::MouseUtil, video::Window};

use super::Vector2;

pub struct Input {
    pub mouse_position: Vector2,
    pub mouse_delta: Vector2,

    pub cursor_locked: bool,

    mouse: MouseUtil,
}

impl Input {
    pub fn new(mouse: MouseUtil) -> Self {
        Self {
            mouse_delta: Vector2::zero(),
            mouse_position: Vector2::zero(),

            cursor_locked: false,
            mouse,
        }
    }

    pub fn update(&mut self, window: &Window, ev: Event) {
        match ev {
            Event::MouseMotion { x, y, xrel, yrel, .. } => {
                self.mouse_position = Vector2::new(x, y);
                self.mouse_delta = Vector2::new(xrel, yrel);
            }
            Event::Unknown { .. } => {
                self.mouse_delta = Vector2::new(0., 0.);
                if self.cursor_locked && window.has_mouse_focus() && !self.mouse.relative_mouse_mode(window) {
                    self.mouse.set_relative_mouse_mode(window, true);
                } else if !self.cursor_locked && self.mouse.relative_mouse_mode(window) {
                    self.mouse.set_relative_mouse_mode(window, false);
                }
            }
            _ => {
            },
        }
    }

    pub fn lock_cursor(&mut self) {
        self.cursor_locked = true;
    }

    pub fn unlock_cursor(&mut self) {
        self.cursor_locked = false;
    }
}