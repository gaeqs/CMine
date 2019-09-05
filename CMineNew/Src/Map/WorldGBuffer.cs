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
            DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1,
            DrawBuffersEnum.ColorAttachment2, DrawBuffersEnum.ColorAttachment3,
            DrawBuffersEnum.ColorAttachment4, DrawBuffersEnum.ColorAttachment5,
            DrawBuffersEnum.ColorAttachment6, DrawBuffersEnum.ColorAttachment7
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
        private readonly ShaderProgram _directionalShader, _pointShader, _flashShader;

        private VertexArrayObject _quadVao;
        private int _width, _height;

        private int _positionTexture, _normalTexture, _ambientTexture, _diffuseTexture, _specularTexture;
        private int _ambientBrightness, _diffuseBrightness, _specularBrightness;
        private int _depthBuffer;

        public WorldGBuffer(INativeWindow window) {
            _postRenderShader = new ShaderProgram(Shaders.post_render_vertex, Shaders.post_render_fragment);
            _postRenderShader.SetupForPostRender();
            _backgroundShader = new ShaderProgram(Shaders.background_vertex, Shaders.background_fragment);
            _directionalShader = new ShaderProgram(Shaders.post_render_vertex, Shaders.directional_light_fragment);
            _pointShader = new ShaderProgram(Shaders.post_render_vertex, Shaders.point_light_fragment);
            _flashShader = new ShaderProgram(Shaders.post_render_vertex, Shaders.flash_light_fragment);
            _directionalShader.SetupForLight();
            _pointShader.SetupForLight();
            _flashShader.SetupForLight();
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

        public void DrawBackground(Color4 color) {
            _quadVao.Bind();
            _backgroundShader.Use();
            _backgroundShader.SetUVector("background", new Vector4(color.R, color.G, color.B, color.A));
            DrawQuad();
        }

        public void BindPositionAndNormalTextures() {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _positionTexture);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, _normalTexture);
        }

        public void Draw(Vector3 cameraPosition, Vector3 ambientColor, float ambientStrength, bool waterShader, SkyBox skyBox) {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _quadVao.Bind();
            _postRenderShader.Use();
            _postRenderShader.SetUVector("cameraPosition", cameraPosition);
            _postRenderShader.SetUVector("ambientColor", ambientColor);
            _postRenderShader.SetUFloat("ambientStrength", ambientStrength);

            const int min = (CMine.ChunkRadius - 2) << 4;
            const int max = (CMine.ChunkRadius - 1) << 4;
            _postRenderShader.SetUFloat("waterShader", waterShader ? 1 : 0);
            _postRenderShader.SetUFloat("viewDistanceSquared", min * min);
            _postRenderShader.SetUFloat("viewDistanceOffsetSquared", max * max);


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
            GL.ActiveTexture(TextureUnit.Texture5);
            GL.BindTexture(TextureTarget.Texture2D, _ambientBrightness);
            GL.ActiveTexture(TextureUnit.Texture6);
            GL.BindTexture(TextureTarget.Texture2D, _diffuseBrightness);
            GL.ActiveTexture(TextureUnit.Texture7);
            GL.BindTexture(TextureTarget.Texture2D, _specularBrightness);
            GL.ActiveTexture(TextureUnit.Texture8);
            GL.BindTexture(TextureTarget.TextureCubeMap, skyBox.Id);
            DrawQuad();
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
            var images = new int[8];
            GL.GenTextures(images.Length, images);
            _positionTexture = images[0];
            _normalTexture = images[1];
            _ambientTexture = images[2];
            _diffuseTexture = images[3];
            _specularTexture = images[4];
            _ambientBrightness = images[5];
            _diffuseBrightness = images[6];
            _specularBrightness = images[7];

            GL.GenRenderbuffers(1, out _depthBuffer);
            Resize();
        }

        private void Resize() {
            ConfigureTexture(_width, _height, _positionTexture, PixelInternalFormat.Rgb32f, PixelFormat.Rgb);
            ConfigureTexture(_width, _height, _normalTexture, PixelInternalFormat.Rgba32f, PixelFormat.Rgba);
            ConfigureTexture(_width, _height, _ambientTexture, PixelInternalFormat.Rgba16f, PixelFormat.Rgba);
            ConfigureTexture(_width, _height, _diffuseTexture, PixelInternalFormat.Rgb8, PixelFormat.Rgb);
            ConfigureTexture(_width, _height, _specularTexture, PixelInternalFormat.Rgb8, PixelFormat.Rgb);
            ConfigureTexture(_width, _height, _ambientBrightness, PixelInternalFormat.Rgb32f, PixelFormat.Rgb);
            ConfigureTexture(_width, _height, _diffuseBrightness, PixelInternalFormat.Rgb32f, PixelFormat.Rgb);
            ConfigureTexture(_width, _height, _specularBrightness, PixelInternalFormat.Rgb32f, PixelFormat.Rgb);

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
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment5,
                TextureTarget.Texture2D, _ambientBrightness, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment6,
                TextureTarget.Texture2D, _diffuseBrightness, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment7,
                TextureTarget.Texture2D, _specularBrightness, 0);
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
            _quadVao.DrawArrays(0, 6);
        }
    }
}