#version 440 core

in vec3 fragTexCoord;

layout (location = 0) out vec2 gNormal;
layout (location = 1) out vec3 gAlbedo;
layout (location = 2) out vec3 gBrightness;

uniform samplerCube skyBox;

void main() {
    gAlbedo = texture(skyBox, fragTexCoord).rgb;
    gNormal = vec2(2);
    gBrightness = vec3(1);
}