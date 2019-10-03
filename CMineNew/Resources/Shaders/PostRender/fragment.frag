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
    vec3 modelAmbientColor = texture2D(gAlbedo, fragTexCoords).rgb;
    vec2 normalXY = texture2D(gNormal, fragTexCoords).rg;
    
    if (normalXY.x == 2 && normalXY.y == 2) {
        FragColor = vec4(modelAmbientColor, 1);
    }
    else {
        vec3 normal = vec3(normalXY, sqrt(1 - dot(normalXY, normalXY)));
        float depth = texture2D(gDepth, fragTexCoords).r * 2 - 1;
        vec4 projected = vec4(fragPos, depth, 1);
        vec4 position4 = invertedViewProjection * projected;
        vec3 position = position4.xyz / position4.w;

        vec3 brightness = clamp(texture2D(gBrightness, fragTexCoords).rgb, 0, 1);

        vec3 result = calculateGlobalAmbient(modelAmbientColor) + modelAmbientColor * brightness;

        FragColor = vec4(result, 1);

        vec3 distance = position - cameraPosition;
        float lengthSquared = dot(distance, distance);

        vec4 color = texture(skyBox, distance);
        float a = clamp(1 - (viewDistanceOffsetSquared - lengthSquared) / (viewDistanceOffsetSquared - viewDistanceSquared), 0, 1);
        FragColor = mix(FragColor, color, a);
    }

}