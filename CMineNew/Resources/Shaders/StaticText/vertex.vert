#version 400 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec2 texturePosition;

layout (location = 3) in vec2 minPosition;
layout (location = 4) in vec2 size;
layout (location = 5) in vec4 color;
layout (location = 6) in vec4 textureCoords;

out vec2 fragTextCoord;
out vec4 fragColor;

uniform mat4 model, view, projection;

void main () {
    gl_Position = vec4(position.xy * size, 0.9f, 1.0) + vec4(minPosition, 0, 0);
    fragColor = color;
    fragTextCoord = vec2(texturePosition.x > 0 ? textureCoords.z : textureCoords.x, texturePosition.y > 0 ? textureCoords.w : textureCoords.y);
}