use std::{io::{Cursor, Read, Seek, Write}, path::PathBuf};

use byteorder::{LittleEndian, ReadBytesExt};
use sonic_rs::{object, Object};
use tracing::{info, warn, error};
use super::beatmap::NoteData;

#[derive(Clone, Default)]
pub enum AudioType {
    #[default]
    None,
    Mp3,
    Wav,
    Ogg,
}

impl AudioType {
    fn compare_bytes(buffer: &Vec<u8>, index: u64, bytes: Vec<u8>) -> bool {
        for (i, b) in bytes.into_iter().enumerate() {
            if b != buffer[i + index as usize] {
                return false;
            }
        }
        true
    }

    pub fn get_type(buffer: &Vec<u8>) -> Self {
        if Self::compare_bytes(buffer, 0, vec![0x4f,0x67,0x67,0x53]) {
            return Self::Ogg;
        } else if Self::compare_bytes(buffer, 0, vec![0x52,0x49,0x46,0x46]) &&
                    Self::compare_bytes(buffer, 8, vec![0x57,0x41,0x56,0x45]) {
            return Self::Wav;
        } else if Self::compare_bytes(buffer, 0, vec![0xff,0xfb]) ||
                    Self::compare_bytes(buffer, 0, vec![0xff, 0xf3]) ||
                    Self::compare_bytes(buffer, 0, vec![0xff, 0xfa]) ||
                    Self::compare_bytes(buffer, 0, vec![0xff, 0xf2]) ||
                    Self::compare_bytes(buffer, 0, vec![0x49, 0x44, 0x33]) {
            return Self::Mp3;
        }

        Self::None
    }
}

struct DataOffsets {
    custom_offset: u64,
    marker_offset: u64,
    audio_offset: u64,
    audio_length: u64,
    cover_offset: u64,
    cover_length: u64
}

pub fn get_difficulty_title(v: usize) -> String {
    match v {
        0 => String::from("N/A"),
        1 => String::from("Easy"),
        2 => String::from("Medium"),
        3 => String::from("Hard"),
        4 => String::from("Logic?!"),
        5 => String::from("BRRR"),
        _ => {
            error!("Invalid difficulty level: {}", v);
            String::new()
        }
    }
}

enum BlockOffsets {
    Magic = 0x0,
    Version = 0x4,
    HasCover = 0x2e,
    // MapLength = 0x1E,
    NoteCount = 0x22,
    Difficulty = 0x2a,
    DataOffsets = 0x30,
    CoverOffset = 0x50,
    MarkerOffset = 0x70,
    IdOffset = 0x80,
}

pub struct SSPMParser {
    buffer: Cursor<Vec<u8>>,
}

