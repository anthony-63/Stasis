#pragma once

#include <raylib.h>
#include <flecs.h>

typedef struct {
    Camera3D camera;
} CameraData;

void define_camera();
void init_camera(ecs_entity_t player, Vector3 position);