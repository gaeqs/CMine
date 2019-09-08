#version 400 core

in vec3 fragPos, fragNormal;
in vec2 fragTexCoord;
in vec4 fragColorFilter;

layout (location = 0) out vec4 gNormal;
layout (location = 1) out vec3 gAlbedo;

uniform sampler2D sampler;

void main() {
    vec4 texture = texture(sampler, fragTexCoord);
    if (texture.w < 0.1) discard;

    if (texture.r == texture.g && texture.r == texture.b && fragColorFilter.a > 0.5) {
        texture = fragColorFilter * texture.r;
    }

    gAlbedo = texture.rgb;

    //W = Specular Weight
    gNormal = vec4(fragNormal, 8);
}