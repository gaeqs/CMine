#version 440 core

in vec3 fragPos, fragNormal;
in vec2 fragTexCoord;
flat in float fragColorFilter;
in float fragLight;

layout (location = 0) out vec4 gNormal;
layout (location = 1) out vec4 gAlbedo;
layout (location = 2) out vec3 gBrightness;

uniform sampler2D sampler;

void main() {
    vec4 texture4 = texture(sampler, fragTexCoord);
    if (texture4.w < 0.1) discard;
    vec3 texture = texture4.rgb;
    float light = 0.05 + fragLight * 0.95;
    gBrightness = vec3(1) * light;
    gAlbedo = vec4(texture, fragColorFilter);

    //W = Specular Weight
    gNormal = vec4(fragNormal, 2);
}