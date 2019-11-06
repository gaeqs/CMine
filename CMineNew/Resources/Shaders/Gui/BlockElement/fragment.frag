#version 440 core

in vec2 fragTexCoord;
flat in vec4 fragColorFilter;

out vec4 outColor;

uniform sampler2D sampler;

void main() {
    vec4 texture4 = texture(sampler, fragTexCoord);
    if (texture4.w < 0.1) discard;
    vec3 texture = texture4.rgb;

    if (texture.r == texture.g && texture.r == texture.b && fragColorFilter.a > 0.5) {
        texture *= fragColorFilter.rgb;
    }
    
    outColor = vec4(texture, 1);
}