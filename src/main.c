#include <stdio.h>

#include <flecs.h>
#include <raylib.h>

#include "scenes/scene.h"
#include "scenes/loading/loading.h"
#include "scenes/game/game.h"
#include "content/beatmap/beatmap.h"
#include "types/types.h"

ecs_world_t* world;

int main() {
    InitWindow(1280, 720, "Stasis");

    world = ecs_init();

    ECS_IMPORT(world, FlecsStats);
    ecs_singleton_set(world, EcsRest, {0});
    
    define_types();
    define_scene_manager();
    define_beatmap();
    define_game_scene();

    ecs_entity_t loading_scene = empty_scene();
    init_loading_scene(loading_scene);

    while(!WindowShouldClose()) {
        BeginDrawing();
        ClearBackground(BLACK);
        ecs_progress(world, 0);
        EndDrawing();
    }

    ecs_fini(world);
}
