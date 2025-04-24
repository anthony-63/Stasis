#version 450

layout (location = 0) in vec4 in_color;
layout (location = 1) in vec3 position;

layout (location = 0) out vec4 color;

void main() {
    color = in_color;
    gl_Position = vec4(position, 1);
}