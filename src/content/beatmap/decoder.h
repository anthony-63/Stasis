#pragma once

#include <stdint.h>
#include <stdio.h>

#include "beatmap.h"

typedef struct {
    FILE* handle;
    char* file_path;
    Beatmap output;
} BeatmapDecoder;

BeatmapDecoder new_decoder(char* path);
Beatmap decode_map(BeatmapDecoder* decoder);
uint8_t decode_u8(BeatmapDecoder* decoder);
uint16_t decode_u16(BeatmapDecoder* decoder);
uint32_t decode_u32(BeatmapDecoder* decoder);
uint64_t decode_u64(BeatmapDecoder* decoder);
void decoder_seek(BeatmapDecoder* decoder, size_t address);
void decoder_skip(BeatmapDecoder* decoder, size_t to_skip);
char* decode_string(BeatmapDecoder* decoder);