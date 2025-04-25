#pragma once

#include <raylib.h>
#include <flecs.h>

typedef struct {
    Material material;
    Mesh mesh;
} TexturedMesh;

void define_textured_mesh();