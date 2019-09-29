#version 440 core

in vec3 fragPos;
in vec2 fragTexCoord;

out vec4 FragColor;

uniform sampler2D pointer;

void main() {
    vec4 texture = texture(pointer, fragTexCoord);
    if(texture.w < 0.1) discard;
    FragColor = vec4(1);
}