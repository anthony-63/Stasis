#version 450

// Get the vertex position from the vertex buffer
layout (location = 0) in vec3 pos;
layout (location = 1) in vec2 tex_coord;

// Output texture coordinates to the fragment shader
layout (location = 0) out vec2 out_tex_coord;

void main(void) {
	gl_Position = vec4(pos, 1);
    out_tex_coord = tex_coord;
}