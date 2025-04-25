#include "player.h"

#include "grid.h"
#include "camera.h"

extern ecs_world_t* world;

ECS_TAG_DECLARE(PlayerTag);

void define_player() {
    ECS_TAG_DEFINE(world, PlayerTag);
    define_camera();
    define_grid();
}

void init_player(ecs_entity_t root) {
    ecs_entity_t player = ecs_new(world);
    ecs_add(world, player, PlayerTag);
    ecs_set_name(world, player, "Player");

    init_grid(player);
    init_camera(player, (Vector3){0, 0, 7});

    ecs_add_pair(world, player, EcsChildOf, root);
}