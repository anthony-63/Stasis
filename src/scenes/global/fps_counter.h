#pragma once

#include <flecs.h>
#include <raylib.h>

typedef struct {
    Vector2 Position;
} FpsCounterData;

void define_fps_counter();
void init_fps_counter(ecs_entity_t parent, int x, int y);