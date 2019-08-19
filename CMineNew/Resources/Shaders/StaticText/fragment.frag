#version 400 core

out vec4 FragColor;

in vec2 fragTextCoord;
in vec4 fragColor;

uniform float relative;
uniform sampler2D textureSampler;

void main() {
    vec4 color = texture(textureSampler, fragTextCoord) * fragColor;
    if(color.w < 0.01) discard;
    FragColor = color;
}