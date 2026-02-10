using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using CMineNew.Render.Object;
using CMineNew.Resources.Shaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using StbImageSharp;

namespace CMineNew.Render
{
    public class SkyBox
    {
        private int _id;

        public SkyBox(byte[] right, byte[] left, byte[] top, byte[] bottom, byte[] front, byte[] back)
        {
            GL.GenTextures(1, out _id);
            GL.BindTexture(TextureTarget.TextureCubeMap, _id);
            LoadTexture(right, TextureTarget.TextureCubeMapPositiveX);
            LoadTexture(left, TextureTarget.TextureCubeMapNegativeX);
            LoadTexture(top, TextureTarget.TextureCubeMapPositiveY);
            LoadTexture(bottom, TextureTarget.TextureCubeMapNegativeY);
            LoadTexture(front, TextureTarget.TextureCubeMapNegativeZ);
            LoadTexture(back, TextureTarget.TextureCubeMapPositiveZ);

            const int quality = (int)All.Nearest;
            const int edge = (int)All.ClampToEdge;
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, quality);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, quality);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, edge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, edge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, edge);
            GL.BindTexture(TextureTarget.TextureCubeMap, 0);

            SkyBoxStatic.SkyBoxShader.SetUInt("skyBox", 0);
        }

        private void LoadTexture(byte[] data, TextureTarget target)
        {
            StbImage.stbi_set_flip_vertically_on_load(0);
            ImageResult image = ImageResult.FromMemory(data, ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(
                target,
                0,
                PixelInternalFormat.Rgba,
                image.Width,
                image.Height,
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                image.Data
            );
        }

        public int Id => _id;

        public void Draw(Camera camera)
        {
            var view = new Matrix4(new Matrix3(camera.Matrix));
            var viewProjection = view * camera.Frustum.Matrix;

            SkyBoxStatic.SkyBoxVao.Bind();
            SkyBoxStatic.SkyBoxShader.Use();
            SkyBoxStatic.SkyBoxShader.SetUMatrix("viewProjection", viewProjection);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, _id);
            SkyBoxStatic.SkyBoxVao.DrawArrays(0, SkyBoxStatic.Vertices.Length);
        }
    }

    static class SkyBoxStatic
    {
        public static readonly float[] Vertices =
        {
            // positions          
            -1.0f, 1.0f, -1.0f,
            -1.0f, -1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,
            1.0f, 1.0f, -1.0f,
            -1.0f, 1.0f, -1.0f,

            -1.0f, -1.0f, 1.0f,
            -1.0f, -1.0f, -1.0f,
            -1.0f, 1.0f, -1.0f,
            -1.0f, 1.0f, -1.0f,
            -1.0f, 1.0f, 1.0f,
            -1.0f, -1.0f, 1.0f,

            1.0f, -1.0f, -1.0f,
            1.0f, -1.0f, 1.0f,
            1.0f, 1.0f, 1.0f,
            1.0f, 1.0f, 1.0f,
            1.0f, 1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,

            -1.0f, -1.0f, 1.0f,
            -1.0f, 1.0f, 1.0f,
            1.0f, 1.0f, 1.0f,
            1.0f, 1.0f, 1.0f,
            1.0f, -1.0f, 1.0f,
            -1.0f, -1.0f, 1.0f,

            -1.0f, 1.0f, -1.0f,
            1.0f, 1.0f, -1.0f,
            1.0f, 1.0f, 1.0f,
            1.0f, 1.0f, 1.0f,
            -1.0f, 1.0f, 1.0f,
            -1.0f, 1.0f, -1.0f,

            -1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f, 1.0f,
            1.0f, -1.0f, -1.0f,
            1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f, 1.0f,
            1.0f, -1.0f, 1.0f
        };

        public static readonly VertexArrayObject SkyBoxVao = LoadVao();

        public static readonly ShaderProgram SkyBoxShader =
            new ShaderProgram(Shaders.sky_box_vertex, Shaders.sky_box_fragment);

        private static VertexArrayObject LoadVao()
        {
            var vao = new VertexArrayObject();
            var vbo = new VertexBufferObject();
            vao.Bind();
            vbo.Bind(BufferTarget.ArrayBuffer);
            vao.LinkBuffer(vbo);
            vbo.SetData(BufferTarget.ArrayBuffer, Vertices, BufferUsageHint.StaticDraw);
            var builder = new AttributePointerBuilder(vao, 3, 0);
            builder.AddPointer(3, false);
            VertexBufferObject.Unbind(BufferTarget.ArrayBuffer);
            VertexArrayObject.Unbind();
            return vao;
        }
    }
}