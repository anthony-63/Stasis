#include "decoder.h"

#include <stdlib.h>
#include "versions/v1.h"

BeatmapDecoder new_decoder(char* path) {
    FILE* handle = fopen(path, "rb");

    if(handle == NULL) {
        printf("DECODER: Failed to open file for reading '%s'\n", path);
        
    }

    return (BeatmapDecoder) {
        .handle = handle,
        .file_path = path,
        .output = (Beatmap){.valid = handle != NULL},
    };
}

void decode_metadata(BeatmapDecoder* decoder, uint8_t version) {
    switch(version) {
        case 1: v1_decode_metadata(decoder); break;
        default: 
            printf("Invalid map version: %d\n", version);
            decoder->output.valid = false;
    }
}

void decode_notes(BeatmapDecoder* decoder, uint8_t version) {
    switch(version) {
        case 1: v1_decode_notes(decoder); break;
        default:
            printf("Invalid map version: %d\n", version);
            decoder->output.valid = false;
    }
}

void decode_audio(BeatmapDecoder* decoder, uint8_t version) {
    switch(version) {
        case 1: v1_decode_audio(decoder); break;
        default: 
            printf("Invalid map version: %d\n", version);
            decoder->output.valid = false;
    }
}

Beatmap decode_map(BeatmapDecoder* decoder) {
    uint8_t version = decode_u8(decoder);
    decode_metadata(decoder, version);
    decode_notes(decoder, version);
    decode_audio(decoder, version);
    return decoder->output;
}

uint8_t decode_u8(BeatmapDecoder* decoder) {
    uint8_t data = 0;
    fread(&data, sizeof(uint8_t), 1, decoder->handle);
    return data;
}

uint16_t decode_u16(BeatmapDecoder* decoder) {
    uint16_t data = 0;
    fread(&data, sizeof(uint16_t), 1, decoder->handle);
    return data;
}

uint32_t decode_u32(BeatmapDecoder* decoder) {
    uint32_t data = 0;
    fread(&data, sizeof(uint32_t), 1, decoder->handle);
    return data;
}

uint64_t decode_u64(BeatmapDecoder* decoder) {
    uint64_t data = 0;
    fread(&data, sizeof(uint64_t), 1, decoder->handle);
    return data;
}

char* decode_string(BeatmapDecoder* decoder) {
    uint16_t length = decode_u16(decoder);
    char* output = malloc(sizeof(char) * length + 1);
    for(int i = 0; i < length; i++) {
        output[i] = decode_u8(decoder);
    }
    output[length] = '\0';
    return output;
}

void decoder_seek(BeatmapDecoder* decoder, size_t address) {
    fseek(decoder->handle, address, SEEK_SET);
}

void decoder_skip(BeatmapDecoder* decoder, size_t to_skip) {
    fseek(decoder->handle, to_skip, SEEK_CUR);
}