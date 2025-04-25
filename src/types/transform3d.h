#pragma once

#include <raylib.h>
#include <flecs.h>

typedef struct {
    Vector3 position;
    Vector3 rotation;
} Transform3D;

void define_transform3d();
Matrix get_transform_matrix(Transform3D transform);