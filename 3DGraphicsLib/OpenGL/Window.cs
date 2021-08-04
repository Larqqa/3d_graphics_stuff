using System;
using System.Collections.Generic;
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
        private int _lampModel;
        private int _objModel;
        private Shader _lampProgram;
        private Shader _modelProgram;
        private Model _model;
        private int _vbo;

        private readonly float[] _vertix =
        {
            -0.5f, -0.5f, 0.0f, // Bottom-left vertex
            0.5f, -0.5f, 0.0f, // Bottom-right vertex
            0.0f,  0.5f, 0.0f  // Top vertex
        };

        private readonly List<Point> _pointLights = new List<Point>();

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            CursorVisible = true;
            CursorGrabbed = true;
            _camera = new Camera(new Vector3(0f, 0f, 3f), Size.X / (float)Size.Y);

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Multisample);

            //_lampProgram = new Shader("assets/shaders/vertex.glsl", "assets/shaders/white_fragment.glsl");
            _modelProgram = new Shader("assets/shaders/model_vertex.glsl", "assets/shaders/model_fragment.glsl");
            _model = new Model("assets/models/backpack/backpack.obj");

            //_vertices = Cube.Use();

            //{
            //    _lampModel = GL.GenVertexArray();
            //    GL.BindVertexArray(_lampModel);

            //    GL.BindBuffer(BufferTarget.ArrayBuffer, _vertices);

            //    var positionLocation = _lampProgram.GetAttribLocation("aPosition");
            //    GL.EnableVertexAttribArray(positionLocation);
            //    GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            //}

            //_pointLights.Add(new Point(new Vector3(-2.0f, -2.0f, -2.0f), new(1.0f, 0.5f, 0.0f)));
            //_pointLights.Add(new Point(new Vector3(-2.0f, 2.0f, -2.0f), new(0.0f, 1.0f, 0.5f)));
            //_pointLights.Add(new Point(new Vector3(2.0f, -2.0f, 2.0f), new(1.0f, 0.0f, 1.0f)));
            //_pointLights.Add(new Point(new Vector3(2.0f, 2.0f, 2.0f), new(0.0f, 0.5f, 1.0f)));
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused)
                return;

            if (KeyboardState.IsKeyDown(Keys.Escape))
                Close();

            _camera.Move(KeyboardState, MouseState, (float)e.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //GL.BindVertexArray(_lampModel);
            //_lampProgram.Use();
            //_lampProgram.SetMatrix4("view", _camera.GetViewMatrix());
            //_lampProgram.SetMatrix4("projection", _camera.GetProjectionMatrix());

            //foreach (var light in _pointLights)
            //{
            //    light.Draw(_lampProgram);
            //}

            _modelProgram.Use();
            var model = Matrix4.Identity;
            model *= Matrix4.CreateTranslation(new Vector3(1.0f, 1.0f, 1.0f));
            model *= Matrix4.CreateScale(new Vector3(0.5f,0.5f,0.5f));

            _modelProgram.SetMatrix4("model", model);
            _modelProgram.SetMatrix4("view", _camera.GetViewMatrix());
            _modelProgram.SetMatrix4("projection", _camera.GetProjectionMatrix());

            _model.Draw(_modelProgram);

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
