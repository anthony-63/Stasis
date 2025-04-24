#include <stdio.h>

#include <flecs.h>
#include <raylib.h>

int main() {
    InitWindow(800, 600, "hi");

    ecs_world_t* world = ecs_init();

    

    ecs_fini(world);
}
