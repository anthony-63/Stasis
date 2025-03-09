use tracing::info;

use crate::core::{gfx::{objects::sprite3d::Sprite3dObject, Graphics, ObjectId}, Vector2};

pub struct Cursor {
    id: ObjectId,
    
    scale: f32,
    sensitivity: f32,

    locked_to_grid: bool,

    pub pos: Vector2,
}

impl Cursor {
    pub fn new(gfx: &mut Graphics, scale: f32) -> Self {
        Self {
            id: gfx.add_sprite(Sprite3dObject::new_plane(-scale / 2., -scale / 2., 0., scale, scale, "Assets/Game/Cursor.png")),
            pos: Vector2::zero(),
            sensitivity: 0.017,
            locked_to_grid: true,
            scale,
        }
    }

    pub fn update(&mut self, gfx: &mut Graphics, delta: Vector2) {
        let cursor = &mut gfx.sprite3ds_mut()[self.id];
        
        cursor.x += delta.x * self.sensitivity;
        cursor.y -= delta.y * self.sensitivity;
        
        if self.locked_to_grid {
            cursor.x = cursor.x.clamp(-3., 3. - self.scale);
            cursor.y = cursor.y.clamp(-3., 3. - self.scale);
        }

        cursor.update();


    }
}