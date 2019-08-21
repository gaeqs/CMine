#version 400 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec2 texturePosition;

out vec3 fragPos;
out vec2 fragTexCoord;

uniform float aspectRatio;

void main () {
    gl_Position = vec4(position, 1);
    gl_Position.y *= aspectRatio;
    fragTexCoord = texturePosition;
}
