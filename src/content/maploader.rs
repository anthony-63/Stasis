
use tracing::info;

use crate::content::maps::{beatmapset::BeatmapSet, sspm::SSPMParser};

pub struct MapLoader;

impl MapLoader {
    pub fn load_all_from_dir(path: String)  -> Vec<BeatmapSet> {
        info!("loading all maps from {}", path);

        let mut maps = vec![];

        // let map_count = std::fs::read_dir(path.clone()).unwrap().count();
        let map_folders = std::fs::read_dir(path).unwrap();

        for filename in map_folders.into_iter() {
            let file: std::fs::DirEntry = filename.unwrap();

            if file.path().is_dir() {
                maps.push(BeatmapSet::from_folder(file.path().to_str().unwrap().to_string(), false));
            } else if file.path().to_string_lossy().ends_with(".sspm") {
                let parsed = SSPMParser::sspm_to_folder(file.path().to_str().unwrap(), true);

                if parsed {
                    maps.push(BeatmapSet::from_folder(file.path().with_extension("").to_str().unwrap().to_string(), false));
                }
                continue
            }
        }
        maps
    }
}