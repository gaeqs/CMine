#version 400 core

layout (location = 0) out vec4 gNormal;
layout (location = 1) out vec3 gAlbedo;

void main() {
    gAlbedo = vec3(0, 0, 0);
    //W = Specular Weight
    gNormal = vec4(0, 1, 0, 8);
}