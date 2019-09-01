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

uniform float viewDistanceSquared, viewDistanceOffsetSquared, waterShader;
uniform vec4 fogColor;

vec3 calculateGlobalAmbient (vec3 modelAmbientColor) {
    return ambientStrength * ambientColor * modelAmbientColor;
}

void main() {
    vec4 ambientFull = texture2D(gAmbient, fragTexCoords);
    vec3 modelAmbientColor = ambientFull.rgb;

    float opacity = ambientFull.a;

    vec3 position = texture2D(gPosition, fragTexCoords).rgb;

    vec3 diffuseColor = texture2D(gDiffuse, fragTexCoords).rgb;
    vec3 specularColor = texture2D(gSpecular, fragTexCoords).rgb;
    vec3 ambientBrightness = texture2D(gAmbientBrightness, fragTexCoords).rgb;
    vec3 diffuseBrigtness = texture2D(gDiffuseBrightness, fragTexCoords).rgb;
    vec3 specularBrightness = texture2D(gSpecularBrightness, fragTexCoords).rgb;

    //0.4 is temporal.
    vec3 result = calculateGlobalAmbient(modelAmbientColor)
    + modelAmbientColor * ambientBrightness + diffuseColor * diffuseBrigtness + specularColor * specularBrightness * 0.4;

    FragColor = vec4(result, 1);

    vec3 distance = position - cameraPosition;
    float lengthSquared = dot(distance, distance);

    if (waterShader > 0.5) {
        float length = 1 - lengthSquared / 1000;
        FragColor *= vec4(0.3, 0.3, 0.7, 1) * length;
    }
    else if (lengthSquared > viewDistanceSquared) {
        if (lengthSquared > viewDistanceOffsetSquared) {
            FragColor = fogColor;
        }
        else {
            float a = 1 - (viewDistanceOffsetSquared - lengthSquared) / (viewDistanceOffsetSquared - viewDistanceSquared);
            FragColor = mix(FragColor, fogColor, a);
        }
    }
}