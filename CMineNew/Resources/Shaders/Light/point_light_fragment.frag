#version 400 core

struct PointLight {
    vec3 position;
    vec3 ambientColor;
    vec3 diffuseColor;
    vec3 specularColor;

    float constantAttenuation;
    float linearAttenuation;
    float quadraticAttenuation;
};


in vec2 fragTexCoords;

layout (location = 5) out vec3 gAmbientBrightness;
layout (location = 6) out vec3 gDiffuseBrightness;
layout (location = 7) out vec3 gSpecularBrightness;

uniform sampler2D gPosition;
uniform sampler2D gNormal;

uniform vec3 cameraPosition;
uniform PointLight light;

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

        vec3 lightPosition = light.position - position;
        float lightDistance = length(lightPosition);
        vec3 lightDirection = lightPosition / lightDistance;

        float diffuseStrength = max(dot(normal, lightDirection), 0.0);

        vec3 reflectDirection = reflect(-lightDirection, normal);
        float specularStrenth = pow(max(dot(direction, reflectDirection), 0.0), specularWeight);
        
        float attenuation = 1.0 / (light.constantAttenuation + light.linearAttenuation * lightDistance + light.quadraticAttenuation * (lightDistance * lightDistance));

        gAmbientBrightness = light.ambientColor * attenuation;
        gDiffuseBrightness = light.diffuseColor  * diffuseStrength * attenuation;
        gSpecularBrightness = light.specularColor * specularStrenth * attenuation;
    }
}