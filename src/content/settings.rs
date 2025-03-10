use std::path::Path;

use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize, Clone)]
pub struct NoteSettings {
    #[serde(default)]
    pub approach_time: f32,
    #[serde(default)]
    pub approach_distance: f32,
    #[serde(default)]
    pub pushback: bool,
    #[serde(default)]
    pub fade_in: f32,
    #[serde(default)]
    pub half_ghost: bool,
    #[serde(default)]
    pub colors: Vec<u32>,
}

impl Default for NoteSettings {
    fn default() -> Self {
        Self { 
            approach_time: 0.39,
            approach_distance: 14.,
            fade_in: 0.0,
            colors: vec![0xff0000, 0x00ff00, 0x0000ff],
            half_ghost: false,
            pushback: false,
        }
    }
}

#[derive(Serialize, Deserialize, Clone)]
pub struct CursorSettings {
    #[serde(default)]
    pub clamped: bool,
    #[serde(default)]
    pub sensitivity: f32,
    #[serde(default)]
    pub scale: f32,
}

impl Default for CursorSettings {
    fn default() -> Self {
        Self { 
            clamped: true,
            scale: 0.5,
            sensitivity: 17.,
        }
    }
}

#[derive(Serialize, Deserialize, Clone)]
pub struct AudioSettings {
    #[serde(default)]
    pub volume: f32,
    #[serde(default)]
    pub fxvolume: f32
}

impl Default for AudioSettings {
    fn default() -> Self {
        Self { 
            volume: 0.1,
            fxvolume: 0.1,
        }
    }
}

#[derive(Serialize, Deserialize, Clone)]
pub struct CameraSettings {
    #[serde(default)]
    pub fov: f32,
    #[serde(default)]
    pub camera_parallax: f32,
    #[serde(default)]
    pub grid_parallax: f32,
}

impl Default for CameraSettings {
    fn default() -> Self {
        Self { 
            fov: 70.,
            camera_parallax: 10.,
            grid_parallax: 0.
        }
    }
}

#[derive(Serialize, Deserialize, Clone)]
pub struct MiscSettings {
    #[serde(default)]
    pub enable_replays: bool,
}

impl Default for MiscSettings {
    fn default() -> Self {
        Self { 
            enable_replays: false,
        }
    }
}

#[derive(Default, Clone)]
#[derive(Serialize, Deserialize)]
pub struct Settings {
    #[serde(default)]
    pub note: NoteSettings,
    #[serde(default)]
    pub cursor: CursorSettings,
    #[serde(default)]
    pub camera: CameraSettings,
    #[serde(default)]
    pub audio: AudioSettings,
    #[serde(default)]
    pub misc: MiscSettings,
}

impl Settings {
    pub fn load<P: AsRef<Path>>(path: P) -> Self {
        toml::from_str(&std::fs::read_to_string(path).unwrap()).unwrap()
    }

    pub fn save<P: AsRef<Path>>(&self, path: P) {
        std::fs::write(path, toml::to_string_pretty(self).unwrap()).unwrap();
    }
}