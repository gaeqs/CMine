#version 440 core

layout (location = 0) out vec2 gNormal;
layout (location = 1) out vec4 gAlbedo;

void main() {
    gAlbedo = vec4(0, 0, 0, 0);
    //W = Specular Weight
    gNormal = vec2(0, 1);
}