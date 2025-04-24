#pragma once

#include "../decoder.h"

void v1_decode_metadata(beatmap_decoder_t* decoder);
void v1_decode_notes(beatmap_decoder_t* decoder);
void v1_decode_audio(beatmap_decoder_t* decoder);