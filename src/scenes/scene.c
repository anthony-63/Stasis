#include "scene.h"

extern ecs_world_t* world;

ECS_TAG_DECLARE(Scene);

void define_scene_manager() {
    ECS_TAG_DEFINE(world, Scene);
}

ecs_entity_t empty_scene() {
    ecs_iter_t iter = ecs_query_iter(world, ecs_query(world, {
        .terms = {
            { .first.id = Scene }
        }
    }));

    while(ecs_query_next(&iter)) {
        ecs_id_t id = ecs_field_id(&iter, 0);
        ecs_delete(world, id);
    }

    ecs_entity_t scene = ecs_new(world);
    ecs_add_id(world, scene, Scene);
    return scene;
}