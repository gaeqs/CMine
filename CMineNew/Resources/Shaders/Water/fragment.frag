#version 400 core

in vec3 fragPos, fragNormal;
in vec2 fragTexCoords;
in vec4 fragColorFilter;

out vec4 FragColor;

uniform vec3 cameraPosition;
uniform sampler2D sampler;
uniform float viewDistanceSquared, viewDistanceOffsetSquared, waterShader;

uniform samplerCube skyBox;

void main() {
    vec4 ambient = texture(sampler, fragTexCoords);
    if (ambient.r == ambient.g && ambient.r == ambient.b && fragColorFilter.a > 0.5) {
        ambient = fragColorFilter * ambient.r;
    }

    FragColor = ambient;
    
    vec3 distance = fragPos - cameraPosition;
    float lengthSquared = dot(distance, distance);
    if(waterShader > 0.5) {
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