using CMineNew.Loader;
using CMineNew.Render;
using CMineNew.Render.Object;
using CMineNew.Resources.Shaders;
using CMineNew.Resources.Textures;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.RayTrace{
    public static class Pointer{
        private static readonly Vertex[] Vertices = {
            new Vertex(-0.01f, -0.01f, 1, 0, 0, 0, 0, 1),
            new Vertex(0.01f, -0.01f, 1, 0, 0, 0, 1, 1),
            new Vertex(-0.01f, 0.01f, 1, 0, 0, 0, 0, 0),
            new Vertex(0.01f, 0.01f, 1, 0, 0, 0, 1, 0)
        };

        private static readonly int[] Indices = {0, 1, 3, 0, 3, 2};

        private static VertexArrayObject VertexArrayObject;

        private static ShaderProgram Shader;

        private static int Texture;

        public static void Load() {
            VertexArrayObject = new VertexArrayObject(Vertices, Indices);
            Shader = new ShaderProgram(Shaders.pointer_vertex, Shaders.pointer_fragment);
            Shader.SetUInt("pointer", 0);
            Texture = ImageLoader.Load(Textures.pointer);
        }

        public static void Draw(Camera camera) {
            GL.Disable(EnableCap.DepthTest);
            Shader.Use();
            VertexArrayObject.Bind();
            Shader.SetUFloat("aspectRatio", camera.Frustum.AspectRatio);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.One);
            GL.BlendEquationSeparate(BlendEquationMode.FuncSubtract, BlendEquationMode.Max);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Texture);
            VertexArrayObject.Draw();
        }
    }
}