#include "grid.h"
#include "../../../types/transform3d.h"
#include "../../../types/textured_mesh.h"

extern ecs_world_t* world;


extern ECS_COMPONENT_DECLARE(Transform3D);
extern ECS_COMPONENT_DECLARE(TexturedMesh);

ECS_TAG_DECLARE(GridTag);

void render_grid(ecs_iter_t* iter) {
    TexturedMesh* meshes = ecs_field(iter, TexturedMesh, 1);
    Transform3D* transforms = ecs_field(iter, Transform3D, 2);

    for(int i = 0; i < iter->count; i++) {
        Matrix transform = get_transform_matrix(transforms[i]);
        DrawMesh(meshes[i].mesh, meshes[i].material, transform);
    }
}

void define_grid() {
    ECS_TAG_DEFINE(world, GridTag);
    ECS_SYSTEM(world, render_grid, EcsPostUpdate, GridTag, TexturedMesh, Transform3D);
}

void set_mesh(ecs_entity_t grid) {
    Image img = LoadImage("assets/game/grid.png");
    Texture2D texture = LoadTextureFromImage(img);
    UnloadImage(img);

    Material mat = LoadMaterialDefault();
    Mesh mesh = GenMeshPlane(6, 6, 1, 1);
    SetMaterialTexture(&mat, MATERIAL_MAP_DIFFUSE, texture);

    ecs_set(world, grid, TexturedMesh, {
        .material = mat,
        .mesh = mesh,
    });
}

void init_grid(ecs_entity_t player) {
    ecs_entity_t grid = ecs_new(world);
    ecs_add(world, grid, GridTag);
    
    ecs_set(world, grid, Transform3D, {
        .position = (Vector3){0, 0, 0},
        .rotation = (Vector3){90, 0, 0},
    });

    set_mesh(grid);

    ecs_set_name(world, grid, "Grid");
    ecs_add_pair(world, grid, EcsChildOf, player);
}