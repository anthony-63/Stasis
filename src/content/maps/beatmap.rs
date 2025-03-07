use sonic_rs::{JsonNumberTrait, JsonValueTrait, Value};

#[derive(Default, Clone, Debug)]
pub struct NoteData {
    pub x: f32,
    pub y: f32,
    pub time: f32,
}

#[derive(Clone, Debug, Default)]
pub struct Beatmap {
    pub parsed: bool,
    pub version: u8,
    pub path: String,
    pub name: String,
    pub notes: Vec<NoteData>,
    pub id: String,
}

impl Beatmap {
    pub fn from_file(path: String) -> Self {
        let mut map = Self::empty(path);
        map.load();
        map
    }

    pub fn load(&mut self) {
        if self.parsed {
            return;
        }
        
        let data_json = std::fs::read_to_string(self.path.clone()).unwrap_or_else(|_| panic!("data json not found somehow? '{}'", self.path));
        let data: Value = sonic_rs::from_str(&data_json).unwrap();

        self.version = data["_version"].as_number().expect("expected number for version").as_u64().unwrap() as u8;
        self.name = data["_name"].to_string();

        self.notes = vec![];
        for note in data["_notes"].clone().into_array().unwrap() {
            self.notes.push(NoteData {
                x: note["_x"].as_number().unwrap().as_f64().unwrap() as f32,
                y: note["_y"].as_number().unwrap().as_f64().unwrap() as f32,
                time: note["_time"].as_number().unwrap().as_f64().unwrap() as f32,
            });
        }

        self.notes.sort_by(|a, b| a.time.partial_cmp(&b.time).unwrap());
        self.parsed = true;
    }

    pub fn empty(path: String) -> Self {
        Self {
            parsed: false,
            path,
            ..Default::default()
        }
    }
}