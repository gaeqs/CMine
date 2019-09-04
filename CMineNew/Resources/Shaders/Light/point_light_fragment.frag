#version 400 core

in vec2 fragTexCoords;
in vec3 fragLightPosition, fragAmbientColor, fragDiffuseColor, fragSpecularColor;
in float fragConstantAttenuation, fragLinearAttenuation, fragQuadraticAttenuation;

layout (location = 5) out vec3 gAmbientBrightness;
layout (location = 6) out vec3 gDiffuseBrightness;
layout (location = 7) out vec3 gSpecularBrightness;

uniform sampler2D gPosition;
uniform sampler2D gNormal;

uniform vec3 cameraPosition;

void main() {
    vec4 normalFull = texture(gNormal, fragTexCoords);
    vec3 normal = normalFull.rgb;
    
    if(normal.x == 0 && normal.y == 0 && normal.z == 0) {
        gAmbientBrightness = vec3(1);
        gDiffuseBrightness = vec3(1);
        gSpecularBrightness = vec3(1);
    }
    else {
        vec3 position = texture(gPosition, fragTexCoords).rgb;
        float specularWeight = normalFull.a;
        vec3 direction = normalize(cameraPosition -  position);

        vec3 lightPosition = fragLightPosition - position;
        float lightDistance = length(lightPosition);
        vec3 lightDirection = lightPosition / lightDistance;

        float diffuseStrength = max(dot(normal, lightDirection), 0.0);

        vec3 reflectDirection = reflect(-lightDirection, normal);
        float specularStrenth = pow(max(dot(direction, reflectDirection), 0.0), specularWeight);
        
        float attenuation = 1.0 / (fragConstantAttenuation + fragLinearAttenuation * lightDistance + fragQuadraticAttenuation * (lightDistance * lightDistance));

        gAmbientBrightness = fragAmbientColor * attenuation;
        gDiffuseBrightness = fragDiffuseColor  * diffuseStrength * attenuation;
        gSpecularBrightness = fragSpecularColor * specularStrenth * attenuation;
    }
}