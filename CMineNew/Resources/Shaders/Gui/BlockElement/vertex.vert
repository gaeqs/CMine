#version 440 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec2 texturePosition;

out vec2 fragTexCoord;
flat out vec4 fragColorFilter;

uniform vec2 instancePosition;
uniform vec3 instanceSize;
uniform vec4[6] instanceTextureAreas;
uniform mat4 model, projection;

uniform vec4 colorFilter;

void main () {
    gl_Position = projection * (model * vec4(position * instanceSize - instanceSize / 2, 1) - vec4(0, 0, 2, 0));
    gl_Position /= gl_Position.w;
    gl_Position += vec4(instancePosition, 0, 0);
    
    vec4 textureArea = instanceTextureAreas[ gl_VertexID / 4];
    vec2 minT = textureArea.xy;
    vec2 maxT = textureArea.zw;
    vec2 size = maxT - minT;

    fragTexCoord = minT + texturePosition * size;

    fragColorFilter = colorFilter;
}
