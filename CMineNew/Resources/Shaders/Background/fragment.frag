#version 400 core

uniform vec4 background;

layout (location = 0) out vec3 gPosition;
layout (location = 1) out vec4 gNormal;
layout (location = 2) out vec4 gAmbient;
layout (location = 3) out vec4 gDiffuse;
layout (location = 4) out vec4 gSpecular;

void main() {
    gAmbient = background;
    gDiffuse = vec4(0);
    gSpecular = vec4(0);
    gPosition = vec3(0);
    gNormal = vec4(0); // normal = Zero represents that the pixel is from the background.
}