#version 400 core

in vec3 fragPos, fragNormal;
in vec2 fragTexCoord;
in vec4 fragColorFilter;

layout (location = 0) out vec3 gPosition;
layout (location = 1) out vec4 gNormal;
layout (location = 2) out vec4 gAmbient;
layout (location = 3) out vec4 gDiffuse;
layout (location = 4) out vec4 gSpecular;

uniform sampler2D sampler;

void main() {
    vec4 texture = texture(sampler, fragTexCoord);
    if (texture.w < 0.1) discard;

    if (texture.r == texture.g && texture.r == texture.b && fragColorFilter.a > 0.5) {
        texture = fragColorFilter * texture.r;
    }

    gAmbient = vec4(texture.rgb, 1);
    gDiffuse = vec4(texture.rgb, 1);
    gSpecular = vec4(texture.rgb, 1);
    gPosition = fragPos;
    
    //W = Specular Weight
    gNormal = vec4(fragNormal, 2);
}