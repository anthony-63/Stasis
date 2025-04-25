#include "camera.h"

extern ecs_world_t* world;

ECS_COMPONENT_DECLARE(CameraData);

void start_camera(ecs_iter_t* iter) {
    CameraData data = ecs_field(iter, CameraData, 0)[0];
    BeginMode3D(data.camera);
}

void end_camera(ecs_iter_t* iter) {
    EndMode3D();
}

void define_camera() {
    ECS_COMPONENT_DEFINE(world, CameraData);
    ECS_SYSTEM(world, start_camera, EcsOnValidate, CameraData);
    ECS_SYSTEM(world, end_camera, EcsPreStore, CameraData);
}

void init_camera(ecs_entity_t player, Vector3 position) {
    ecs_entity_t camera = ecs_new(world);
    ecs_set(world, camera, CameraData, {
        .camera = (Camera3D) {
            .fovy = 70,
            .position = position,
            .target = (Vector3){0, 0, 0},
            .projection = CAMERA_PERSPECTIVE,
            .up = (Vector3){0, 1, 0},
        }
    });
    ecs_set_name(world, camera, "Camera");
    ecs_add_pair(world, camera, EcsChildOf, player);
}
