#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform vec3 lightColor;

out vec3 Normal;
out vec2 TexCoords;
out vec3 FragPos;
out vec4 lColor;

void main()
{
    gl_Position = vec4(aPosition, 1.0) * model * view * projection;
    FragPos = vec3(vec4(aPosition, 1.0) * model);
    Normal = aNormal * mat3(transpose(inverse(model)));
    TexCoords = aTexCoords;
    lColor = vec4(lightColor, 1.0);
}