#pragma once

#include <raylib.h>

typedef struct {
    float sensitivity;
    int clamped;
    float scale;
} cursor_settings_t;

typedef struct {
    float approach_distance;
    float approach_time;
    int pushback;
    Color* colors;
    int color_count;
} note_settings_t;

typedef struct {
    float parallax;
    float fov;
} camera_settings_t;

typedef struct {
    cursor_settings_t cursor;
    camera_settings_t camera;
    note_settings_t note;
} settings_t;