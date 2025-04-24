#include "fps_counter.h"

extern ecs_world_t* world;

ECS_COMPONENT_DECLARE(FpsCounterData);

void draw_system(ecs_iter_t* it) {
    FpsCounterData* data = ecs_field(it, FpsCounterData, 0);

    for(int i = 0; i < it->count; i++) {
        DrawFPS(data[i].Position.x, data[i].Position.y);
    }
}

void define_fps_counter() {
    ECS_COMPONENT_DEFINE(world, FpsCounterData);
    ECS_SYSTEM(world, draw_system, EcsPostUpdate, FpsCounterData);
}

void init_fps_counter(ecs_entity_t parent, int x, int y) {
    ecs_entity_t counter = ecs_new(world);
    ecs_add(world, counter, FpsCounterData);
    ecs_set(world, counter, FpsCounterData, {
        .Position = (Vector2){x, y},
    });

    ecs_add_pair(world, counter, EcsChildOf, parent);
}