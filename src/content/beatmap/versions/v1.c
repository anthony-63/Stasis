#include "v1.h"

#include "../util.h"
#include <stdlib.h>
#include <math.h>

void v1_decode_metadata(BeatmapDecoder* decoder) {
    uint8_t count = decode_u8(decoder);

    for(int i = 0; i < count; i++) {
        switch(decode_u8(decoder)) {
            case META_TITLE: decoder->output.title = decode_string(decoder); break;
            case META_ARTIST: decoder->output.artist = decode_string(decoder); break;
            case META_MAPPER: decoder->output.mapper = decode_string(decoder); break;
            case META_NOTE_COUNT: decoder->output.note_count = decode_u32(decoder); break;
            case META_AUDIO_SIZE: decoder->output.audio_size = decode_u32(decoder); break;
        }
    }
}

void v1_decode_notes(BeatmapDecoder* decoder) {
    decoder->output.notes = malloc(sizeof(NoteData) * decoder->output.note_count);
    uint32_t NoteDataime = 0;
    for(uint32_t i = 0; i < decoder->output.note_count; i++) {
        uint8_t header = decode_u8(decoder);
        NoteData note = (NoteData){0};
        note.quantum = header >> 7;
        int time_long = (header & 0b01000000) >> 6;

        if(note.quantum) {
            int xfixedp = (header & 0b00001000) >> 3;
            int yfixedp = (header & 0b00000100) >> 2;
            int xneg = (header & 0b00000010) >> 1;
            int yneg = header & 0b00000001;
            
            double xval = xfixedp ? float64_8(decode_u8(decoder)) : float64_16(decode_u16(decoder));
            note.x = xneg ? -(xval) : xval;
            double yval = yfixedp ? float64_8(decode_u8(decoder)) : float64_16(decode_u16(decoder));
            note.y = yneg ? -(yval) : yval;
        } else {
            note.x = (header & 0b1100) >> 2;
            note.y = (header & 0b11);
        }

        int diff = 0;
        if(time_long) diff = decode_u32(decoder);
        else diff = decode_u16(decoder);
        NoteDataime += diff;
        note.time = NoteDataime;
    }
}

void v1_decode_audio(BeatmapDecoder* decoder) {
    decoder->output.audio = malloc(decoder->output.audio_size);

    for(uint32_t i = 0; i < decoder->output.audio_size; i++) {
        decoder->output.audio[i] = decode_u8(decoder);
    }
}

