#include "sspm.h"

#include <stdlib.h>
#include <string.h>

sspm_data_offsets_t get_sspm_data_offsets(beatmap_decoder_t* decoder) {
    sspm_data_offsets_t offsets = (sspm_data_offsets_t) {0};
    decoder_seek(decoder, SSPM_OFFSET_DATA);
    
    offsets.custom_offset = decode_u64(decoder);
    decoder_skip(decoder, 0x8);

    offsets.audio_offset = decode_u64(decoder);
    offsets.audio_length = decode_u64(decoder);

    decoder_seek(decoder, SSPM_OFFSET_MARKER);
    offsets.marker_offset = decode_u64(decoder);

    decoder_seek(decoder, SSPM_OFFSET_COVER);
    offsets.cover_offset = decode_u64(decoder);
    offsets.cover_length = decode_u64(decoder);

    return offsets;
}

int check_magic(beatmap_decoder_t* decoder) {
    uint32_t magic = decode_u32(decoder);
    return magic == 0x53532b6d; // 'SS+m'
}

int get_version(beatmap_decoder_t* decoder) {
    decoder_seek(decoder, SSPM_OFFSET_VERSION);
    return decode_u16(decoder);
}

char* get_title(beatmap_decoder_t* decoder) {
    decoder_seek(decoder, SSPM_OFFSET_ID);
    size_t title_offset = SSPM_OFFSET_ID + decode_u16(decoder) + 2;
    decoder_seek(decoder, title_offset);

    return decode_string(decoder);
}

uint8_t* get_audio_buffer(beatmap_decoder_t* decoder, sspm_data_offsets_t offsets) {
    decoder_seek(decoder, offsets.audio_offset);
    uint8_t* audio_buffer = malloc(offsets.audio_length);
    for(uint64_t i = 0; i < offsets.audio_length; i++) {
        audio_buffer[i] = decode_u8(decoder);
    }

    return audio_buffer;
}

size_t get_note_count(beatmap_decoder_t* decoder) {
    decoder_seek(decoder, SSPM_OFFSET_NOTE_COUNT);
    return decode_u32(decoder);
}

note_t* get_notes(beatmap_decoder_t* decoder, size_t note_count, sspm_data_offsets_t offsets) {
    decoder_seek(decoder, offsets.marker_offset);

    union conv {
        uint32_t u;
        float f;
    };

    note_t* notes = malloc(sizeof(note_t) * note_count);
    for(size_t i = 0; i < note_count; i++) {
        float time = decode_u32(decoder);
        decode_u8(decoder); // always 1 lol
        int has_quantum = decode_u8(decoder);

        if(has_quantum) {
            notes[i] = (note_t) {
                .quantum = has_quantum,
                .time = time,
                .x = (((union conv){.u = decode_u32(decoder)}).f - 1),
                .y = -(((union conv){.u = decode_u32(decoder)}).f - 1),
            };
        } else {
            notes[i] = (note_t) {
                .quantum = has_quantum,
                .time = time,
                .x = ((int)decode_u8(decoder) - 1),
                .y = -((int)decode_u8(decoder) - 1)
            };
        }
    }
    return notes;
}

beatmap_t decode_sspm(beatmap_decoder_t* decoder) {
    if(check_magic(decoder)) {
        printf("Invalid magic for map: %s\n", decoder->file_path);
        exit(-1);
    }
    printf("verified 'SS+m' magic, continuing...\n");

    int version = get_version(decoder);
    if(version != 2) {
        printf("invalid version: %d\n", version);
    }
    printf("version: %d\n", version);

    sspm_data_offsets_t offsets = get_sspm_data_offsets(decoder);

    char* title = get_title(decoder);
    printf("title: %s\n", title);

    size_t note_count = get_note_count(decoder);
    note_t* notes = get_notes(decoder, note_count, offsets);

    printf("parsed %d notes\n", note_count);

    uint8_t* audio_buffer = get_audio_buffer(decoder, offsets);

    printf("read %d bytes of audio\n", offsets.audio_length);

    return (beatmap_t){
        .title = title,
        .mapper = "NONE",
        .artist = "SSPM",
        .version = 1,
        .audio_size = offsets.audio_length,
        .audio = audio_buffer,
        .note_count = note_count,
        .notes = notes,
    };
}