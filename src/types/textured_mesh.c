#include "textured_mesh.h"

extern ecs_world_t* world;

ECS_COMPONENT_DECLARE(TexturedMesh);

void define_textured_mesh() {
    ECS_COMPONENT_DEFINE(world, TexturedMesh);
}