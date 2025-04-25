#include "scene.h"

extern ecs_world_t* world;

ECS_TAG_DECLARE(Scene);

void define_scene_manager() {
    ECS_TAG_DEFINE(world, Scene);
}

ecs_entity_t empty_scene() {
    ecs_delete_with(world, Scene);

    ecs_entity_t scene = ecs_new(world);
    ecs_set_name(world, scene, "Root");
    ecs_add_id(world, scene, Scene);
    return scene;
}