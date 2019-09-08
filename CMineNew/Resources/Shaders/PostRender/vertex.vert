#version 400 core

layout (location = 0) in vec2 position;
layout (location = 1) in vec2 texturePosition;

out vec2 fragPos, fragTexCoords;

void main () {
    gl_Position = vec4(position.xy, 0, 1);
    fragPos = position.xy;
    fragTexCoords = texturePosition;
}
