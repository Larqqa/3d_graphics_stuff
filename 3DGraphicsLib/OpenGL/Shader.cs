using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace _3DGraphicsLib.OpenGL
{
    class Shader
    {
        public readonly int Handle;
        private readonly Dictionary<string, int> _uniformLocations;

        public Shader(string vertexShaderLocation, string fragmentShaderLocation)
        {
            Handle = GL.CreateProgram();
            var vertexShader = LoadShader(vertexShaderLocation, ShaderType.VertexShader);
            var fragmentShader = LoadShader(fragmentShaderLocation, ShaderType.FragmentShader);

            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);
            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
                throw new Exception($"Error occurred whilst linking Program({Handle})");

            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
            _uniformLocations = new Dictionary<string, int>();

            for (var i = 0; i < numberOfUniforms; i++)
            {
                var key = GL.GetActiveUniform(Handle, i, out _, out _);
                var location = GL.GetUniformLocation(Handle, key);
                _uniformLocations.Add(key, location);
            }
        }

        private static int LoadShader(string shaderLocation, ShaderType type)
        {
            var shaderId = GL.CreateShader(type);
            GL.ShaderSource(shaderId, File.ReadAllText(shaderLocation));
            GL.CompileShader(shaderId);
            GL.GetShader(shaderId, ShaderParameter.CompileStatus, out var code);

            if (code != (int) All.True)
            {
                var infoLog = GL.GetShaderInfoLog(shaderId);
                throw new Exception($"Error occurred whilst compiling Shader({shaderId}).\n\n{infoLog}");
            }
            
            return shaderId;
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }
        
        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(Handle, attribName);
        }

        public void SetBoolean(string name, bool data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(_uniformLocations[name], data ? 1 : 0);
        }

        public void SetInt(string name, int data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(_uniformLocations[name], data);
        }

        public void SetFloat(string name, float data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(_uniformLocations[name], data);
        }

        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(Handle);
            GL.UniformMatrix4(_uniformLocations[name], true, ref data);
        }

        public void SetVector3(string name, Vector3 data)
        {
            GL.UseProgram(Handle);
            GL.Uniform3(_uniformLocations[name], data);
        }

        public void Delete()
        {
            GL.DeleteProgram(Handle);
        }
    }
}
