#version 440 core

in vec3 fragPos;
in vec2 fragTexCoord;

out vec4 FragColor;

uniform sampler2D elementTexture;

void main() {
    FragColor = texture(elementTexture, fragTexCoord);
    if (FragColor.w < 0.1) discard;
}