#include "loading.h"

#include "../global/fps_counter.h"

void init_loading_scene(ecs_entity_t scene) {
    init_fps_counter(scene, 10, 10);
}
