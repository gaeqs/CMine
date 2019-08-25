using System.Collections.Generic;

namespace CMineNew.Render.Object{
    public class ShaderManager{
        private static readonly Dictionary<string, ShaderProgram> _programs = new Dictionary<string, ShaderProgram>();

        public static ShaderProgram GetOrCreateShader(string name, string vertex, string fragment) {
            if (_programs.TryGetValue(name, out var value)) return value;
            var program = new ShaderProgram(vertex, fragment);
            _programs.Add(name, program);
            return program;
        }

        public static void CleanUp() {
            foreach (var program in _programs) {
                program.Value.CleanUp();
            }
        }
    }
}