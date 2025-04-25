#pragma once

#include <stdint.h>
#include <stdbool.h>

enum MetadataTag {
    META_TITLE,
    META_ARTIST,
    META_AUDIO_SIZE,
    META_NOTE_COUNT,
    META_MAPPER,
};

#define V1_META_SIZE META_MAPPER 

typedef struct {
    double x;
    double y;

    uint32_t time;
    uint8_t quantum;
} NoteData;

typedef struct {
    bool valid;
    uint8_t version;
    char* title;
    char* artist;
    char* mapper;
    uint32_t audio_size;
    uint32_t note_count;

    uint8_t* audio;
    NoteData* notes;
} Beatmap;

typedef struct {
    Beatmap map;
} BeatmapSingleton;

void define_beatmap();