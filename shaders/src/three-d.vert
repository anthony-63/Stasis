#version 450

layout (location = 0) in vec3 pos;
layout (location = 1) in vec2 tex_coord;

layout (location = 0) out vec2 out_tex_coord;

layout(set = 1, binding = 0) uniform PushConstants {
    mat4 model;
    mat4 view;
    mat4 projection;
};

void main(void) {
    mat4 mvp = projection * view * model;
	gl_Position = mvp * vec4(pos, 1);
    out_tex_coord = tex_coord;
}