impl SSPMParser {
    pub fn sspm_to_folder(path: &str, should_remove: bool) -> bool {
        let folder_name = PathBuf::from(path).with_extension("").file_name().unwrap().to_str().unwrap().to_string();

        let folder_path = format!("Assets/Maps/{}", folder_name);

        info!("parsing sspm: {} -> {}", path, folder_path);

        let file_bytes = std::fs::read(path).unwrap();
        let buffer = Cursor::new(file_bytes);
        
        let mut parser = Self {
            buffer,
        };

        if !parser.magic_exists() {
            warn!("SSPM File has invalid magic.\nmap: {}", path);
            return false;
        }

        if parser.get_version() != 2 {
            warn!("Only sspmv2 is supported.\nmap: {}\nversion: {}", path, parser.get_version());
            return false;
        }

        let offsets = parser.get_data_offsets();

        let title = parser.get_title();
        let mapper = parser.get_mapper();

        let note_count = parser.get_note_count();

        let difficulty_name = parser.get_difficulty_name(offsets.custom_offset);

        let before_notes = std::time::SystemTime::now();
        let notes = parser.get_notes(note_count, offsets.marker_offset);
        let elapsed_notes = std::time::SystemTime::now().duration_since(before_notes).unwrap();
        
        info!("loaded {} notes in {}ms", note_count, elapsed_notes.as_millis());

        let audio_buffer: Vec<u8> = parser.get_buffer(offsets.audio_offset, offsets.audio_length);
        
        let cover_buffer: Option<Vec<u8>> = if parser.has_cover() {
            Some(parser.get_buffer(offsets.cover_offset, offsets.cover_length))
        } else {
            None
        };

        let mut mappers: Vec<String> = vec![];
        for m in mapper.split(" & ").collect::<Vec<&str>>() {
            mappers.push(m.to_string());
        }

        match std::fs::create_dir(folder_path.clone()) {
            Ok(_) => {},
            Err(error) => error!("{}", error)
        }

        let meta = sonic_rs::to_string_pretty(&object! {
            "_version": 1,
            "_title": title,
            "_mappers": mappers,
            "_music": "music.bin",
            "_difficulties": [
                "sspm.json",
            ],
        }).unwrap();

        let mut notes_json: Vec<Object> = vec![];
        for note in notes {
            notes_json.push(object! {
                "_x": note.x,
                "_y": note.y,
                "_time": note.time,
            });
        }

        let diffname_new = if difficulty_name.is_empty() {
            parser.get_difficulty()
        } else {
            difficulty_name
        };

        let difficulty = sonic_rs::to_string_pretty(&object! {
            "_version": 1,
            "_name": diffname_new,
            "_notes": notes_json,
        }).unwrap();

        if cover_buffer.is_some() {
            let mut cover_file = match std::fs::File::create(format!("{}/cover.png", folder_path)) {
                Ok(file) => file,
                Err(error) => {
                    error!("{}", error);
                    return false;
                }
            };

            cover_file.write_all(&cover_buffer.unwrap()).unwrap();
            cover_file.flush().unwrap();
        }

        let mut meta_file = match std::fs::File::create(format!("{}/meta.json", folder_path)) {
            Ok(file) => file,
            Err(error) => {
                error!("{}", error);
                return false;
            }
        };
        let mut export_file = match std::fs::File::create(format!("{}/sspm.json", folder_path)) {
            Ok(file) => file,
            Err(error) => {
                error!("{}", error);
                return false;
            }
        };
        let mut music_file = match std::fs::File::create(format!("{}/music.bin", folder_path)) {
            Ok(file) => file,
            Err(error) => {
                error!("{}", error);
                return false;
            }
        };

        write!(meta_file, "{}", meta).unwrap();
        meta_file.flush().unwrap();

        write!(export_file, "{}", difficulty).unwrap();
        export_file.flush().unwrap();

        music_file.write_all(&audio_buffer).unwrap();
        music_file.flush().unwrap();

        if should_remove {
            std::fs::remove_file(path).unwrap();
        }

        true
    }

    fn magic_exists(&mut self) -> bool {
        self.buffer.seek(std::io::SeekFrom::Start(BlockOffsets::Magic as u64)).unwrap();

        let mut magic_buffer: [u8; 4] = [0; 4];
        self.buffer.read_exact(&mut magic_buffer).unwrap();

        magic_buffer == [b'S', b'S', b'+', b'm']
    }

    fn get_version(&mut self) -> usize {
        self.buffer.seek(std::io::SeekFrom::Start(BlockOffsets::Version as u64)).unwrap(); // right after magic
        self.buffer.read_u16::<LittleEndian>().unwrap() as usize
    }

    fn get_note_count(&mut self) -> usize {
        self.buffer.seek(std::io::SeekFrom::Start(BlockOffsets::NoteCount as u64)).unwrap(); // 4 bytes after map length
        self.buffer.read_u32::<LittleEndian>().unwrap() as usize
    }

    fn get_data_offsets(&mut self) -> DataOffsets {
        self.buffer.seek(std::io::SeekFrom::Start(BlockOffsets::DataOffsets as u64)).unwrap(); // start of data offsets

        let custom_offset = self.buffer.read_u64::<LittleEndian>().unwrap();
        self.buffer.seek(std::io::SeekFrom::Current(0x8)).unwrap(); // skip custom_data_byte_length, its uneeded

        let audio_offset = self.buffer.read_u64::<LittleEndian>().unwrap();
        let audio_length = self.buffer.read_u64::<LittleEndian>().unwrap();

        self.buffer.seek(std::io::SeekFrom::Start(BlockOffsets::MarkerOffset as u64)).unwrap(); // offset where marker offset is stored
        let marker_offset = self.buffer.read_u64::<LittleEndian>().unwrap();

        self.buffer.seek(std::io::SeekFrom::Start(BlockOffsets::CoverOffset as u64)).unwrap();
        let cover_offset = self.buffer.read_u64::<LittleEndian>().unwrap();
        let cover_length = self.buffer.read_u64::<LittleEndian>().unwrap();

        DataOffsets {
            audio_offset,
            audio_length,
            cover_offset,
            cover_length,
            custom_offset,
            marker_offset,
        }
    }

    // fn get_id(&mut self) -> String {
    //     self.buffer.seek(std::io::SeekFrom::Start(BlockOffsets::IdOffset as u64)).unwrap();
    //     return self.read_string();
    // }

