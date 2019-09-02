#version 400 core

in vec2 fragTexCoords;
out vec4 FragColor;

uniform sampler2D gAmbient;
uniform sampler2D gDiffuse;
uniform sampler2D gSpecular;
uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D gAmbientBrightness;
uniform sampler2D gDiffuseBrightness;
uniform sampler2D gSpecularBrightness;
uniform samplerCube skyBox;

uniform vec3 cameraPosition;
uniform float ambientStrength;
uniform vec3 ambientColor;

uniform float viewDistanceSquared, viewDistanceOffsetSquared, waterShader;

vec3 calculateGlobalAmbient (vec3 modelAmbientColor) {
    return ambientStrength * ambientColor * modelAmbientColor;
}

void main() {
    vec4 ambientFull = texture2D(gAmbient, fragTexCoords);
    vec3 modelAmbientColor = ambientFull.rgb;

    vec3 normal = texture2D(gNormal, fragTexCoords).rgb;

    if(normal.x == 0 && normal.y == 0 && normal.z == 0) {
        FragColor = vec4(modelAmbientColor, 1);
    }
    else {
        float opacity = ambientFull.a;

        vec3 position = texture2D(gPosition, fragTexCoords).rgb;

        vec3 diffuseColor = texture2D(gDiffuse, fragTexCoords).rgb;
        vec3 specularColor = texture2D(gSpecular, fragTexCoords).rgb;
        vec3 ambientBrightness = texture2D(gAmbientBrightness, fragTexCoords).rgb;
        vec3 diffuseBrigtness = texture2D(gDiffuseBrightness, fragTexCoords).rgb;
        vec3 specularBrightness = texture2D(gSpecularBrightness, fragTexCoords).rgb;

        //0.4 is temporal.
        vec3 result = calculateGlobalAmbient(modelAmbientColor) + modelAmbientColor * ambientBrightness + diffuseColor * diffuseBrigtness + specularColor * specularBrightness * 0.4;

        FragColor = vec4(result, 1);

        vec3 distance = position - cameraPosition;
        float lengthSquared = dot(distance, distance);

        if (waterShader > 0.5) {
            float length = 1 - lengthSquared / 1000;
            FragColor *= vec4(0.3, 0.3, 0.7, 1) * length;
        }
        else if (lengthSquared > viewDistanceSquared) {
            
            vec4 color = texture(skyBox, distance);
            
            if (lengthSquared > viewDistanceOffsetSquared) {
                FragColor = color;
            }
            else {
                float a = 1 - (viewDistanceOffsetSquared - lengthSquared) / (viewDistanceOffsetSquared - viewDistanceSquared);
                FragColor = mix(FragColor, color, a);
            }
        }
    }
}