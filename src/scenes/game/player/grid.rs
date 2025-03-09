use crate::core::{gfx::{objects::sprite3d::Sprite3dObject, Graphics, ObjectId}, Vector2};

pub struct Grid {
    id: ObjectId,

    pub pos: Vector2,
}

impl Grid {
    pub fn new(gfx: &mut Graphics) -> Self {
        Self {
            id: gfx.add_sprite(Sprite3dObject::new_plane(-3., -3., 0., 6., 6., "Assets/Game/Grid.png")),
            pos: Vector2::zero(),
        }
    }
}