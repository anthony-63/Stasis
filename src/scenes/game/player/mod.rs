use cursor::Cursor;
use grid::Grid;

use crate::{content::settings::{CameraSettings, CursorSettings}, core::{gfx::{objects::camera::CameraObject, Graphics, ObjectId}, input::Input, Vector3}};

mod cursor;
mod grid;

pub struct Player {
    grid: Grid,
    cursor: Cursor,
}

impl Player {
    pub fn new(gfx: &mut Graphics, cursor_settings: &CursorSettings, camera_settings: &CameraSettings) -> Self {
        let camera = CameraObject::new(70.0, Vector3::new(0., 0., 7.), Vector3::zero());
        gfx.bind_camera(camera);
        Self {
            cursor: Cursor::new(gfx, cursor_settings, camera_settings),
            grid: Grid::new(gfx, camera_settings)
        }
    }

    pub fn update(&mut self, gfx: &mut Graphics, input: &mut Input) {
        self.cursor.update(gfx, input.mouse_delta);
        self.cursor.apply_parallax(gfx, &mut self.grid);
    }
}