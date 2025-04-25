#include "transform3d.h"

#include <raymath.h>

extern ecs_world_t* world;

ECS_COMPONENT_DECLARE(Transform3D);

void define_transform3d() {
    ECS_COMPONENT_DEFINE(world, Transform3D);
}

Matrix get_transform_matrix(Transform3D transform) {
    Matrix m = MatrixTranslate(transform.position.x, transform.position.y, transform.position.z);
    m = MatrixMultiply(m, MatrixRotateX(DEG2RAD * transform.rotation.x));
    m = MatrixMultiply(m, MatrixRotateY(DEG2RAD * transform.rotation.y));
    m = MatrixMultiply(m, MatrixRotateZ(DEG2RAD * transform.rotation.z));
    return m;
}