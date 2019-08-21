#version 400 core

in vec3 fragPos, fragNormal;
in vec2 fragTexCoord;
in vec4 fragColorFilter;

out vec4 FragColor;

uniform vec3 cameraPosition;
uniform sampler2D sampler;
uniform float waterShader;

void main() {
    vec4 texture = texture(sampler, fragTexCoord);
    if (texture.r == texture.g && texture.r == texture.b && fragColorFilter.a > 0.5) {
        texture = fragColorFilter * texture.r;
    }

    FragColor = texture;

    if (waterShader > 0.5) {
        float length = 1 - length(fragPos - cameraPosition) / 10;
        FragColor *= vec4(0.5, 0.5, 1, 1) * length;
    }
}