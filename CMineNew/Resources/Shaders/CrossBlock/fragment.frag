#version 440 core

in VertexData {
    vec3 fragPos;
    vec3 fragNormal;
    vec2 fragTexCoord;
    vec4 fragColorFilter;
    float fragLight;
} data;

layout (location = 0) out vec2 gNormal;
layout (location = 1) out vec3 gAlbedo;
layout (location = 2) out vec3 gBrightness;

uniform sampler2D sampler;

void main() {
    vec4 texture4 = texture(sampler, data.fragTexCoord);
    if (texture4.w < 0.1) discard;
    vec3 texture = texture4.rgb;

    if (texture.r == texture.g && texture.r == texture.b && data.fragColorFilter.a > 0.5) {
        texture *= data.fragColorFilter.rgb;
    }

    float light = 0.05 + data.fragLight * 0.95;
    gBrightness = vec3(1) * light;
    gAlbedo = texture;

    gNormal = data.fragNormal.xy;
}