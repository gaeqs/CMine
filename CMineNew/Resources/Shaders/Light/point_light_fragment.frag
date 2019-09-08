#version 400 core

struct PointLight {
    vec3 position;          // 16
    vec3 ambientColor;      // 16
    vec3 diffuseColor;      // 16
    vec3 specularColor;     // 16

    float constantAttenuation;  // 4
    float linearAttenuation;    // 4
    float quadraticAttenuation; // 4
    
    // 64 + 12 = 76 => 80 bytes => 20 floats
};

in vec2 fragPosition, fragTexCoords;

layout (location = 2) out vec3 gBrightness;

uniform sampler2D gDepth;
uniform sampler2D gNormal;


uniform vec3 cameraPosition;
uniform mat4 invertedViewProjection;
uniform int lightAmount;
layout (std140) uniform LightsBlock {
    PointLight lights[200];
};

void main() {
    vec4 normalFull = texture(gNormal, fragTexCoords);
    vec3 normal = normalFull.rgb;
    
    if(normal.x == 0 && normal.y == 0 && normal.z == 0) {
        gBrightness = vec3(1);
    }
    else {
        float depth = texture2D(gDepth, fragTexCoords).r * 2 - 1;
        vec4 proyectedPosition = vec4(fragPosition, depth, 1);
        vec4 position4 = invertedViewProjection * proyectedPosition;
        vec3 position = position4.xyz / position4.w;
        
        float specularWeight = normalFull.a;
        vec3 direction = normalize(cameraPosition -  position);
        
        vec3 ambient = vec3(0);
        vec3 diffuse = vec3(0);
        vec3 specular = vec3(0);
        for(int i = 0; i < lightAmount; i++) {
            PointLight light = lights[i];

            vec3 lightPosition = light.position - position;
            float lightDistance = length(lightPosition);
            vec3 lightDirection = lightPosition / lightDistance;

            float diffuseStrength = max(dot(normal, lightDirection), 0.0);

            vec3 reflectDirection = reflect(-lightDirection, normal);
            float specularStrenth = pow(max(dot(direction, reflectDirection), 0.0), specularWeight);

            float attenuation = 1.0 / (light.constantAttenuation + light.linearAttenuation * lightDistance + light.quadraticAttenuation * (lightDistance * lightDistance));

            ambient += light.ambientColor * attenuation;
            diffuse += light.diffuseColor  * diffuseStrength * attenuation;
            specular += light.specularColor * specularStrenth * attenuation;
        }
        gBrightness = ambient + diffuse + specular;
    }
    
}