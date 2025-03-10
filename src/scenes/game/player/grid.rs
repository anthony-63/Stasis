use crate::{content::settings::CameraSettings, core::{gfx::{objects::sprite3d::Sprite3dObject, Graphics, ObjectId}, Vector2}};

pub struct Grid {
    id: ObjectId,

    parallax: f32,
    pub pos: Vector2,
}

impl Grid {
    pub fn new(gfx: &mut Graphics, settings: &CameraSettings) -> Self {
        Self {
            id: gfx.add_sprite(Sprite3dObject::new_plane(-3., -3., 0., 6., 6., "Assets/Game/Grid.png")),
            parallax: settings.grid_parallax,
            pos: Vector2::zero(),
        }
    }

    pub fn apply_parallax(&self, gfx: &mut Graphics, cursor_pos: Vector2) {
        let grid = &mut gfx.sprite3ds_mut()[self.id];
        grid.x = (-cursor_pos.x * self.parallax / 50.) - grid.w / 2.;
        grid.y = (-cursor_pos.y * self.parallax / 50.) - grid.h / 2.;
        gfx.sprite3ds_mut()[self.id].update();
    }
}