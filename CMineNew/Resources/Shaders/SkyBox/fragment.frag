#version 400 core

in vec3 fragTexCoord;

layout (location = 0) out vec3 gPosition;
layout (location = 1) out vec4 gNormal;
layout (location = 2) out vec4 gAmbient;
layout (location = 3) out vec4 gDiffuse;
layout (location = 4) out vec4 gSpecular;

uniform samplerCube skyBox;

void main() {
    gAmbient = texture(skyBox, fragTexCoord);
    gDiffuse = vec4(0);
    gSpecular = vec4(0);
    gPosition = vec3(0);
    gNormal = vec4(0); // normal = Zero represents that the pixel is from the background.
}