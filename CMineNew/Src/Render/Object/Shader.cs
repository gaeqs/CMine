using System;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Render.Object{
    public class Shader{
        private readonly int _id;
        private readonly ShaderType _shaderType;
        private bool _error;

        public Shader(string data, ShaderType shaderType) {
            _shaderType = shaderType;

            _id = GL.CreateShader(shaderType);
            GL.ShaderSource(_id, data);
            GL.CompileShader(_id);
            CheckErrors(data);
        }

        public int Id => _id;

        public ShaderType ShaderType => _shaderType;

        public bool Error => _error;

        public void CleanUp() {
            GL.DeleteShader(_id);
        }

        private void CheckErrors(string data) {
            var info = GL.GetShaderInfoLog(_id);
            _error = info.Length != 0;
            if (!_error) return;
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("Error while parsing shader: ");
            Console.WriteLine(data);
            Console.WriteLine("Errors: ");
            Console.WriteLine(info);
            Console.WriteLine("-----------------------------------");
        }
    }
}