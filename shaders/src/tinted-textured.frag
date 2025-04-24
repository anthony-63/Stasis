#version 450

layout(set=2, binding=0) uniform sampler2D tex_sampler;

layout (location = 0) in vec2 tex_coord;
layout (location = 1) in vec4 color;

layout (location = 0) out vec4 final_color;
    
void main() {
    final_color = texture(tex_sampler, tex_coord) * color;
}