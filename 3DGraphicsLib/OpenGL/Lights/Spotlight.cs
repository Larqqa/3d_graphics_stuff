using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace _3DGraphicsLib.OpenGL.Lights
{
    class Spotlight
    {
        private static Boolean _spotlightOn = false;
        public static Vector3 Ambient { get; set; } = new Vector3(0.0f, 0.0f, 0.0f);
        public static Vector3 Diffuse { get; set; } = new Vector3(1.0f, 1.0f, 1.0f);
        public static Vector3 Specular { get; set; } = new Vector3(1.0f, 1.0f, 1.0f);
        public static float Constant { get; set; } = 1.0f;
        public static float Linear { get; set; } = 0.09f;
        public static float Quadratic { get; set; } = 0.032f;
        public static float CutOff { get; set; } = MathF.Cos(MathHelper.DegreesToRadians(12.0f));
        public static float OuterCutOff { get; set; } = MathF.Cos(MathHelper.DegreesToRadians(18.0f));

        public static void ToggleSpotlight()
        {
            _spotlightOn = !_spotlightOn;
        }

        public static void Use(Shader shader, Camera camera)
        {
            shader.SetBoolean("spotlightOn", _spotlightOn);
            shader.SetVector3("spotLight.position", camera.Position);
            shader.SetVector3("spotLight.direction", camera.Front);
            shader.SetVector3("spotLight.ambient", Ambient);
            shader.SetVector3("spotLight.diffuse", Diffuse);
            shader.SetVector3("spotLight.specular", Specular);
            shader.SetFloat("spotLight.constant", Constant);
            shader.SetFloat("spotLight.linear", Linear);
            shader.SetFloat("spotLight.quadratic", Quadratic);
            shader.SetFloat("spotLight.cutOff", CutOff);
            shader.SetFloat("spotLight.outerCutOff", OuterCutOff);
        }
    }
}
