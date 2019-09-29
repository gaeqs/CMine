#version 440 core


in vec2 fragTexCoords;
in vec3 fragLightPosition, fragLightDirection, fragAmbientColor, fragDiffuseColor, fragSpecularColor;
in float fragConstantAttenuation, fragLinearAttenuation, fragQuadraticAttenuation;
in float fragCutOff, fragOuterCutOff;

layout (location = 2) out vec3 gBrightness;

uniform sampler2D gDepth;
uniform sampler2D gNormal;

uniform vec3 cameraPosition;

void main() {

    vec4 normalFull = texture(gNormal, fragTexCoords);
    vec3 normal = normalFull.rgb;

    if (normal.x == 0 && normal.y == 0 && normal.z == 0) {
        gBrightness = vec3(1);
    }
    else {
        vec3 position = texture(gDepth, fragTexCoords).rgb;
        float specularWeight = normalFull.a;
        vec3 direction = normalize(cameraPosition -  position);

        vec3 lightDirection = normalize(fragLightPosition - position);

        float diff = max(dot(normal, lightDirection), 0.0);

        vec3 reflectDir = reflect(-lightDirection, normal);
        float spec = pow(max(dot(direction, reflectDir), 0.0), specularWeight);

        float distance    = length(fragLightPosition - position);
        float attenuation = 1.0 / (fragConstantAttenuation + fragLinearAttenuation * distance + fragQuadraticAttenuation * (distance * distance));

        float theta = dot(lightDirection, normalize(-fragLightDirection));
        float epsilon = (fragCutOff - fragOuterCutOff);
        float intensity = clamp((theta - fragOuterCutOff) / epsilon, 0.0, 1.0);

        float intensityAttenuation = intensity * attenuation;

        gBrightness = fragAmbientColor * intensityAttenuation + fragDiffuseColor  * diff * intensityAttenuation + fragSpecularColor * spec * intensityAttenuation;
    }
}