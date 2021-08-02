using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace _3DGraphicsLib.OpenGL.Lights
{
    class Point
    {
        private readonly Vector3 _position;
        private readonly int _index;
        private static int _counter = 0;
        private Vector3 _color;
        public float Constant = 1.0f;
        public float Linear = 0.5f;
        public float Quadratic = 0.5f;

        public Point(Vector3 position, Vector3 color)
        {
            _position = position;
            _color = color;
            _index = _counter;
            _counter++;
        }

        public void Use(Shader shader)
        {
            shader.SetVector3($"pointLights[{_index}].position", _position);
            shader.SetVector3($"pointLights[{_index}].ambient", _color * 0.5f);
            shader.SetVector3($"pointLights[{_index}].diffuse", _color * 0.8f);
            shader.SetVector3($"pointLights[{_index}].specular", _color);
            shader.SetFloat($"pointLights[{_index}].constant", Constant);
            shader.SetFloat($"pointLights[{_index}].linear", Linear);
            shader.SetFloat($"pointLights[{_index}].quadratic", Quadratic);
        }

        public void Draw(Shader shader)
        {

            var lampMatrix = Matrix4.CreateScale(0.2f);
            lampMatrix = lampMatrix * Matrix4.CreateTranslation(_position);
            shader.SetMatrix4("model", lampMatrix);
            shader.SetVector3("lightColor", _color);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }
    }
}
