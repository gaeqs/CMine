#version 400 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec2 texturePosition;
layout (location = 3) in vec3 worldPosition;
layout (location = 4) in vec4 textureArea;
layout (location = 5) in vec4 blockColorFilter;
layout (location = 6) in float blockLight;
layout (location = 7) in vec4 waterLevels;

out vec3 fragPos, fragNormal;
out vec2 fragTexCoords;
out vec4 fragColorFilter;
out float fragLight;

uniform mat4 viewProjection;

float getWaterLevel () {
    if (position.x < 0.5 && position.z < 0.5) {
        return waterLevels[0];
    }
    if (position.x > 0.5 && position.z < 0.5) {
        return waterLevels[1];
    }
    if (position.x < 0.5 && position.z > 0.5) {
        return waterLevels[2];
    }
    return waterLevels[3];
}

void main () {
    mat4 model = mat4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, worldPosition.x, worldPosition.y, worldPosition.z, 1);
    vec4 modelPosition = model * vec4(position, 1);
    
    if (position.x == 0) {
        modelPosition.x -= 0.0003;
    }
    if (position.y == 0) {
        modelPosition.y -= 0.0003;
    }
    if (position.z == 0) {
        modelPosition.z -= 0.0003;
    }

    if (position.x == 1) {
        modelPosition.x += 0.0003;
    }
    if (position.y == 1) {
        modelPosition.y += 0.0003;
    }
    if (position.z == 1) {
        modelPosition.z += 0.0003;
    }

    if (position.y > 0.5) {
        float level = getWaterLevel() + 1;
        modelPosition.y += -1 + level / 9;
    }

    gl_Position = viewProjection * modelPosition;

    fragPos = modelPosition.xyz;
    fragNormal = mat3(transpose(inverse(model))) * normal;
    fragTexCoords = vec2(texturePosition.x > 0 ? textureArea.x : textureArea.z, texturePosition.y > 0 ? textureArea.w : textureArea.y);
    fragColorFilter = blockColorFilter;
    fragLight = blockLight;
}
