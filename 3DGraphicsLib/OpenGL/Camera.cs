using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace _3DGraphicsLib.OpenGL
{
    class Camera
    {
        private Vector3 _front = -Vector3.UnitZ;
        private Vector3 _up = Vector3.UnitY;
        private Vector3 _right = Vector3.UnitX;
        private float _pitch;
        private float _yaw = -MathHelper.PiOver2;
        private float _fov = MathHelper.DegreesToRadians(70.0f);
        private static bool _firstMove = true;
        private static Vector2 _lastPos;
        private const float CameraSpeed = 1.5f;
        private const float Sensitivity = 0.2f;
        public Vector3 Position { get; set; }
        public float AspectRatio { private get; set; }
        public Vector3 Front => _front;
        public Vector3 Up => _up;
        public Vector3 Right => _right;

        public Camera(Vector3 position, float aspectRatio)
        {
            Position = position;
            AspectRatio = aspectRatio;
        }

        public void Move(KeyboardState input, MouseState mouse, float time)
        {
            if (input.IsKeyDown(Keys.W))
            {
                Position += _front * CameraSpeed * time; // Forward
            }
            if (input.IsKeyDown(Keys.S))
            {
                Position -= _front * CameraSpeed * time; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                Position -= _right * CameraSpeed * time; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                Position += _right * CameraSpeed * time; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                Position += _up * CameraSpeed * time; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                Position -= _up * CameraSpeed * time; // Down
            }

            if (_firstMove)
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                // Calculate the offset of the mouse position
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                Yaw += deltaX * Sensitivity;
                Pitch -= deltaY * Sensitivity;
            }
        }

        public float Pitch
        {
            get => MathHelper.RadiansToDegrees(_pitch);
            set
            {
                var angle = MathHelper.Clamp(value, -89f, 89f);
                _pitch = MathHelper.DegreesToRadians(angle);
                UpdateVectors();
            }
        }

        public float Yaw
        {
            get => MathHelper.RadiansToDegrees(_yaw);
            set
            {
                _yaw = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }

        public float Fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 70f);
                _fov = MathHelper.DegreesToRadians(angle);
            }
        }

        public Matrix4 GetViewMatrix()
        {
            //var cameraPosition = new Vector3(Position.X, 0, Position.Z); // No up down movement
            return Matrix4.LookAt(Position, Position + _front, _up);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
        }

        private void UpdateVectors()
        {
            _front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
            _front.Y = MathF.Sin(_pitch);
            _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

            _front = Vector3.Normalize(_front);
            _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }
    }
}
