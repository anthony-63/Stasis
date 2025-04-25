#include "beatmap.h"

#include <flecs.h>

extern ecs_world_t* world;

ECS_COMPONENT_DECLARE(BeatmapSingleton);

void define_beatmap() {
    ECS_COMPONENT_DEFINE(world, BeatmapSingleton);
}