#version 400 core

in vec2 fragTexCoords;
out vec4 FragColor;

uniform sampler2D gAmbient;
uniform sampler2D gDiffuse;
uniform sampler2D gSpecular;
uniform sampler2D gPosition;
uniform sampler2D gAmbientBrightness;
uniform sampler2D gDiffuseBrightness;
uniform sampler2D gSpecularBrightness;

uniform vec3 cameraPosition;
uniform float ambientStrength;
uniform vec3 ambientColor;
uniform float waterShader;

vec3 calculateGlobalAmbient (vec3 modelAmbientColor) {
    return ambientStrength * ambientColor * modelAmbientColor;
}

vec3 toLdr (vec3 color) {
    float exposure = 1;
    float gamma = 1; 
    return pow(vec3(1) - exp(-color * exposure), vec3(1/gamma));
}

void main() {
    vec4 ambientFull = texture2D(gAmbient, fragTexCoords);
    vec3 modelAmbientColor = ambientFull.rgb;
    float opacity = ambientFull.a;

    vec3 diffuseColor = texture2D(gDiffuse, fragTexCoords).rgb;
    vec3 specularColor = texture2D(gSpecular, fragTexCoords).rgb;
    vec3 ambientBrightness = texture2D(gAmbientBrightness, fragTexCoords).rgb;
    vec3 diffuseBrigtness = texture2D(gDiffuseBrightness, fragTexCoords).rgb;
    vec3 specularBrightness = texture2D(gSpecularBrightness, fragTexCoords).rgb;

    vec3 result = calculateGlobalAmbient(modelAmbientColor) + modelAmbientColor * ambientBrightness +
    diffuseColor * diffuseBrigtness + specularColor * specularBrightness;

    FragColor = vec4(toLdr(result), 1);
    if (waterShader > 0.5) {
        vec3 position = texture2D(gPosition, fragTexCoords).rgb;
        float length = 1 - length(position - cameraPosition) / 10;
        FragColor *= vec4(0.5, 0.5, 1, 1) * length;
    }
}