#version 440 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec2 texturePosition;
layout (location = 3) in vec3 worldPosition;
layout (location = 4) in float textureIndex;
layout (location = 5) in float blockColorFilter;
layout (location = 6) in float blockLight;
layout (location = 7) in float sunlight;
layout (location = 8) in float upside;

out vec3 fragPos, fragNormal;
out vec2 fragTexCoord;
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
    float height = upside > 0.5 ? 0.5 : 0;
    mat4 model = mat4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, worldPosition.x, worldPosition.y + height, worldPosition.z, 1);
    vec4 modelPosition = model * vec4(position, 1);
    gl_Position = viewProjection * modelPosition;
    fragPos = modelPosition.xyz;
    fragNormal = mat3(transpose(inverse(model))) * normal;

    vec2 minT = vec2(textureIndex * normalizedSpriteSize / spriteTextureLength, (int(textureIndex) % spriteTextureLength) * normalizedSpriteSize);
    vec2 maxT = minT + normalizedSpriteSize;

    vec2 size = maxT - minT;

    fragTexCoord = minT + texturePosition * size;

    uint color = floatBitsToUint(blockColorFilter);
    uint a = color & 0xFF;
    uint r = color >> 24;
    uint g = (color >> 16) & 0xFF;
    uint b = (color >> 8) & 0xFF;
    
    fragColorFilter = vec4(r, g, b, a) / 255.0;
    
    fragLight = max(blockLight, sunlight * 0.8 + sunlight *  max(0, dot(-fragNormal, sunlightDirection)) * 0.2);
}
