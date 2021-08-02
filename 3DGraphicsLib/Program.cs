using System;
using _3DGraphicsLib.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace _3DGraphicsLib
{
    class Program
    {
        private static void Main(string[] args)
        {
            var nativeWindowSettings = new NativeWindowSettings()
            {
                APIVersion = Version.Parse("4.1.0"),
                Size = new Vector2i(800, 600),
                Title = "3D Graphics WOOO!",
                NumberOfSamples = 8
            };

            var gameWindowSettings = new GameWindowSettings()
            {
                RenderFrequency = 60,
                UpdateFrequency = 60,
            };

            var window = new Window(gameWindowSettings, nativeWindowSettings);
            window.Run();
        }
    }
}
