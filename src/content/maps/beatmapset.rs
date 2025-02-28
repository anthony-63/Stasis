use std::{hash::Hasher, path::Path};

use gxhash::GxHasher;
use tracing::{error, info, warn};

use super::{beatmap::Beatmap, sspm::AudioType};

const STAGE2_MAP_SEED: i64 = 1775906128188434359;

#[derive(Default, Clone, Debug)]
pub struct BeatmapSet {
    pub broken: bool,

    pub loaded: bool,

    pub version: u8,
    pub hash: String,
    pub path: String,

    pub artist: String,
    pub title: String,
    
    pub difficulties: Vec<Beatmap>,
    pub mappers: Vec<String>,

    pub music_path: String,

    pub cover: Option<Vec<u8>>,
}

impl BeatmapSet {
    pub fn from_folder(folder_path: String) -> Self {
        let meta_path = &format!("{}/meta.json", folder_path);

        if !Path::new(meta_path).exists() {
            warn!("meta.json not found for {}", folder_path);
            return Self {
                broken: true,
                ..Default::default()
            };
        }

        let meta_json = std::fs::read_to_string(meta_path).expect("meta json not found somehow?");
        let meta = json::parse(&meta_json).unwrap();

        let version = meta["_version"].as_u8().expect("version must be a number");
        let title = meta["_title"].to_string();

        let mut mappers: Vec<String> = vec![];
        for mapper in meta["_mappers"].members() {
            mappers.push(mapper.to_string());
        }
        
        let mut difficulties: Vec<Beatmap> = vec![];
        for difficulty in meta["_difficulties"].members() {
            difficulties.push(Beatmap::from_file(format!("{}/{}", folder_path, difficulty.to_string())));
        }

        let music_path = meta["_music"].to_string();

        let cover = if Path::new(&format!("{}/cover.png", folder_path)).exists() {
            Some(std::fs::read(format!("{}/cover.png", folder_path)).unwrap())
        } else {
            None
        };

        let mut hasher = GxHasher::with_seed(STAGE2_MAP_SEED);

        hasher.write(&[version]);
        hasher.write(meta_json.as_bytes());
        hasher.write(folder_path.as_bytes());

        difficulties.clone().into_iter().for_each(|diff| {
            hasher.write(diff.name.as_bytes());
            hasher.write(diff.path.as_bytes());
        });

        let hash = hasher.finish();
        
        difficulties.as_mut_slice().into_iter().for_each(|diff| {
            diff.id = hash.to_string() + "/" + &diff.name;
        });
        
        Self {
            broken: false,
            version,
            title,
            mappers,
            music_path,
            loaded: true,
            difficulties,
            path: folder_path,
            hash: hash.to_string(),
            cover,
            ..Default::default()
        }
    }
}


/*
func get_ogg_packet_sequence(data:PackedByteArray):
	var packets: Array = []
	var granule_positions: Array = []
	var sampling_rate: int = 0
	var pos: int  = 0
	while pos < data.size():
		var header = data.slice(pos, pos + 27)
		pos += 27
		if header.slice(0, 4) != "OggS".to_ascii_buffer():
			break

		var packet_type = header.decode_u8(5)
		var granule_position = header.decode_u64(6)

		granule_positions.append(granule_position)

		var segment_table_length = header.decode_u8(26)

		var segment_table = data.slice(pos, pos + segment_table_length)
		pos += segment_table_length

		var packet_data = []
		var appending = false
		for i in range(segment_table_length):
			var segment_size = segment_table.decode_u8(i)
			var segment = data.slice(pos, pos + segment_size)
			if appending: packet_data.back().append_array(segment)
			else: packet_data.append(segment)
			appending = segment_size == 255
			pos += segment_size

		packets.append(packet_data)
		if sampling_rate == 0 and packet_type == 2:
			var info_header = packet_data[0]
			if info_header.slice(1, 7).get_string_from_ascii() != "vorbis":
				break
			sampling_rate = info_header.decode_u32(12)
	var packet_sequence = OggPacketSequence.new()
	packet_sequence.sampling_rate = sampling_rate
	packet_sequence.granule_positions = granule_positions
	packet_sequence.packet_data = packets
	return packet_sequence

*/