#version 440 core

layout (location = 0) in vec3 position;

out vec4 fragColor;

void main () {
    gl_Position = vec4(position, 1);
    if(gl_VertexID % 4 > 1) {
        fragColor = vec4(1, 0, 0, 1);
    }
    else {
        fragColor = vec4(0, 1, 0, 1);
    }
}
