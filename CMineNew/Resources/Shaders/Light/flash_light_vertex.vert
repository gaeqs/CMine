#version 400 core

layout (location = 0) in vec2 position;
layout (location = 1) in vec2 texturePosition;
layout (location = 2) in vec3 lightPosition;
layout (location = 3) in vec3 lightDirection;
layout (location = 4) in vec3 ambientColor;
layout (location = 5) in vec3 diffuseColor;
layout (location = 6) in vec3 specularColor;
layout (location = 7) in float constantAttenuation;
layout (location = 8) in float linearAttenuation;
layout (location = 9) in float quadraticAttenuation;
layout (location = 10) in float cutOff;
layout (location = 11) in float outerCutOff;

out vec2 fragTexCoords;
out vec3 fragLightPosition, fragLightDirection, fragAmbientColor, fragDiffuseColor, fragSpecularColor;
out float fragConstantAttenuation, fragLinearAttenuation, fragQuadraticAttenuation;
out float fragCutOff, fragOuterCutOff;

void main () {
    gl_Position = vec4(position.xy, 0, 1);
    fragTexCoords = texturePosition;

    fragLightPosition = lightPosition;
    fragLightDirection = lightDirection;
    fragAmbientColor = ambientColor;
    fragDiffuseColor = diffuseColor;
    fragSpecularColor = specularColor;

    fragConstantAttenuation = constantAttenuation;
    fragLinearAttenuation = linearAttenuation;
    fragQuadraticAttenuation = quadraticAttenuation;

    fragCutOff = cutOff;
    fragOuterCutOff = fragOuterCutOff;
}
