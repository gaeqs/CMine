#version 440 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec2 texturePosition;
layout (location = 3) in vec3 worldPosition;
layout (location = 4) in float textureIndex;
layout (location = 5) in float blockColorFilter;
layout (location = 6) in float blockLight;
layout (location = 7) in float sunlight;
layout (location = 8) in float waterLevels;

out vec3 fragPos, fragNormal;
out vec2 fragTexCoords;
out vec4 fragColorFilter;
out float fragLight;

layout (std140, binding = 0) uniform Uniforms {

    mat4 viewProjection;
    vec3 cameraPosition;
    vec3 sunlightDirection;
    float viewDistanceSquared;
    float viewDistanceOffsetSquared;
    bool waterShader;
    int millis;
    float normalizedSpriteSize;
    int spriteTextureLength;

};

void main () {
    mat4 model = mat4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, worldPosition.x, worldPosition.y, worldPosition.z, 1);
    vec4 modelPosition = model * vec4(position, 1);

    uint waterLevelsInt = floatBitsToUint(waterLevels);

    float water0 = float(waterLevelsInt >> 24);
    float water1 = float((waterLevelsInt >> 16) & 0xFF);
    float water2 = float((waterLevelsInt >> 8) & 0xFF);
    float water3 = float(waterLevelsInt & 0xFF);

    int is0 = int(position.x < 0.5 && position.z < 0.5);
    int is1 = int(position.x > 0.5 && position.z < 0.5);
    int is2 = int(position.x < 0.5 && position.z > 0.5);
    int is3 = int(position.x > 0.5 && position.z > 0.5);
    int isTop = int(position.y > 0.5);
    int isNotTop = 1 - isTop;

    float level = water0 * is0 + water1 * is1 + water2 * is2 + water3 * is3 + 1;
    level = -1 + level / 9;
    modelPosition.y += isTop * level;

    gl_Position = viewProjection * modelPosition;
    gl_Position.z += 0.0001f;

    fragPos = modelPosition.xyz;
    fragNormal = mat3(transpose(inverse(model))) * normal;

    int iIndex = int(textureIndex);
    int xIndex = iIndex / spriteTextureLength;
    vec2 minT = vec2(xIndex * normalizedSpriteSize, (iIndex % spriteTextureLength) * normalizedSpriteSize);
    vec2 maxT = minT + normalizedSpriteSize;
    vec2 size = maxT - minT;
    fragTexCoords = minT + texturePosition * size;

    uint color = floatBitsToUint(blockColorFilter);
    uint a = color & 0xFF;
    uint r = color >> 24;
    uint g = (color >> 16) & 0xFF;
    uint b = (color >> 8) & 0xFF;


    fragColorFilter = vec4(r, g, b, a) / 255.0;

    fragLight = max(blockLight, sunlight * 0.8 + sunlight *  max(0, dot(-fragNormal, sunlightDirection)) * 0.2);
}
