using System.Collections.Generic;

namespace CMineNew.Render.Object{
    public class ShaderManager{
        private static readonly Dictionary<string, ShaderProgram> Programs = new Dictionary<string, ShaderProgram>();

        public static ShaderProgram GetOrCreateShader(string name, string vertex, string fragment) {
            if (Programs.TryGetValue(name, out var value)) return value;
            var program = new ShaderProgram(vertex, fragment);
            Programs.Add(name, program);
            return program;
        }

        public static void CleanUp() {
            foreach (var program in Programs) {
                program.Value.CleanUp();
            }
        }
    }
}