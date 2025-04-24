#pragma once

#include "decoder.h"

typedef enum {
    SSPM_OFFSET_MAGIC = 0x0,
    SSPM_OFFSET_VERSION = 0x4,
    SSPM_OFFSET_HAS_COVER = 0x2e,
    SSPM_OFFSET_MAP_LENGTH = 0x1E,
    SSPM_OFFSET_NOTE_COUNT = 0x22,
    SSPM_OFFSET_DIFFICULTY = 0x2a,
    SSPM_OFFSET_DATA = 0x30,
    SSPM_OFFSET_COVER = 0x50,
    SSPM_OFFSET_MARKER = 0x70,
    SSPM_OFFSET_ID = 0x80,
} sspm_block_offset_t;

typedef struct {
    uint64_t custom_offset;
    uint64_t marker_offset;
    uint64_t audio_offset;
    uint64_t audio_length;
    uint64_t cover_offset;
    uint64_t cover_length;
} sspm_data_offsets_t;

sspm_data_offsets_t get_sspm_data_offsets(BeatmapDecoder* decoder);
Beatmap decode_sspm(BeatmapDecoder* decoder);