#include "game.h"

#include "../../types/fps_counter.h"

#include "player/player.h"

void define_game_scene() {
    define_player();
}

void init_game_scene(ecs_entity_t scene) {
    init_fps_counter(scene, 10, 10);
    init_player(scene);
}