    fn get_difficulty(&mut self) -> String {
        self.buffer.seek(std::io::SeekFrom::Start(BlockOffsets::Difficulty as u64)).unwrap();
        get_difficulty_title(self.buffer.read_u8().unwrap() as usize)
    }

    fn get_title(&mut self) -> String {
        self.buffer.seek(std::io::SeekFrom::Start(BlockOffsets::IdOffset as u64)).unwrap();
        let title_offset = BlockOffsets::IdOffset as u64 + self.buffer.read_u16::<LittleEndian>().unwrap() as u64 + 0x2; // add 2 due to the u16 not taken into account
        self.buffer.seek(std::io::SeekFrom::Start(title_offset)).unwrap();

        self.read_string()
    }

    fn has_cover(&mut self) -> bool {
        self.buffer.seek(std::io::SeekFrom::Start(BlockOffsets::HasCover as u64)).unwrap();
        self.buffer.read_u8().unwrap() == 1
    }

    fn get_mapper(&mut self) -> String {
        self.buffer.seek(std::io::SeekFrom::Start(BlockOffsets::IdOffset as u64)).unwrap();
        let title_offset = BlockOffsets::IdOffset as u64 + self.buffer.read_u16::<LittleEndian>().unwrap() as u64 + 0x2; // add 2 due to the u16 not taken into account
        self.buffer.seek(std::io::SeekFrom::Start(title_offset)).unwrap();
        let mapper_offset = title_offset + self.buffer.read_u16::<LittleEndian>().unwrap() as u64 + 0x2;
        self.buffer.seek(std::io::SeekFrom::Start(mapper_offset)).unwrap();
        _ = self.read_string(); // skip over song name.

        let mut mapper = String::new();

        for (i, _) in (0..self.buffer.read_u16::<LittleEndian>().unwrap()).enumerate() {
            if i != 0 {
                mapper.push_str(" & ");
            }

            mapper.push_str(&self.read_string());
        }

        mapper
    }

    fn get_difficulty_name(&mut self, offset: u64) -> String {
        self.buffer.seek(std::io::SeekFrom::Start(offset)).unwrap();
        let count = self.buffer.read_u16::<LittleEndian>().unwrap();

        if count == 0 {
            return String::new();
        }
        _ = self.read_string();
        let custom_data_type = self.buffer.read_u8().unwrap();

        if custom_data_type == 0x09 {
            self.read_string()
        } else if custom_data_type == 0x0b {
            return self.read_string_long();
        } else {
            info!("Why does this map not only have difficulty name?????? wtf this isnt real! got type {}", custom_data_type);
            return String::new()
        }
    }

    fn get_notes(&mut self, note_count: usize, offset: u64) -> Vec<NoteData> {
        let mut notes: Vec<NoteData> = vec![];

        self.buffer.seek(std::io::SeekFrom::Start(offset)).unwrap();
        for _ in 0..note_count {
            let mut note = NoteData::default();

            note.time = self.buffer.read_u32::<LittleEndian>().unwrap() as f32 / 1000.;

            _ = self.buffer.read_u8().unwrap(); // always 1? stupid lol
            
            let has_quantum = self.buffer.read_u8().unwrap() == 1;
            if has_quantum {
                note.x = -(self.buffer.read_f32::<LittleEndian>().unwrap() - 1.);
                note.y = -(self.buffer.read_f32::<LittleEndian>().unwrap() - 1.);
            } else {
                note.x = -(self.buffer.read_u8().unwrap() as f32 - 1.);
                note.y = -(self.buffer.read_u8().unwrap() as f32 - 1.);
            }

            notes.push(note);
        }
        
        notes.sort_by(|a, b| {
            a.time.partial_cmp(&b.time).unwrap()
        });

        notes
    }

    fn get_buffer(&mut self, offset: u64, length: u64) -> Vec<u8> {
        self.buffer.seek(std::io::SeekFrom::Start(offset)).unwrap();
        let mut v= vec![0; length as usize];
        let buffer = v.as_mut_slice();
        for _ in 0..length {
            _ = self.buffer.read_exact(buffer);
        }

        buffer.to_vec()
    }

    fn read_string(&mut self) -> String {
        let len = self.buffer.read_u16::<LittleEndian>().unwrap();
        let mut str_buffer = vec![0; len as usize];
        self.buffer.read_exact(&mut str_buffer).unwrap();
        String::from_utf8(str_buffer).unwrap()
    }

    fn read_string_long(&mut self) -> String {
        let len = self.buffer.read_u32::<LittleEndian>().unwrap();
        let mut str_buffer = vec![0; len as usize];
        self.buffer.read_exact(&mut str_buffer).unwrap();
        String::from_utf8(str_buffer).unwrap()
    }
}