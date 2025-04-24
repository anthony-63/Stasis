#version 450

layout(set=2, binding=0) uniform sampler2D tex_sampler;

layout (location = 0) in vec2 tex_coord;

layout (location = 0) out vec4 final_color;

void main() {
    vec2 real_uv = tex_coord.st * vec2(1.0, -1.0);
    final_color = texture(tex_sampler, real_uv);
}