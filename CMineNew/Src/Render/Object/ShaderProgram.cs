using System.Collections.Generic;
using System.Collections.ObjectModel;
using CMineNew.Exception.Shader;
using CMineNew.Geometry;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Render.Object{
    public class ShaderProgram{
        private static int _bindShader = 0;

        public static void UnbindShader() {
            GL.UseProgram(0);
            _bindShader = 0;
        }

        private readonly int _id;
        private readonly Collection<Shader> _shaders;
        private readonly Dictionary<string, int> _uniforms;

        public ShaderProgram(Collection<Shader> shaders) {
            _shaders = shaders;

            _uniforms = new Dictionary<string, int>();
            _id = GL.CreateProgram();
            if (_id == 0) throw new ShaderProgramCreationException("Program id is 0.");
            foreach (var shader in _shaders) {
                GL.AttachShader(_id, shader.Id);
            }

            GL.LinkProgram(_id);
        }

        public ShaderProgram(string vertexData, string fragmentData) {
            _shaders = new Collection<Shader>();
            var vertex = new Shader(vertexData, ShaderType.VertexShader);
            if (vertex.Error) throw new ShaderProgramCreationException("Error while loading vertex shader.");
            var fragment = new Shader(fragmentData, ShaderType.FragmentShader);
            if (fragment.Error) throw new ShaderProgramCreationException("Error while loading vertex shader.");
            _uniforms = new Dictionary<string, int>();
            _id = GL.CreateProgram();
            if (_id == 0) throw new ShaderProgramCreationException("Program id is 0.");
            GL.AttachShader(_id, vertex.Id);
            GL.AttachShader(_id, fragment.Id);
            GL.LinkProgram(_id);
            vertex.CleanUp();
            fragment.CleanUp();
        }

        public ShaderProgram(IReadOnlyList<string> shaderData, IReadOnlyList<ShaderType> types) {
            if (shaderData.Count != types.Count)
                throw new ShaderProgramCreationException("Shaders and types must have the same length.");
            _shaders = new Collection<Shader>();
            for (var i = 0; i < shaderData.Count; i++) {
                var shader = new Shader(shaderData[i], types[i]);
                if (!shader.Error) _shaders.Add(shader);
            }

            _uniforms = new Dictionary<string, int>();
            _id = GL.CreateProgram();
            if (_id == 0) throw new ShaderProgramCreationException("Program id is 0.");
            foreach (var shader in _shaders) {
                GL.AttachShader(_id, shader.Id);
            }

            GL.LinkProgram(_id);
            foreach (var shader in _shaders) {
                shader.CleanUp();
            }

            _shaders.Clear();
        }

        public int Id => _id;

        public void Use() {
            if (_bindShader == _id) return;
            GL.UseProgram(_id);
            _bindShader = _id;
        }

        public void CleanUp() {
            if (_bindShader == _id) {
                UnbindShader();
            }

            foreach (var shader in _shaders) {
                GL.DetachShader(_id, shader.Id);
            }

            GL.DeleteProgram(_id);
        }

        public int GetUniformLocation(string uniformName) {
            if (_uniforms.ContainsKey(uniformName))
                return _uniforms[uniformName];
            var location = GL.GetUniformLocation(_id, uniformName);
            _uniforms.Add(uniformName, location);
            return location;
        }

        public void SetUBoolean(string name, bool value) {
            GL.Uniform1(GetUniformLocation(name), value ? 1 : 0);
        }

        public void SetUInt(string name, int value) {
            GL.Uniform1(GetUniformLocation(name), value);
        }

        public void SetUFloat(string name, float value) {
            GL.Uniform1(GetUniformLocation(name), value);
        }

        public void SetUVector(string name, Vector2 value) {
            GL.Uniform2(GetUniformLocation(name), value);
        }

        public void SetUVector(string name, Vector3 value) {
            GL.Uniform3(GetUniformLocation(name), value);
        }

        public void SetUVector(string name, Vector3i value) {
            GL.Uniform3(GetUniformLocation(name), value.ToFloat());
        }

        public void SetUVector(string name, Vector4 value) {
            GL.Uniform4(GetUniformLocation(name), value);
        }

        public void SetUMatrix(string name, Matrix4 value) {
            GL.UniformMatrix4(GetUniformLocation(name), false, ref value);
        }

        public void SetupForPostRender() {
            Use();
            SetUInt("gAmbient", 0);
            SetUInt("gDiffuse", 1);
            SetUInt("gSpecular", 2);
            SetUInt("gPosition", 3);
            SetUInt("gNormal", 4);
        }
    }
}