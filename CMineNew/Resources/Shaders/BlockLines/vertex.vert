#version 400 core

layout (location = 0) in vec3 position;

out vec3 fragPos;

uniform mat4 viewProjection;
uniform vec3 worldPosition;

void main () {
    mat4 model = mat4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, worldPosition.x, worldPosition.y, worldPosition.z, 1);
    vec4 modelPosition = model * vec4(position, 1);
    gl_Position = viewProjection * modelPosition;
    gl_Position.z -= 0.001;
    fragPos = modelPosition.xyz;
}
