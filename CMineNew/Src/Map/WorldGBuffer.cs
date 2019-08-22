using System;
using CMineNew.Render.Object;
using CMineNew.Resources.Shaders;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Map{
    public class WorldGBuffer{
        private static readonly DrawBuffersEnum[] DrawBuffersEnums = {
            DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1,
            DrawBuffersEnum.ColorAttachment2, DrawBuffersEnum.ColorAttachment3,
            DrawBuffersEnum.ColorAttachment4
        };

        private static readonly float[] QuadVertices = {
            // positions   // texCoords
            -1.0f, 1.0f, 0.0f, 1.0f,
            -1.0f, -1.0f, 0.0f, 0.0f,
            1.0f, -1.0f, 1.0f, 0.0f,

            -1.0f, 1.0f, 0.0f, 1.0f,
            1.0f, -1.0f, 1.0f, 0.0f,
            1.0f, 1.0f, 1.0f, 1.0f
        };

        private int _id;
        private readonly ShaderProgram _postRenderShader, _backgroundShader;

        private int _quadVao;
        private int _width, _height;

        private int _positionTexture, _normalTexture, _ambientTexture, _diffuseTexture, _specularTexture;
        private int _depthBuffer;

        public WorldGBuffer(INativeWindow window) {
            _postRenderShader = new ShaderProgram(Shaders.post_render_vertex, Shaders.post_render_fragment);
            _postRenderShader.SetupForPostRender();
            _backgroundShader = new ShaderProgram(Shaders.background_vertex, Shaders.background_fragment);
            _width = window.Width;
            _height = window.Height;
            GenerateSimpleVao();
            GenerateTextures();
            GenerateFrameBuffer();
        }

        public void Bind() {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _id);
        }

        public static void Unbind() {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void TransferDepthBufferToMainFbo() {
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _id);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
            GL.BlitFramebuffer(0, 0, _width, _height, 0, 0,
                _width, _height, ClearBufferMask.DepthBufferBit, BlitFramebufferFilter.Nearest);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void DrawBackground(Color4 color) {
            VertexArrayObject.Bind(_quadVao);
            _backgroundShader.Use();
            _backgroundShader.SetUVector("background", new Vector4(color.R, color.G, color.B, color.A));
            DrawQuad();
        }

        public void Draw(Vector3 cameraPosition, Vector3 ambientColor, float ambientStrength, bool waterShader) {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Disable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            VertexArrayObject.Bind(_quadVao);
            _postRenderShader.Use();
            _postRenderShader.SetUVector("cameraPosition", cameraPosition);
            _postRenderShader.SetUVector("ambientColor", ambientColor);
            _postRenderShader.SetUFloat("ambientStrength", ambientStrength);

            const int min = (CMine.ChunkRadius - 2) << 4;
            const int max = (CMine.ChunkRadius - 1) << 4;
            _postRenderShader.SetUFloat("waterShader", waterShader ? 1 : 0);
            _postRenderShader.SetUFloat("viewDistanceSquared", min * min);
            _postRenderShader.SetUFloat("viewDistanceOffsetSquared", max * max);
            _postRenderShader.SetUVector("fogColor", new Vector4(0, 1, 1, 1));


            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _ambientTexture);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, _diffuseTexture);
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, _specularTexture);
            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture2D, _positionTexture);
            GL.ActiveTexture(TextureUnit.Texture4);
            GL.BindTexture(TextureTarget.Texture2D, _normalTexture);
            DrawQuad();
        }


        #region constructor methods

        private void GenerateSimpleVao() {
            GL.GenVertexArrays(1, out _quadVao);
            GL.GenBuffers(1, out int quadVbo);
            VertexArrayObject.Bind(_quadVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, quadVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * QuadVertices.Length,
                QuadVertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false,
                sizeof(float) * 4, 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false,
                sizeof(float) * 4, sizeof(float) * 2);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            VertexArrayObject.Unbind();
        }

        private void GenerateTextures() {
            var images = new int[5];
            GL.GenTextures(images.Length, images);
            _positionTexture = images[0];
            _normalTexture = images[1];
            _ambientTexture = images[2];
            _diffuseTexture = images[3];
            _specularTexture = images[4];

            GL.GenRenderbuffers(1, out _depthBuffer);
            Resize();
        }

        private void Resize() {
            ConfigureTexture(_width, _height, _positionTexture, PixelInternalFormat.Rgb32f, PixelFormat.Rgb);
            ConfigureTexture(_width, _height, _normalTexture, PixelInternalFormat.Rgba32f, PixelFormat.Rgba);
            ConfigureTexture(_width, _height, _ambientTexture, PixelInternalFormat.Rgba16f, PixelFormat.Rgba);
            ConfigureTexture(_width, _height, _diffuseTexture, PixelInternalFormat.Rgb8, PixelFormat.Rgb);
            ConfigureTexture(_width, _height, _specularTexture, PixelInternalFormat.Rgb8, PixelFormat.Rgb);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _depthBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, _width,
                _height);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        }

        private void GenerateFrameBuffer() {
            GL.GenFramebuffers(1, out _id);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _id);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, _positionTexture, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment1,
                TextureTarget.Texture2D, _normalTexture, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment2,
                TextureTarget.Texture2D, _ambientTexture, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment3,
                TextureTarget.Texture2D, _diffuseTexture, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment4,
                TextureTarget.Texture2D, _specularTexture, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                RenderbufferTarget.Renderbuffer, _depthBuffer);
            GL.DrawBuffers(DrawBuffersEnums.Length, DrawBuffersEnums);
            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete) {
                throw new System.Exception("Framebuffer thrown error " + status + ".");
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        private static void ConfigureTexture(int width, int height, int texture,
            PixelInternalFormat format, PixelFormat outFormat) {
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.PixelStore(PixelStoreParameter.PackAlignment, 1);
            var nearest = (int) All.Nearest;
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ref nearest);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ref nearest);
            GL.TexImage2D(TextureTarget.Texture2D, 0, format, width, height, 0, outFormat, PixelType.Float,
                IntPtr.Zero);
        }

        #endregion

        private void DrawQuad() {
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }
    }
}