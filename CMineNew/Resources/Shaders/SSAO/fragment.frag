#version 440 core

in vec2 fragTexCoords;
out float FragColor;

uniform sampler2D gDepth;
uniform sampler2D gNormal;
uniform sampler2D gNoise;
uniform int kernelSize;
uniform float radius;
uniform float bias;

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

uniform mat4 invertedProjection;
uniform vec3 samples[64];

vec3 calculatePosition (vec2 texCoords) {
    vec4 unprojected = invertedProjection * vec4(texCoords * 2.0 - 1.0, texture2D(gDepth, texCoords).r * 2 - 1, 1);
    return unprojected.xyz / unprojected.w;
}

void main() {
    vec2 noiseScale = windowsSize / 4.0f;

    //View position
    vec3 position = calculatePosition(fragTexCoords);
    vec2 normalXY = texture2D(gNormal, fragTexCoords).rg;
    vec3 normal = vec3(normalXY, sqrt(1 - dot(normalXY, normalXY)));
    vec3 randomVec = texture2D(gNoise, fragTexCoords * noiseScale).rgb;

    vec3 tangent = normalize(randomVec - normal * dot(randomVec, normal));
    vec3 bitangent = cross(normal, tangent);
    mat3 TBN = mat3(tangent, bitangent, normal);

    float occlusion = 0.0;
    for(int i = 0; i < kernelSize; i++) {
        vec3 kernelSample = TBN * samples[i];
        kernelSample = position + kernelSample * radius;
        
        vec4 offset = projection * vec4(kernelSample, 1.0);
        offset /= offset.w;
        offset.xyz = offset.xyz * 0.5 + 0.5;
        
        float sampleDepth = calculatePosition(offset.xy).z;
        float rangeCheck = smoothstep(0.0, 1.0, radius / abs(position.z - sampleDepth));
        occlusion += (sampleDepth >= kernelSample.z + bias ? 1.0 : 0.0) * rangeCheck;
    }
    FragColor = occlusion / kernelSize;
}