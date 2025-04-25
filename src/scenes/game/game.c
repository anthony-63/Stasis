#include "game.h"

#include "../global/fps_counter.h"

void init_game_scene(ecs_entity_t scene) {
    init_fps_counter(scene, 10, 10);
}