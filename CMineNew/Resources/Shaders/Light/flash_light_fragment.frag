#version 400 core

struct FlashLight {
    vec3 position;
    vec3 direction;
    vec3 ambientColor;
    vec3 diffuseColor;
    vec3 specularColor;

    float constantAttenuation;
    float linearAttenuation;
    float quadraticAttenuation;

    float cutOff;
    float outerCutOff;
};


in vec2 fragTexCoords;

layout (location = 5) out vec3 gAmbientBrightness;
layout (location = 6) out vec3 gDiffuseBrightness;
layout (location = 7) out vec3 gSpecularBrightness;

uniform sampler2D gPosition;
uniform sampler2D gNormal;

uniform vec3 cameraPosition;
uniform FlashLight light;


void main() {

    vec4 normalFull = texture(gNormal, fragTexCoords);
    vec3 normal = normalFull.rgb;

    if (normal.x == 0 && normal.y == 0 && normal.z == 0) {
        gAmbientBrightness = vec3(1);
        gDiffuseBrightness = vec3(1);
        gSpecularBrightness = vec3(1);
    }
    else {
        vec3 position = texture(gPosition, fragTexCoords).rgb;
        float specularWeight = normalFull.a;
        vec3 direction = normalize(cameraPosition -  position);

        vec3 lightDirection = normalize(light.position - position);

        float diff = max(dot(normal, lightDirection), 0.0);

        vec3 reflectDir = reflect(-lightDirection, normal);
        float spec = pow(max(dot(direction, reflectDir), 0.0), specularWeight);

        float distance    = length(light.position - position);
        float attenuation = 1.0 / (light.constantAttenuation + light.linearAttenuation * distance + light.quadraticAttenuation * (distance * distance));

        float theta = dot(lightDirection, normalize(-light.direction));
        float epsilon = (light.cutOff - light.outerCutOff);
        float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);

        float intensityAttenuation = intensity * attenuation;

        gAmbientBrightness = light.ambientColor * intensityAttenuation;
        gDiffuseBrightness = light.diffuseColor  * diff * intensityAttenuation;
        gSpecularBrightness = light.specularColor * spec * intensityAttenuation;
    }
}