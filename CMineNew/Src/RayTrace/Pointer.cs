using System;
using CMineNew.Render;
using CMineNew.Render.Object;
using CMineNew.Resources.Shaders;
using CMineNew.Resources.Textures;
using GraphicEngine.Loader;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.RayTrace{
    public class Pointer{
        private static readonly Vertex[] Vertices = {
            new Vertex(-0.005f, -0.005f, 0, 0, 0, 0, 0, 0),
            new Vertex(-0.005f, 0.005f, 0, 0, 0, 0, 0, 1),
            new Vertex(0.005f, -0.005f, 0, 0, 0, 0, 1, 0),
            new Vertex(0.005f, 0.005f, 0, 0, 0, 0, 1, 1)
        };

        private static readonly int[] Indices = {0, 2, 3, 0, 1, 3};

        private static VertexArrayObject VertexArrayObject;

        private static ShaderProgram Shader;

        private static int Texture;

        public static void Load() {
            VertexArrayObject = new VertexArrayObject(Vertices, Indices);
            Shader = new ShaderProgram(Shaders.pointer_vertex, Shaders.pointer_fragment);
            Texture =  ImageLoader.Load(Textures.pointer);
        }

        public static void Draw(Camera camera) {
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