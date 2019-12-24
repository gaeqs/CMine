#version 440 core

in vec2 fragPos, fragTexCoords;
out vec4 FragColor;

uniform sampler2D gAlbedo;
uniform sampler2D gDepth;
uniform sampler2D gNormal;
uniform sampler2D gBrightness;
uniform sampler2D gSsao;


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

uniform float ambientStrength;
uniform vec3 ambientColor;
uniform mat4 invertedProjection;

vec3 calculateGlobalAmbient (vec3 modelAmbientColor) {
    return ambientStrength * ambientColor * modelAmbientColor;
}

vec3 calculatePosition (vec2 texCoords) {
    float depth = texture2D(gDepth, texCoords).r * 2 - 1;
    vec2 projectedPositionXY = texCoords * 2.0 - 1.0;
    vec4 projected = vec4(projectedPositionXY.x, projectedPositionXY.y, depth, 1);
    vec4 position4 = invertedProjection * projected;
    return position4.xyz / position4.w;
}

void main() {
    vec3 albedo = texture2D(gAlbedo, fragTexCoords).rgb;
    vec2 normalXY = texture2D(gNormal, fragTexCoords).rg;

    vec3 normal = vec3(normalXY, sqrt(1 - dot(normalXY, normalXY)));
    float ambientOcclusion = 1 - texture(gSsao, fragTexCoords).r;
    vec3 brightness = clamp(texture2D(gBrightness, fragTexCoords).rgb, 0, 1);

    vec3 result = calculateGlobalAmbient(albedo) + albedo * brightness;
    
    FragColor = vec4(0.5 * result + 0.5 * result * ambientOcclusion, 1);

    vec3 position = calculatePosition(fragTexCoords);
    vec3 distance = position;
    float lengthSquared = dot(distance, distance);

    float a = clamp((viewDistanceOffsetSquared - lengthSquared) / (viewDistanceOffsetSquared - viewDistanceSquared), 0, 1);
    FragColor.a = a;
}