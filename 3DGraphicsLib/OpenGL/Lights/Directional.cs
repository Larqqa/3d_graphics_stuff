using OpenTK.Mathematics;

namespace _3DGraphicsLib.OpenGL.Lights
{
    class Directional
    {
        public static Vector3 Direction = new Vector3(-0.2f, -1.0f, -0.3f);
        public static Vector3 Ambient { get; set; } = new Vector3(0.05f, 0.05f, 0.05f);
        public static Vector3 Diffuse { get; set; } = new Vector3(0.4f, 0.4f, 0.4f);
        public static Vector3 Specular { get; set; } = new Vector3(0.5f, 0.5f, 0.5f);

        public static void Use(Shader shader)
        {
            shader.SetVector3("dirLight.direction", Direction);
            shader.SetVector3("dirLight.ambient", Ambient);
            shader.SetVector3("dirLight.diffuse", Diffuse);
            shader.SetVector3("dirLight.specular", Specular);
        }
    }
}
