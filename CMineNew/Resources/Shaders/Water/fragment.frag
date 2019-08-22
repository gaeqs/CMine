#version 400 core

in vec3 fragPos, fragNormal;
in vec2 fragTexCoords;
in vec4 fragColorFilter;

out vec4 FragColor;

uniform vec3 cameraPosition;
uniform sampler2D sampler;
uniform float viewDistanceSquared, viewDistanceOffsetSquared, waterShader;
uniform vec4 fogColor;

void main() {
    vec4 texture = texture(sampler, fragTexCoords);
    if (texture.r == texture.g && texture.r == texture.b && fragColorFilter.a > 0.5) {
        texture = fragColorFilter * texture.r;
    }

    FragColor = texture;
    
    vec3 distance = fragPos - cameraPosition;
    float lengthSquared = dot(distance, distance);
    if(waterShader > 0.5) {
        float length = 1 - lengthSquared / 100;
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