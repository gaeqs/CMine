#version 400 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec2 texturePosition;
layout (location = 3) in vec3 worldPosition;
layout (location = 4) in vec4 textureArea;
layout (location = 5) in vec4 blockColorFilter;
layout (location = 6) in float blockLight;
layout (location = 7) in float upside;

out vec3 fragPos, fragNormal;
out vec2 fragTexCoord;
out vec4 fragColorFilter;
out float fragLight;

uniform mat4 viewProjection;

void main () {
    float height = upside > 0.5 ? 0.5 : 0;
    mat4 model = mat4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, worldPosition.x, worldPosition.y + height, worldPosition.z, 1);
    vec4 modelPosition = model * vec4(position, 1);
    gl_Position = viewProjection * modelPosition;
    fragPos = modelPosition.xyz;
    fragNormal = mat3(transpose(inverse(model))) * normal;

    vec2 min = textureArea.xy;
    vec2 max = textureArea.zw;
    vec2 size = max - min;

    fragTexCoord = min + texturePosition * size;
    fragColorFilter = blockColorFilter;
    fragLight = blockLight;
}
