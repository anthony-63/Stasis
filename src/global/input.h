#pragma once

#include <raylib.h>
#include <flecs.h>

typedef struct {
    Vector2 mdel;
    Vector2 mpos;
    Vector2 lpos;
} Input;

void setup_input();