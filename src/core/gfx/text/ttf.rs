use std::{collections::HashMap, io::{Cursor, Read, Seek}, path::PathBuf};

use byteorder::{BigEndian, ReadBytesExt};
use tracing::{error, info};

use crate::core::Vector2;

pub struct TtfFont {
    pub glyphs: Vec<GlyphInfo>
}

#[derive(Debug, Copy, Clone, Default)]
pub struct GlyphPoint {
    pub p: Vector2,
    pub on_curve: bool
}


#[derive(Debug, Default, Clone)]
pub struct GlyphInfo {
    pub points: Vec<GlyphPoint>,
    pub end_indices: Vec<u16>,
    pub unicode_value: char,
    pub advance_width: u32,
    pub left_side_bearing: u32,

    pub min_x: u16,
    pub max_x: u16,
    pub min_y: u16,
    pub max_y: u16,
}

#[derive(Debug)]
pub struct GlyphMap {
    pub index: usize,
    pub unicode: char,
}

impl GlyphMap {
    pub fn new(index: usize, unicode: char) -> Self {
        Self {
            index, unicode
        }
    }
}

impl TtfFont {
    pub fn load(path: PathBuf) -> Self {
        info!("loading ttf font: {}", path.to_str().unwrap());
        let file_bytes = std::fs::read(path).unwrap();
        let mut buffer = Cursor::new(file_bytes);

        _ = buffer.read_u32::<BigEndian>();

        info!("loading offset table");
        let offset_table = Self::load_offset_table(&mut buffer);

        info!("loading header");
        buffer.set_position(offset_table["head"] as u64);

        _ = buffer.seek(std::io::SeekFrom::Current(18));
        let _units_per_em = buffer.read_u16::<BigEndian>().unwrap();

        _ = buffer.seek(std::io::SeekFrom::Current(30));
        let bytes_per_loc: usize = if buffer.read_u16::<BigEndian>().unwrap() == 0 { 2 } else { 4 };

        info!("loading maxp");
        buffer.set_position(offset_table["maxp"] as u64);
        _ = buffer.read_u32::<BigEndian>();

        let glyph_len = buffer.read_u16::<BigEndian>().unwrap();

        info!("loading glyph locations");
        let locations = Self::load_glyph_locations(&mut buffer, glyph_len as usize, bytes_per_loc, offset_table["loca"], offset_table["glyf"]);
        
        info!("loading cmap");
        let cmap = Self::load_cmap(&mut buffer, offset_table["cmap"]);
        
        info!("loading ghlyhs(finally)");
        let glyphs = GlyphInfo::load_all(&mut buffer, cmap, locations);

        Self {
            glyphs,
        }
    }

