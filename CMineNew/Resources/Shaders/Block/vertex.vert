#version 440 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec2 texturePosition;
layout (location = 3) in vec3 worldPosition;
layout (location = 4) in float textureIndex;
layout (location = 5) in float blockColorFilter;
layout (location = 6) in float blockLight;
layout (location = 7) in float sunlight;

out vec3 fragPos, fragNormal;
out vec2 fragTexCoord;
flat out vec4 fragColorFilter;
out float fragLight;

layout (std140, binding = 0) uniform Uniforms {

    mat4 viewProjection;
    mat4 view;
    mat4 projection;

    vec3 cameraPosition;
    ivec3 floorCameraPosition;
    vec3 decimalCameraPosition;

    vec3 sunlightDirection;
    float viewDistanceSquared;
    float viewDistanceOffsetSquared;
    bool waterShader;
    int millis;
    float normalizedSpriteSize;
    int spriteTextureLength;
    vec2 windowsSize;
};

void main () {
    
    ivec3 blockPosition = floatBitsToInt(worldPosition);
    ivec3 relative = blockPosition - floorCameraPosition;
    vec3 relativeF = vec3(relative);
    vec3 modelRelativePosition = position + relativeF - decimalCameraPosition;
    
    vec4 viewPosition = view * vec4(modelRelativePosition, 0.0);
    viewPosition.w = 1;
    
    gl_Position = projection * viewPosition;
    
    
    fragNormal = mat3(view) * normal;

    int iIndex = int(textureIndex);
    int xIndex = iIndex / spriteTextureLength;
    vec2 minT = vec2(xIndex * normalizedSpriteSize, (iIndex % spriteTextureLength) * normalizedSpriteSize);
    vec2 maxT = minT + normalizedSpriteSize;
    vec2 size = maxT - minT;
    fragTexCoord = minT + texturePosition * size;

    uint color = floatBitsToUint(blockColorFilter);
    uint a = color & 0xFF;
    uint r = color >> 24;
    uint g = (color >> 16) & 0xFF;
    uint b = (color >> 8) & 0xFF;


    fragColorFilter = vec4(r, g, b, a) / 255.0;
    fragLight = max(blockLight, sunlight * 0.8 + sunlight *  max(0, dot(-fragNormal, mat3(view) * sunlightDirection)) * 0.2);
}
