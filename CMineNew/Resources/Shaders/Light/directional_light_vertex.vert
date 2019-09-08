#version 400 core

layout (location = 0) in vec2 position;
layout (location = 1) in vec2 texturePosition;
layout (location = 2) in vec3 lightDirection;
layout (location = 3) in vec3 ambientColor;
layout (location = 4) in vec3 diffuseColor;
layout (location = 5) in vec3 specularColor;

out vec2 fragPosition, fragTexCoords;
out vec3 fragLightDirection, fragAmbientColor, fragDiffuseColor, fragSpecularColor;

void main () {
    gl_Position = vec4(position.xy, 0, 1);
    fragPosition = position.xy;
    fragTexCoords = texturePosition;

    fragLightDirection = lightDirection;
    fragAmbientColor = ambientColor;
    fragDiffuseColor = diffuseColor;
    fragSpecularColor = specularColor;
}