    fn load_cmap(buffer: &mut Cursor<Vec<u8>>, cmap_offset: usize) -> Vec<GlyphMap> {
        buffer.set_position(cmap_offset as u64);

        let mut map = vec![];

        let _version = buffer.read_u16::<BigEndian>().unwrap();
        let subtable_len = buffer.read_u16::<BigEndian>().unwrap();

        let mut cmap_subtable_offset = 0;
        let mut unicode_version_id: i32 = -1;

        for _ in 0..subtable_len {
            let plat_id = buffer.read_u16::<BigEndian>().unwrap();
            let plat_spec_id = buffer.read_u16::<BigEndian>().unwrap();
            let offset = buffer.read_u32::<BigEndian>().unwrap();

            if plat_id == 0 {
                if plat_spec_id <= 4 && plat_spec_id as i32 > unicode_version_id {
                    cmap_subtable_offset = offset;
                    unicode_version_id = plat_spec_id as i32;
                }
            } else if plat_id == 3 && unicode_version_id == -1 && (plat_spec_id == 1 || plat_spec_id == 10) {
                cmap_subtable_offset = offset;
            }
        }

        if cmap_subtable_offset == 0 {
            error!("Font doesnt contain supported cmap");
        }

        buffer.set_position(cmap_offset as u64 + cmap_subtable_offset as u64);
        let format = buffer.read_u16::<BigEndian>().unwrap();
        let mut has_missing_char_glyph = false;

        if format == 4 {
            let _len = buffer.read_u16::<BigEndian>().unwrap();
            let _lang_code = buffer.read_u16::<BigEndian>().unwrap();

            let seg_count_2x = buffer.read_u16::<BigEndian>().unwrap();
            let seg_count = seg_count_2x / 2;
            _ = buffer.read_u48::<BigEndian>().unwrap();

            let mut end_codes = vec![];
            for _ in 0..seg_count {
                end_codes.push(buffer.read_u16::<BigEndian>().unwrap());
            }

            _ = buffer.read_u16::<BigEndian>().unwrap();

            let mut start_codes = vec![];
            for _ in 0..seg_count {
                start_codes.push(buffer.read_u16::<BigEndian>().unwrap());
            }

            let mut id_deltas = vec![];
            for _ in 0..seg_count {
                id_deltas.push(buffer.read_u16::<BigEndian>().unwrap());
            }

            let mut id_range_offsets = Vec::<(u64, u16)>::new();
            for _ in 0..seg_count {
                id_range_offsets.push((buffer.position(), buffer.read_u16::<BigEndian>().unwrap()));
            }

            for i in 0..start_codes.len() {
                let end_code = end_codes[i];
                let mut start_code = start_codes[i];

                if start_code == 65535 {
                    break;
                }

                while start_code <= end_code {
                    let mut glyph_index;

                    if id_range_offsets[i].0 == 0 {
                        glyph_index = (start_code + id_deltas[i]) % 65535;
                    } else {
                        let loc_old = buffer.position();
                        let offs_loc = id_range_offsets[i].1 as u64 + id_range_offsets[i].0;
                        let arr_idx_loc = 2 * (start_code as u64 - start_codes[i] as u64) + offs_loc;

                        buffer.set_position(arr_idx_loc);
                        glyph_index = buffer.read_u16::<BigEndian>().unwrap();

                        if glyph_index != 0 {
                            glyph_index = ((glyph_index as u32 + id_deltas[i] as u32) % 65535) as u16;
                        }

                        buffer.set_position(loc_old);
                    }
                    map.push(GlyphMap::new(glyph_index as usize, char::from_u32(start_code as u32).unwrap()));
                    has_missing_char_glyph |= glyph_index == 0;
                    start_code += 1;
                }
            }
        } else if format == 12 {
            for _ in 0..10 { _ = buffer.read_u8(); }

            let group_len = buffer.read_u32::<BigEndian>().unwrap();
            for _ in 0..group_len {
                let start_code = buffer.read_u32::<BigEndian>().unwrap();
                let end_code = buffer.read_u32::<BigEndian>().unwrap();
                let start_glyph_index = buffer.read_u32::<BigEndian>().unwrap();

                let chars_len = end_code - start_code + 1;
                for char_offset in 0..chars_len {
                    let char_code = start_code + char_offset;
                    let glyph_index = start_glyph_index + char_offset;

                    map.push(GlyphMap::new(glyph_index as usize, char::from_u32(char_code).unwrap()));
                    has_missing_char_glyph |= glyph_index == 0;
                }
            }
        } else {
            error!("Font cmap format not supported");
        }

        if !has_missing_char_glyph {
            map.push(GlyphMap::new(0, char::from_u32(65535).unwrap()));
        }

        map
    }

    fn load_glyph_locations(buffer: &mut Cursor<Vec<u8>>, glyph_count: usize, bytes_per_entry: usize, loca_table_location: usize, glyf_table_location: usize) -> Vec<usize> {
        let mut locations = vec![0; glyph_count];

        for i in 0..glyph_count {
            buffer.set_position((loca_table_location + i * bytes_per_entry) as u64);
            let data_offset = if bytes_per_entry == 2 {
                buffer.read_u16::<BigEndian>().unwrap() as u32 * 2
            } else {
                buffer.read_u32::<BigEndian>().unwrap()
            };
            
            locations[i] = glyf_table_location + data_offset as usize;
        }

        locations
    }

    fn load_offset_table(buffer: &mut Cursor<Vec<u8>>) -> HashMap<String, usize> {
        let mut map = HashMap::new();

        let table_count = buffer.read_u16::<BigEndian>().unwrap();
        _ = buffer.read_u48::<BigEndian>().unwrap();

        info!("table count: {}", table_count);

        for _ in 0..table_count {
            let mut str_buffer = vec![0; 4_usize];
            buffer.read_exact(&mut str_buffer).unwrap();;
            let tag = String::from_utf8(str_buffer).unwrap();

            let _checksum = buffer.read_u32::<BigEndian>().unwrap();
            let offset = buffer.read_u32::<BigEndian>().unwrap();
            let _length = buffer.read_u32::<BigEndian>().unwrap();

            // info!("tag '{}' -> {}", tag, offset);
            map.insert(tag, offset as usize);
        }

        map
    }

}

