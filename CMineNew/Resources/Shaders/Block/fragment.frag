#version 400 core

in vec3 fragPos, fragNormal;
in vec2 fragTexCoord;

layout (location = 0) out vec3 gPosition;
layout (location = 1) out vec4 gNormal;
layout (location = 2) out vec4 gAmbient;
layout (location = 3) out vec4 gDiffuse;
layout (location = 4) out vec4 gSpecular;

uniform sampler2D sampler;

void main() {
    vec4 texture = texture(sampler, fragTexCoord);
    if(texture.w < 0.1) discard;
    gAmbient = vec4(texture.rgb, 1);
    gDiffuse = vec4(0, 0, 0, 1);
    gSpecular = vec4(0, 0, 0, 1);
    gPosition = fragPos;
    gNormal = vec4(fragNormal, 0);
}