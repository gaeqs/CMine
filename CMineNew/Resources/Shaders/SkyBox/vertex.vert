#version 440 core

layout (location = 0) in vec3 position;

out vec3 fragTexCoord;

uniform mat4 viewProjection;

void main () {
    gl_Position = viewProjection * vec4(position, 1);
    gl_Position = gl_Position.xyww;
    fragTexCoord = position;
}
