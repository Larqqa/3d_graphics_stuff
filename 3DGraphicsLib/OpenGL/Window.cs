using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using _3DGraphicsLib.OpenGL.Lights;
using _3DGraphicsLib.OpenGL.Objects;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace _3DGraphicsLib.OpenGL
{
    class Window : GameWindow
    {
        private Camera _camera;
        private int _vertices;
        private int _objectModel;
        private int _lampModel;
        private Shader _cubeProgram;
        private Shader _lampProgram;

        private readonly Vector3[] _cubePositions =
        {
            new Vector3(-1.0f, -2.0f, -1.0f),
            new Vector3(0.0f, -1.0f, 0.0f),
            new Vector3(1.0f, 0.0f, 1.0f),
            new Vector3(2.0f, 1.0f, 2.0f),

        };

        private bool _mouse1True;
        private bool _mouse2True;
        private readonly List<Point> _pointLights = new List<Point>();
        private readonly List<Cube> _cubes = new List<Cube>();
        private Vector3 _lastPos;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            CursorVisible = true;
            CursorGrabbed = true;
            _camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Multisample);

            _cubeProgram = new Shader("assets/shaders/vertex.glsl", "assets/shaders/light_fragment.glsl");
            _lampProgram = new Shader("assets/shaders/vertex.glsl", "assets/shaders/white_fragment.glsl");

            _vertices = Cube.Use();
            var index = 0;
            foreach (var pos in _cubePositions)
            {
                var even = index % 2 == 0;
                var tex = even ? "2" : "";
                var diff = $"assets/images/container{tex}.png";
                var spec = even ? "" : "assets/images/container_specular.png";

                _cubes.Add(new Cube(diff, spec, pos, _cubeProgram));

                index++;
            }

            {
                _objectModel = GL.GenVertexArray();
                GL.BindVertexArray(_objectModel);

                var positionLocation = _cubeProgram.GetAttribLocation("aPosition");
                GL.EnableVertexAttribArray(positionLocation);
                GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

                var normalLocation = _cubeProgram.GetAttribLocation("aNormal");
                GL.EnableVertexAttribArray(normalLocation);
                GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

                var texCoordLocation = _cubeProgram.GetAttribLocation("aTexCoords");
                GL.EnableVertexAttribArray(texCoordLocation);
                GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            }

            {
                _lampModel = GL.GenVertexArray();
                GL.BindVertexArray(_lampModel);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertices);

                var positionLocation = _lampProgram.GetAttribLocation("aPosition");
                GL.EnableVertexAttribArray(positionLocation);
                GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            }

            _pointLights.Add(new Point(new Vector3(-2.0f, -2.0f, -2.0f), new(1.0f, 0.5f, 0.0f)));
            _pointLights.Add(new Point(new Vector3(-2.0f, 2.0f, -2.0f), new(0.0f, 1.0f, 0.5f)));
            _pointLights.Add(new Point(new Vector3(2.0f, -2.0f, 2.0f), new(1.0f, 0.0f, 1.0f)));
            _pointLights.Add(new Point(new Vector3(2.0f, 2.0f, 2.0f), new(0.0f, 0.5f, 1.0f)));
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused)
                return;

            if (KeyboardState.IsKeyDown(Keys.Escape))
                Close();

            if (MouseState.IsButtonDown(MouseButton.Button1))
                _mouse1True = true;

            if (!MouseState.IsButtonDown(MouseButton.Button1) && _mouse1True)
            {
                _mouse1True = false;
                Spotlight.ToggleSpotlight();
            }

            _camera.Move(KeyboardState, MouseState, (float)e.Time);

            //_lastPos = _camera.Position;
            //foreach (var cube in _cubes)
            //{
            //    if (cube.Collision(_camera.Position))
            //        _camera.Position = _lastPos;
            //}

            if (MouseState.IsButtonDown(MouseButton.Button2))
                _mouse2True = true;

            if (!MouseState.IsButtonDown(MouseButton.Button2) && _mouse2True)
            {
                _mouse2True = false;
                _cubes.Add(new Cube("assets/images/container.png", "assets/images/container_specular.png", _camera.Position + (_camera.Front * 2f), _cubeProgram));
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindVertexArray(_objectModel);

            _cubeProgram.Use();

            _cubeProgram.SetMatrix4("view", _camera.GetViewMatrix());
            _cubeProgram.SetMatrix4("projection", _camera.GetProjectionMatrix());
            _cubeProgram.SetVector3("viewPos", _camera.Position);

            Directional.Use(_cubeProgram);

            Spotlight.Use(_cubeProgram, _camera);

            _cubeProgram.SetInt("pointLightAmount", _pointLights.Count);
            foreach (var light in _pointLights)
                light.Use(_cubeProgram);

            foreach (var cube in _cubes)
            {
                cube.Draw();
            }

            GL.BindVertexArray(_lampModel);
            _lampProgram.Use();
            _lampProgram.SetMatrix4("view", _camera.GetViewMatrix());
            _lampProgram.SetMatrix4("projection", _camera.GetProjectionMatrix());

            foreach (var light in _pointLights)
                light.Draw(_lampProgram);

            SwapBuffers();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _camera.Fov -= e.OffsetY;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
            _camera.AspectRatio = Size.X / (float)Size.Y;
        }
    }
}
