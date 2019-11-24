#version 440 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec2 texturePosition;

out vec2 fragTexCoord;
flat out vec4 fragColorFilter;

uniform vec2 instancePosition;
uniform vec3 instanceSize;
uniform int[6] instanceTextureIndices;
uniform mat4 model, projection;

uniform vec4 colorFilter;

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
    gl_Position = projection * (model * vec4(position * instanceSize - instanceSize / 2, 1) - vec4(0, 0, 2, 0));
    gl_Position /= gl_Position.w;
    gl_Position += vec4(instancePosition, 0, 0);
    
    int iIndex = instanceTextureIndices[gl_VertexID / 4];
    int xIndex = iIndex / spriteTextureLength;
    vec2 minT = vec2(xIndex * normalizedSpriteSize, (iIndex % spriteTextureLength) * normalizedSpriteSize);
    vec2 maxT = minT + normalizedSpriteSize;
    vec2 size = maxT - minT;
    fragTexCoord = minT + texturePosition * size;

    fragColorFilter = colorFilter;
}
