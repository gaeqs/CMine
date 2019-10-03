#version 440 core

in vec3 fragPos, fragNormal;
in vec2 fragTexCoord;
flat in float fragColorFilter;
in float fragLight;

layout (location = 0) out vec2 gNormal;
layout (location = 1) out vec3 gAlbedo;
layout (location = 2) out vec3 gBrightness;

uniform sampler2D sampler;

void main() {
    vec4 texture4 = texture(sampler, fragTexCoord);
    if (texture4.w < 0.1) discard;
    vec3 texture = texture4.rgb;

    if (texture.r == texture.g && texture.r == texture.b) {
        uint color = floatBitsToUint(fragColorFilter);
        uint a = color & 0xFF;
        if(a > 100) {
            uint r = color >> 24;
            uint g = (color >> 16) & 0xFF;
            uint b = (color >> 8) & 0xFF;
            texture = vec3(r, g, b) * texture.r  / 255.0;
        }
    }

    float light = 0.05 + fragLight * 0.95;
    gBrightness = vec3(1) * light;
    gAlbedo = texture;

    gNormal = fragNormal.xy;
}