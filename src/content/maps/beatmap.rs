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
        let data_json = std::fs::read_to_string(path.clone()).unwrap_or_else(|_| panic!("data json not found somehow? '{}'", path));
        let data: Value = sonic_rs::from_str(&data_json).unwrap();

        let version = data["_version"].as_number().expect("expected number for version").as_u64().unwrap() as u8;
        let name = data["_name"].to_string();

        let mut notes: Vec<NoteData> = vec![];
        for note in data["_notes"].clone().into_array().unwrap() {
            notes.push(NoteData {
                x: note["_x"].as_number().unwrap().as_f64().unwrap() as f32,
                y: note["_y"].as_number().unwrap().as_f64().unwrap() as f32,
                time: note["_time"].as_number().unwrap().as_f64().unwrap() as f32,
            });
        }

        notes.sort_by(|a, b| a.time.partial_cmp(&b.time).unwrap());

        Self {
            parsed: true,
            version,
            path,
            name,
            notes,
            ..Default::default()
        }
    }

    fn empty() -> Self {
        Self {
            parsed: false,
            ..Default::default()
        }
    }
}