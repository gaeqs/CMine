#version 440 core

in vec2 fragPos, fragTexCoords;
out vec4 FragColor;

uniform sampler2D gAlbedo;
uniform sampler2D gDepth;
uniform sampler2D gNormal;
uniform sampler2D gBrightness;
uniform sampler2D gSsao;
uniform samplerCube skyBox;


layout (std140, binding = 0) uniform Uniforms {

    mat4 viewProjection;
    mat4 view;
    mat4 projection;
    vec3 cameraPosition;
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
uniform mat4 invertedViewProjection;

vec3 calculateGlobalAmbient (vec3 modelAmbientColor) {
    return ambientStrength * ambientColor * modelAmbientColor;
}

vec3 calculatePosition () {
    float depth = texture2D(gDepth, fragTexCoords).r * 2 - 1;
    vec4 projected = vec4(fragPos, depth, 1);
    vec4 position4 = invertedViewProjection * projected;
    return position4.xyz / position4.w;
}

void main() {
    vec3 albedo = texture2D(gAlbedo, fragTexCoords).rgb;
    vec2 normalXY = texture2D(gNormal, fragTexCoords).rg;
    
    if (normalXY.x == 2 && normalXY.y == 2) {
        FragColor = vec4(albedo, 1);
    }
    else {
        //vec3 normal = vec3(normalXY, sqrt(1 - dot(normalXY, normalXY)));
        vec3 position = calculatePosition();
        float ambientOcclusion = texture(gSsao, fragTexCoords).r;
        vec3 brightness = clamp(texture2D(gBrightness, fragTexCoords).rgb, 0, 1);

        vec3 result = calculateGlobalAmbient(albedo) + albedo * brightness;

        FragColor = vec4(result * ambientOcclusion, 1);

        vec3 distance = position - cameraPosition;
        float lengthSquared = dot(distance, distance);

        vec4 color = texture(skyBox, distance);
        float a = clamp(1 - (viewDistanceOffsetSquared - lengthSquared) / (viewDistanceOffsetSquared - viewDistanceSquared), 0, 1);
        FragColor = mix(FragColor, color, a);
        FragColor = vec4(ambientOcclusion, ambientOcclusion, ambientOcclusion, 1);
    }

}