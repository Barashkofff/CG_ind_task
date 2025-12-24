#version 120

attribute vec3 position;

void main() {
    gl_Position = vec4(position.y,position.x, position.z, 1.0);
}