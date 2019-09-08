#version 400 core

in vec3 fragTexCoord;

layout (location = 0) out vec4 gNormal;
layout (location = 1) out vec3 gAlbedo;

uniform samplerCube skyBox;

void main() {
    gAlbedo = texture(skyBox, fragTexCoord).rgb;
    gNormal = vec4(0);// normal = Zero represents that the pixel is from the background.
}