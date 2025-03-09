use cursor::Cursor;
use grid::Grid;

use crate::core::{gfx::Graphics, input::Input};

mod cursor;
mod grid;

pub struct Player {
    grid: Grid,
    cursor: Cursor,
}

impl Player {
    pub fn new(gfx: &mut Graphics, cursor_scale: f32) -> Self {
        Self {
            cursor: Cursor::new(gfx, cursor_scale),
            grid: Grid::new(gfx)
        }
    }

    pub fn update(&mut self, gfx: &mut Graphics, input: &mut Input) {
        self.cursor.update(gfx, input.mouse_delta);
    }
}