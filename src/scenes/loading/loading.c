#include "loading.h"

#include "../../types/fps_counter.h"
#include "../../content/beatmap/beatmap.h"
#include "../../content/beatmap/decoder.h"
#include "../../content/beatmap/sspm.h"

#include "../game/game.h"
#include "../scene.h"

extern ecs_world_t* world;
extern ECS_COMPONENT_DECLARE(BeatmapSingleton);

void load_assets() {
    BeatmapDecoder decoder = new_decoder("assets/maps/testmap.sspm");
    
    ecs_singleton_set(world, BeatmapSingleton, {
        .map = decode_sspm(&decoder),
    });
}

void init_loading_scene(ecs_entity_t scene) {
    init_fps_counter(scene, 10, 10);
    load_assets();

    ecs_entity_t game_scene = empty_scene();
    init_game_scene(game_scene);
}