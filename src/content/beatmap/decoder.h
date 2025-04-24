#pragma once

#include <stdint.h>
#include <stdio.h>

#include "beatmap.h"

typedef struct {
    FILE* handle;
    char* file_path;
    beatmap_t output;
} beatmap_decoder_t;

beatmap_decoder_t new_decoder(char* path);
beatmap_t decode_map(beatmap_decoder_t* decoder);
uint8_t decode_u8(beatmap_decoder_t* decoder);
uint16_t decode_u16(beatmap_decoder_t* decoder);
uint32_t decode_u32(beatmap_decoder_t* decoder);
uint64_t decode_u64(beatmap_decoder_t* decoder);
void decoder_seek(beatmap_decoder_t* decoder, size_t address);
void decoder_skip(beatmap_decoder_t* decoder, size_t to_skip);
char* decode_string(beatmap_decoder_t* decoder);