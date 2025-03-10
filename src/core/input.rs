use sdl3::{event::Event, keyboard::Keycode, mouse::MouseUtil, video::Window};

use super::Vector2;

pub type Key = Keycode;

pub struct Input {
    pub mouse_position: Vector2,
    pub mouse_delta: Vector2,

    pub cursor_locked: bool,

    mouse: MouseUtil,
    keys: Vec<(Key, bool)>
}

impl Input {
    pub fn new(mouse: MouseUtil) -> Self {
        Self {
            mouse_delta: Vector2::zero(),
            mouse_position: Vector2::zero(),

            cursor_locked: false,
            mouse,
            keys: vec![],
        }
    }

    pub fn update(&mut self, window: &Window, ev: Event) {
        match ev {
            Event::MouseMotion { x, y, xrel, yrel, .. } => {
                self.mouse_position = Vector2::new(x, y);
                self.mouse_delta = Vector2::new(xrel, yrel);
            }
            Event::KeyDown { keycode, repeat, .. } => {
                if let Some(code) = keycode {
                    if !repeat {
                        self.keys.push((code, true))
                    }
                }
            }
            Event::KeyUp { keycode, ..} => {
                if let Some(code) = keycode {
                    self.keys.retain(|k| k.0 != code);
                }
            }
            Event::Unknown { .. } => {
                self.mouse_delta = Vector2::new(0., 0.);
                if self.cursor_locked && window.has_mouse_focus() && !self.mouse.relative_mouse_mode(window) {
                    self.mouse.set_relative_mouse_mode(window, true);
                } else if !self.cursor_locked && self.mouse.relative_mouse_mode(window) {
                    self.mouse.set_relative_mouse_mode(window, false);
                }
                for key in self.keys.iter_mut() {
                    key.1 = false;
                }
            }
            _ => {
            },
        }
        
    }

    pub fn key_down(&mut self, key: Key) -> bool {
        self.keys.iter().any(|k| k.0 == key)
    }

    pub fn key_up(&mut self, key: Key) -> bool {
        !self.keys.iter().any(|k| k.0 == key)
    }

    pub fn keys_pressed(&mut self, keys: Vec<Key>) -> bool {
        keys.iter().all(|key|
            self.keys.iter().any(|k| k.0 == *key)) &&
        keys.iter().any(|key|
            self.keys.iter().any(|k| k.0 == *key && k.1))
    }

    pub fn key_pressed(&mut self, key: Key) -> bool {
        self.keys.contains(&(key, true))
    }

    pub fn lock_cursor(&mut self) {
        self.cursor_locked = true;
    }

    pub fn unlock_cursor(&mut self) {
        self.cursor_locked = false;
    }

    pub fn toggle_lock_cursor(&mut self) {
        self.cursor_locked = !self.cursor_locked;
    }
}