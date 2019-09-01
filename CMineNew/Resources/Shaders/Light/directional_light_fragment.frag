#version 400 core

struct DirectionalLight {
    vec3 direction;
    vec3 ambientColor;
    vec3 diffuseColor;
    vec3 specularColor;
};

in vec2 fragTexCoords;

layout (location = 5) out vec3 gAmbientBrightness;
layout (location = 6) out vec3 gDiffuseBrightness;
layout (location = 7) out vec3 gSpecularBrightness;

uniform sampler2D gPosition;
uniform sampler2D gNormal;

uniform vec3 cameraPosition;
uniform DirectionalLight light;

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

        vec3 lightDirection = normalize(-light.direction);

        float diff = max(dot(normal, lightDirection), 0.0);

        vec3 reflectDirection = reflect(-lightDirection, normal);
        float spec = pow(max(dot(direction, reflectDirection), 0.0), specularWeight);

        gAmbientBrightness = light.ambientColor;
        gDiffuseBrightness = light.diffuseColor  * diff;
        gSpecularBrightness = light.specularColor * spec;
    }
}