#[repr(u8)]
enum GlyphFlagMask {
    OnCurve = 0,
    IsSingleByteX = 1,
    IsSingleByteY = 2,
    Repeat = 3,
    InstructionX = 4,
    InstructionY = 5,
}

impl GlyphInfo {
    pub fn load_all(buffer: &mut Cursor<Vec<u8>>, glyph_map: Vec<GlyphMap>, locations: Vec<usize>) -> Vec<Self> {
        let mut glyphs = vec![Self::default(); glyph_map.len()];

        for i in 0..glyph_map.len() {
            let mut glyph = Self::load_single(buffer, &locations, glyph_map[i].index);
            glyph.unicode_value = glyph_map[i].unicode;
            glyphs[i] = glyph;
        }

        glyphs
    }

    fn load_single(buffer: &mut Cursor<Vec<u8>>, locations: &Vec<usize>, glyph_index: usize) -> Self {    
        Self::load_simple(buffer, locations, glyph_index)
    }

    fn load_simple(buffer: &mut Cursor<Vec<u8>>, locations: &Vec<usize>, glyph_index: usize) -> Self {
        let location = locations[glyph_index];
        
        buffer.set_position(location as u64);
        let contour_len = buffer.read_u16::<BigEndian>().unwrap();

        let min_x = buffer.read_u16::<BigEndian>().unwrap();
        let min_y = buffer.read_u16::<BigEndian>().unwrap();
        let max_x = buffer.read_u16::<BigEndian>().unwrap();
        let max_y = buffer.read_u16::<BigEndian>().unwrap();

        let mut point_len: usize = 0;
        let mut end_indices = vec![];
        for _ in 0..contour_len {
            let end_idx = buffer.read_u16::<BigEndian>().unwrap_or(0);
            point_len = point_len.max(end_idx as usize + 1);
            end_indices.push(end_idx);
        }

        let instruction_length = buffer.read_u16::<BigEndian>().unwrap_or(0);
        for _ in 0..instruction_length { _ = buffer.read_u8(); }

        let mut flags = vec![0_u8; point_len];

        let mut iterator = 0..point_len;
        while let Some(i) = iterator.next() {
            let flag = buffer.read_u8().unwrap_or(0);
            flags[i] = flag;

            if is_bit_set(flag, GlyphFlagMask::Repeat as usize) {
                let repeat_count = buffer.read_u8().unwrap();
                for _ in 0..repeat_count {
                    iterator.next();
                    flags[i] = flag;
                }
            }
        }

        let mut points = vec![GlyphPoint::default(); point_len];
        Self::read_coords_simple(&mut points, buffer, &flags, true);
        Self::read_coords_simple(&mut points, buffer, &flags, false);

        Self {
            max_x, max_y, min_x, min_y,
            advance_width: 0,
            end_indices,
            left_side_bearing: 0,
            points,
            unicode_value: 'a',
        }
    }

    fn read_coords_simple(points: &mut Vec<GlyphPoint>, buffer: &mut Cursor<Vec<u8>>, flags: &Vec<u8>, reading_x: bool) {
        let single_byte_flag_bit = if reading_x { GlyphFlagMask::IsSingleByteX as usize } else { GlyphFlagMask::IsSingleByteY as usize };
        let instruction_flag = if reading_x { GlyphFlagMask::InstructionX as usize } else { GlyphFlagMask::InstructionY as usize };

        let mut coord_val = 0;

        for i in 0..points.len() {
            let flag = flags[i];

            if is_bit_set(flag, single_byte_flag_bit) {
                let coord_offset = buffer.read_u8().unwrap_or(0);
                let positive_offset = is_bit_set(flag, instruction_flag);
                coord_val += if positive_offset {
                    coord_offset as i32
                } else { -(coord_offset as i32) };
            } else if !is_bit_set(flag, instruction_flag) {
                coord_val += buffer.read_u16::<BigEndian>().unwrap_or(0) as i32;
            }

            if reading_x {
                points[i].p.x = coord_val as f32;
            } else {
                points[i].p.y = coord_val as f32;
            }

            points[i].on_curve = is_bit_set(flag, GlyphFlagMask::OnCurve as usize);
        }
    }

    pub fn width(&self) -> u16 {
        self.max_x - self.min_x
    }

    pub fn height(&self) -> u16 {
        self.max_y - self.min_y
    }
}

fn is_bit_set(byte: u8, i: usize) -> bool {
    ((byte >> i) & 1) == 1
}