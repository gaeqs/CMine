#version 440 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec2 texturePosition;

out vec3 fragPos;
out vec2 fragTexCoord;

uniform vec2 instancePosition;
uniform vec2 instanceSize;

void main () {
    gl_Position = vec4(position.xy * instanceSize + instancePosition, 1, 1);
    fragTexCoord = texturePosition;
}
