#version 400 core

in vec2 fragPosition, fragTexCoords;
in vec3 fragLightDirection, fragAmbientColor, fragDiffuseColor, fragSpecularColor;

layout (location = 2) out vec3 gBrightness;

uniform sampler2D gDepth;
uniform sampler2D gNormal;

uniform mat4 invertedViewProjection;
uniform vec3 cameraPosition;

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

        vec3 lightDirection = normalize(-fragLightDirection);

        float diff = max(dot(normal, lightDirection), 0.0);

        vec3 reflectDirection = reflect(-lightDirection, normal);
        float spec = pow(max(dot(direction, reflectDirection), 0.0), specularWeight);
        
        gBrightness = fragAmbientColor + fragDiffuseColor  * diff + fragSpecularColor * spec;
    }
}