#version 440 core

in vec3 fragTexCoord;

layout (location = 0) out vec4 gNormal;
layout (location = 1) out vec4 gAlbedo;

uniform samplerCube skyBox;

void main() {
    gAlbedo = vec4(texture(skyBox, fragTexCoord).rgb, 0);
    gNormal = vec4(0);// normal = Zero represents that the pixel is from the background.
}