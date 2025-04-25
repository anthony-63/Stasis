#include <stdio.h>

#include <flecs.h>
#include <raylib.h>

#include "scenes/scene.h"
#include "scenes/loading/loading.h"
#include "scenes/global/fps_counter.h"
#include "content/beatmap/beatmap.h"

ecs_world_t* world;

int main() {
    InitWindow(1280, 720, "Stasis");

    world = ecs_init();

    ECS_IMPORT(world, FlecsStats);
    ecs_singleton_set(world, EcsRest, {0});
    
    define_scene_manager();
    define_fps_counter();
    define_beatmap();

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
