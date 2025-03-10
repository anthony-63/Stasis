use tracing::info;

use crate::{content::settings::{CameraSettings, CursorSettings}, core::{gfx::{objects::sprite3d::Sprite3dObject, Graphics, ObjectId}, Vector2}};

use super::grid::Grid;

pub struct Cursor {
    id: ObjectId,
    
    scale: f32,
    sensitivity: f32,

    locked_to_grid: bool,

    camera_parallax: f32,
    grid_parallax: f32,

    pub pos: Vector2,
}

const CLAMP: f32 =  (6.0 - 0.525) / 2.0;

impl Cursor {
    pub fn new(gfx: &mut Graphics, settings: &CursorSettings, camera_settings: &CameraSettings) -> Self {
        Self {
            id: gfx.add_sprite(Sprite3dObject::new_plane(-settings.scale / 2., -settings.scale / 2., 0., settings.scale, settings.scale, "Assets/Game/Cursor.png")),
            pos: Vector2::zero(),
            sensitivity: settings.sensitivity / 100.0,
            locked_to_grid: settings.clamped,
            camera_parallax: camera_settings.camera_parallax / 50.0,
            grid_parallax: camera_settings.grid_parallax / 50.0,
            scale: settings.scale,
        }
    }

    pub fn update(&mut self, gfx: &mut Graphics, delta: Vector2) {
        let cursor = &mut gfx.sprite3ds_mut()[self.id];
        
        cursor.x += delta.x * self.sensitivity;
        cursor.y -= delta.y * self.sensitivity;
        

        if self.locked_to_grid {
            cursor.x = cursor.x.clamp(-(CLAMP + (self.pos.x * self.grid_parallax)) - self.scale / 2., CLAMP - (self.pos.x * self.grid_parallax) - self.scale / 2.);
            cursor.y = cursor.y.clamp(-(CLAMP + (self.pos.y * self.grid_parallax)) - self.scale / 2., CLAMP - (self.pos.y * self.grid_parallax) - self.scale / 2.);
        }

        self.pos = Vector2::new(cursor.x - self.scale / 2., cursor.y - self.scale / 2.);

        cursor.update();
    }

    pub fn apply_parallax(&self, gfx: &mut Graphics, grid: &mut Grid) {
        let camera = gfx.bound_camera_mut();
        camera.position.x = -self.pos.x * (self.camera_parallax);
        camera.position.y = -self.pos.y * (self.camera_parallax);

        grid.apply_parallax(gfx, self.pos);

    }
}