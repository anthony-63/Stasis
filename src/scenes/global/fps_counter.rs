use crate::core::gfx::{color::Color, objects::text::TextObject, Graphics, ObjectId};

pub struct FpsCounter {
    id: ObjectId,
    timer: f64,
}

impl FpsCounter {
    pub fn new(gfx: &mut Graphics) -> Self {
        Self {
            id: gfx.add_text(TextObject::new(
                10., 5.,
                "0 FPS".into(),
                32.,
                Color::from_rgb(0, 255, 0),
                "Assets/Game/font.ttf"
            )),
            timer: 0.
        }
    }

    pub fn update(&mut self, gfx: &mut Graphics, dt: f64) {
        self.timer += dt;
        if self.timer >= 0.5 {
            let text: &mut TextObject = &mut gfx.text_objects_mut()[self.id];
            text.text = (1./dt).ceil().to_string();
            text.update();
            self.timer = 0.;
        }
    }
}