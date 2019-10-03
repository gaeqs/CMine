#version 440 core

in vec2 fragPos, fragTexCoords;
out vec4 FragColor;

uniform sampler2D gAlbedo;
uniform sampler2D gDepth;
uniform sampler2D gNormal;
uniform sampler2D gBrightness;
uniform samplerCube skyBox;

layout (std140, binding = 0) uniform Uniforms {

    mat4 viewProjection;
    vec3 cameraPosition;
    vec3 sunlightDirection;
    float viewDistanceSquared;
    float viewDistanceOffsetSquared;
    bool waterShader;

};

uniform float ambientStrength;
uniform vec3 ambientColor;
uniform mat4 invertedViewProjection;

vec3 calculateGlobalAmbient (vec3 modelAmbientColor) {
    return ambientStrength * ambientColor * modelAmbientColor;
}

void main() {
    vec4 albedoFull = texture2D(gAlbedo, fragTexCoords);
    vec3 normal = texture2D(gNormal, fragTexCoords).rgb;

    vec3 albedo = albedoFull.rgb;
    float colorFilter = albedoFull.a;

    if (normal.x == 0 && normal.y == 0 && normal.z == 0) {
        FragColor = vec4(albedo, 1);
    }
    else {

        if (albedo.r == albedo.g && albedo.r == albedo.b) {
            uint color = floatBitsToUint(colorFilter);
            uint a = color & 0xFF;
            if(a > 100) {
                uint r = color >> 24;
                uint g = (color >> 16) & 0xFF;
                uint b = (color >> 8) & 0xFF;
                albedo = vec3(r, g, b) * albedo.r  / 255.0;
            }
        }

        float depth = texture2D(gDepth, fragTexCoords).r * 2 - 1;
        vec4 projected = vec4(fragPos, depth, 1);
        vec4 position4 = invertedViewProjection * projected;
        vec3 position = position4.xyz / position4.w;

        vec3 brightness = texture2D(gBrightness, fragTexCoords).rgb;

        vec3 result = calculateGlobalAmbient(albedo) + albedo * brightness;

        FragColor = vec4(result, 1);

        vec3 distance = position - cameraPosition;
        float lengthSquared = dot(distance, distance);
        float length = 1 - lengthSquared / 1000;
        FragColor *= vec4(0.3, 0.3, 0.7, 1) * length;
    }
}