#pragma once

#include "../decoder.h"

void v1_decode_metadata(BeatmapDecoder* decoder);
void v1_decode_notes(BeatmapDecoder* decoder);
void v1_decode_audio(BeatmapDecoder* decoder);