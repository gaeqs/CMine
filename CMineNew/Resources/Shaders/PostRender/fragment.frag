#version 400 core

in vec2 fragTexCoords;
out vec4 FragColor;

uniform sampler2D gAmbient;
uniform sampler2D gDiffuse;
uniform sampler2D gSpecular;
uniform sampler2D gAmbientBrightness;
uniform sampler2D gDiffuseBrightness;
uniform sampler2D gSpecularBrightness;

uniform float ambientStrength;
uniform vec3 ambientColor;

vec3 calculateGlobalAmbient (vec3 modelAmbientColor) {
    return ambientStrength * ambientColor * modelAmbientColor;
}

vec3 toLdr (vec3 color) {
    float exposure = 1;
    float gamma = 1;
    return pow(vec3(1) - exp(-color * exposure), vec3(1/gamma));
}

void main() {
    vec4 ambientFull = texture(gAmbient, fragTexCoords);
    vec3 modelAmbientColor = ambientFull.rgb;
    float opacity = ambientFull.a;

    vec3 diffuseColor = texture(gDiffuse, fragTexCoords).rgb;
    vec3 specularColor = texture(gSpecular, fragTexCoords).rgb;
    vec3 ambientBrightness = texture(gAmbientBrightness, fragTexCoords).rgb;
    vec3 diffuseBrigtness = texture(gDiffuseBrightness, fragTexCoords).rgb;
    vec3 specularBrightness = texture(gSpecularBrightness, fragTexCoords).rgb;

    vec3 result = calculateGlobalAmbient(modelAmbientColor) + modelAmbientColor * ambientBrightness +
    diffuseColor * diffuseBrigtness + specularColor * specularBrightness;

    FragColor = vec4(toLdr(result), 1);

}