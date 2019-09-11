using System;
using CMineNew.Light;
using CMineNew.Render;
using CMineNew.Render.Object;
using CMineNew.Resources.Shaders;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Map{
    public class WorldGBuffer{
        private static readonly DrawBuffersEnum[] DrawBuffersEnums = {
            DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1, DrawBuffersEnum.ColorAttachment2
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
        private readonly ShaderProgram _postRenderShader, _postRenderWaterShader;

        private VertexArrayObject _quadVao;
        private int _width, _height;

        private int _depthTexture, _normalTexture, _albedoTexture, _brightnessTexture;

        public WorldGBuffer(INativeWindow window) {
            _postRenderShader = new ShaderProgram(Shaders.post_render_vertex, Shaders.post_render_fragment);
            _postRenderShader.SetupForPostRender();
            _postRenderWaterShader = new ShaderProgram(Shaders.post_render_vertex, Shaders.post_render_water_fragment);
            _postRenderWaterShader.SetupForPostRender();
            _width = window.Width;
            _height = window.Height;
            _quadVao = GenerateQuadVao();
            GenerateTextures();
            GenerateFrameBuffer();
        }

        public VertexArrayObject QuadVao => _quadVao;

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

        public void BindPositionAndNormalTextures() {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _depthTexture);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, _normalTexture);
        }

        public void Draw(Camera camera, Vector3 ambientColor, float ambientStrength, bool waterShader, SkyBox skyBox) {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _quadVao.Bind();
            UsePostRenderShader(waterShader);
            _postRenderShader.SetUVector("cameraPosition", camera.Position);
            _postRenderShader.SetUMatrix("invertedViewProjection", camera.InvertedViewProjection);
            _postRenderShader.SetUVector("ambientColor", ambientColor);
            _postRenderShader.SetUFloat("ambientStrength", ambientStrength);

            const int min = (CMine.ChunkRadius - 2) << 4;
            const int max = (CMine.ChunkRadius - 1) << 4;
            _postRenderShader.SetUFloat("viewDistanceSquared", min * min);
            _postRenderShader.SetUFloat("viewDistanceOffsetSquared", max * max);


            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _albedoTexture);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, _depthTexture);
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, _normalTexture);
            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture2D, _brightnessTexture);
            GL.ActiveTexture(TextureUnit.Texture4);
            GL.BindTexture(TextureTarget.TextureCubeMap, skyBox.Id);
            DrawQuad();
        }

        private void UsePostRenderShader(bool water) {
            if (water) {
                _postRenderWaterShader.Use();
            }
            else {
                _postRenderShader.Use();
            }
        }

        #region constructor methods

        public static VertexArrayObject GenerateQuadVao() {
            var vao = new VertexArrayObject();
            vao.Bind();
            var vbo = new VertexBufferObject();
            vao.LinkBuffer(vbo);
            vbo.Bind(BufferTarget.ArrayBuffer);
            vbo.SetData(BufferTarget.ArrayBuffer, QuadVertices, BufferUsageHint.StaticDraw);
            var builder = new AttributePointerBuilder(vao, 4, 0);
            builder.AddPointer(2, false);
            builder.AddPointer(2, false);
            VertexBufferObject.Unbind(BufferTarget.ArrayBuffer);
            VertexArrayObject.Unbind();
            return vao;
        }

        private void GenerateTextures() {
            var images = new int[4];
            GL.GenTextures(images.Length, images);
            _depthTexture = images[0];
            _normalTexture = images[1];
            _albedoTexture = images[2];
            _brightnessTexture = images[3];
            Resize();
        }

        private void Resize() {
            ConfigureTexture(_width, _height, _depthTexture, PixelInternalFormat.DepthComponent, PixelFormat.DepthComponent);
            ConfigureTexture(_width, _height, _normalTexture, PixelInternalFormat.Rgba16f, PixelFormat.Rgba);
            ConfigureTexture(_width, _height, _albedoTexture, PixelInternalFormat.Rgb16f, PixelFormat.Rgb);
            ConfigureTexture(_width, _height, _brightnessTexture, PixelInternalFormat.Rgb16f, PixelFormat.Rgb);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private void GenerateFrameBuffer() {
            GL.GenFramebuffers(1, out _id);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _id);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _normalTexture, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment1, TextureTarget.Texture2D, _albedoTexture, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment2, TextureTarget.Texture2D, _brightnessTexture, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, _depthTexture, 0);
            GL.DrawBuffers(DrawBuffersEnums.Length, DrawBuffersEnums);
            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete) {
                throw new System.Exception("Framebuffer thrown error " + status + ".");
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        private static void ConfigureTexture(int width, int height, int texture,
            PixelInternalFormat format, PixelFormat outFormat, PixelType pixelType = PixelType.Float) {
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.PixelStore(PixelStoreParameter.PackAlignment, 1);
            var nearest = (int) All.Nearest;
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ref nearest);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ref nearest);
            GL.TexImage2D(TextureTarget.Texture2D, 0, format, width, height, 0, outFormat, pixelType,
                IntPtr.Zero);
        }

        #endregion

        private void DrawQuad() {
            _quadVao.DrawArrays(0, 6);
        }
    }
}