#version 440 core

in vec3 fragPos, fragNormal;
in vec2 fragTexCoords;
in vec4 fragColorFilter;
in float fragLight;

out vec4 FragColor;

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

uniform sampler2D sampler;
uniform samplerCube skyBox;

void main() {
    vec4 ambient = texture(sampler, fragTexCoords);
    if (ambient.r == ambient.g && ambient.r == ambient.b && fragColorFilter.a > 0.5) {
        ambient = fragColorFilter * ambient.r;
    }

    float light = fragLight * 0.8 + 0.2;
    FragColor = ambient * light;
    
    vec3 distance = fragPos - cameraPosition;
    float lengthSquared = dot(distance, distance);

    vec4 color = texture(skyBox, distance);
    float a = clamp(1 - (viewDistanceOffsetSquared - lengthSquared) / (viewDistanceOffsetSquared - viewDistanceSquared), 0, 1);
    FragColor = mix(FragColor, color, a);
}