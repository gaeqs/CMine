#version 400 core

in vec3 fragPos, fragNormal;
in vec2 fragTexCoord;

layout (location = 0) out vec4 gAmbient;
layout (location = 1) out vec4 gDiffuse;
layout (location = 2) out vec4 gSpecular;
layout (location = 3) out vec3 gPosition;
layout (location = 4) out vec4 gNormal;

uniform sampler2D sampler;

void main() {
    gAmbient = vec4(texture(sampler, fragTexCoord).rgb, 1);
    gDiffuse = vec4(0, 0, 0, 1);
    gSpecular = vec4(0, 0, 0, 1);
    gPosition = fragPos;
    gNormal = vec4(fragNormal, 0);
}