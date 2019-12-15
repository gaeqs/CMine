#version 440 core

in vec3 fragTexCoord;

out vec4 fragColor;

uniform samplerCube skyBox;

void main() {
    fragColor = texture(skyBox, fragTexCoord);
}