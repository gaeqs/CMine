#version 400 core

layout (location = 0) in vec2 position;
layout (location = 1) in vec2 texturePosition;
layout (location = 2) in vec3 lightPosition;
layout (location = 3) in vec3 ambientColor;
layout (location = 4) in vec3 diffuseColor;
layout (location = 5) in vec3 specularColor;
layout (location = 6) in float constantAttenuation;
layout (location = 7) in float linearAttenuation;
layout (location = 8) in float quadraticAttenuation;

out vec2 fragTexCoords;
out vec3 fragLightPosition, fragAmbientColor, fragDiffuseColor, fragSpecularColor;
out float fragConstantAttenuation, fragLinearAttenuation, fragQuadraticAttenuation;

void main () {
    gl_Position = vec4(position.xy, 0, 1);
    fragTexCoords = texturePosition;

    fragLightPosition = lightPosition;
    fragAmbientColor = ambientColor;
    fragDiffuseColor = diffuseColor;
    fragSpecularColor = specularColor;
    
    fragConstantAttenuation = constantAttenuation;
    fragLinearAttenuation = linearAttenuation;
    fragQuadraticAttenuation = quadraticAttenuation;
}
