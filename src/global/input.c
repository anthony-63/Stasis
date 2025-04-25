#include "input.h"

#include <raymath.h>

extern ecs_world_t* world;

ECS_COMPONENT_DECLARE(Input);

void update_input(ecs_iter_t* iter) {
    Input* input = ecs_get_mut(world, ecs_id(Input), Input);
    Vector2 mpos = GetMousePosition();
    input->mpos = mpos;
    input->mdel = Vector2Subtract(mpos, input->lpos);
    input->lpos = mpos;
}

void setup_input() {
    ECS_COMPONENT_DEFINE(world, Input);

    ECS_SYSTEM(world, update_input, EcsOnUpdate, Input);
    ecs_singleton_add(world, Input);
}