#version 440 core

uniform sampler2D ssao;

in vec2 fragTexCoords;
out float FragColor;


void main() {
    vec2 texelSize = 1.0 / vec2(textureSize(ssao, 0));
    float result = 0.0;
    for (int x = -2; x < 2; ++x) {
        for (int y = -2; y < 2; ++y) {
            vec2 offset = vec2(float(x), float(y)) * texelSize;
            result += texture(ssao, fragTexCoords + offset).r;
        }
    }
    FragColor = result / 16.0;
}