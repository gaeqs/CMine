#version 440 core

layout (location = 0) in vec3 position;

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

uniform vec3 worldPosition;

void main () {

    ivec3 blockPosition = floatBitsToInt(worldPosition);
    ivec3 relative = blockPosition - floorCameraPosition;
    vec3 relativeF = vec3(relative);
    vec3 modelRelativePosition = position + relativeF - decimalCameraPosition;

    vec4 viewPosition = view * vec4(modelRelativePosition, 0.0);
    viewPosition.w = 1;

    gl_Position = projection * viewPosition;
    gl_Position.z -= 0.0005;
}
