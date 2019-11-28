#version 440 core

layout (location = 0) in vec2 position;
layout (location = 1) in vec2 texturePosition;

out vec2 fragTexCoords;

void main () {
    gl_Position = vec4(position.xy, 0, 1);
    fragTexCoords = texturePosition;
}
