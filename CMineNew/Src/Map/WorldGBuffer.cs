using System;
using CMineNew.Render;
using CMineNew.Render.Object;
using CMineNew.Resources.Shaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

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

        //Low settings: Kernel 32 Scale 70
        private const int KernelSize = 32;
        private const int SsaoTextureScale = 75;
        private const float Radius = 0.5f;
        private const float Bias = 0.025f;

        private int _id, _ssaoId, _ssaoBlurId;
        private readonly ShaderProgram _postRenderShader, _postRenderWaterShader, _ssaoShader, _ssaoBlurShader;

        private VertexArrayObject _quadVao;
        private int _width, _height;

        private int _depthTexture,
            _normalTexture,
            _albedoTexture,
            _brightnessTexture,
            _ssaoColorTexture,
            _noiseTexture,
            _blurTexture;

        private Vector3[] _ssaoKernel;

        public WorldGBuffer(INativeWindow window) {
            _postRenderShader = new ShaderProgram(Shaders.post_render_vertex, Shaders.post_render_fragment);
            _postRenderShader.SetupForPostRender();
            _postRenderWaterShader = new ShaderProgram(Shaders.post_render_vertex, Shaders.post_render_water_fragment);
            _postRenderWaterShader.SetupForPostRender();
            _ssaoShader = new ShaderProgram(Shaders.ssao_vertex, Shaders.ssao_fragment);
            _ssaoShader.SetupForSSAO();
            _ssaoBlurShader = new ShaderProgram(Shaders.ssao_blur_vertex, Shaders.ssao_blur_fragment);
            _width = window.Width;
            _height = window.Height;
            _quadVao = GenerateQuadVao();
            GenerateTextures();
            GenerateFrameBuffer();
            ConfigureSSAO();
            ConfigureSSAOBlur();
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

        public void BindDefaultFrameBuffer() {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void Draw(Camera camera, Vector3 ambientColor, float ambientStrength, bool waterShader, SkyBox skyBox) {
            _quadVao.Bind();
            UsePostRenderShader(waterShader);
            _postRenderShader.SetUMatrix("invertedViewProjection", camera.InvertedViewProjection);
            _postRenderShader.SetUVector("ambientColor", ambientColor);
            _postRenderShader.SetUFloat("ambientStrength", ambientStrength);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _albedoTexture);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, _depthTexture);
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, _normalTexture);
            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture2D, _brightnessTexture);
            GL.ActiveTexture(TextureUnit.Texture4);
            GL.BindTexture(TextureTarget.Texture2D, _blurTexture);
            GL.ActiveTexture(TextureUnit.Texture5);
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


        public void DrawSSAO(Matrix4 invertedProjection) {
            if (Keyboard.GetState()[Key.B]) {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, _ssaoBlurId);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

                return;
            }

            GL.Viewport(0, 0, _width * SsaoTextureScale / 100, _height * SsaoTextureScale / 100);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _ssaoId);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            _ssaoShader.Use();
            _ssaoShader.SetUInt("kernelSize", KernelSize);
            _ssaoShader.SetUFloat("radius", Radius);
            _ssaoShader.SetUFloat("bias", Bias);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _depthTexture);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, _normalTexture);
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, _noiseTexture);
            _ssaoShader.SetUMatrix("invertedProjection", invertedProjection);
            _quadVao.Bind();
            DrawQuad();

            GL.Viewport(0, 0, _width, _height);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _ssaoBlurId);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            _ssaoBlurShader.Use();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _ssaoColorTexture);
            DrawQuad();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
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
            ConfigureTexture(_width, _height, _depthTexture, PixelInternalFormat.DepthComponent,
                PixelFormat.DepthComponent, PixelType.Float);
            ConfigureTexture(_width, _height, _normalTexture, PixelInternalFormat.Rg16f, PixelFormat.Rg,
                PixelType.Float);
            ConfigureTexture(_width, _height, _albedoTexture, PixelInternalFormat.Rgb16f, PixelFormat.Rgb,
                PixelType.Float);
            ConfigureTexture(_width, _height, _brightnessTexture, PixelInternalFormat.Rgb16f, PixelFormat.Rgb,
                PixelType.Float);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private void GenerateFrameBuffer() {
            GL.GenFramebuffers(1, out _id);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _id);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, _normalTexture, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment1,
                TextureTarget.Texture2D, _albedoTexture, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment2,
                TextureTarget.Texture2D, _brightnessTexture, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                TextureTarget.Texture2D, _depthTexture, 0);
            GL.DrawBuffers(DrawBuffersEnums.Length, DrawBuffersEnums);
            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete) {
                throw new System.Exception("Framebuffer thrown error " + status + ".");
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        private static void ConfigureTexture(int width, int height, int texture,
            PixelInternalFormat internalFormat, PixelFormat outFormat, PixelType pixelType) {
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.PixelStore(PixelStoreParameter.PackAlignment, 1);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) All.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) All.Nearest);
            GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, width, height, 0, outFormat, pixelType,
                IntPtr.Zero);
        }

        private void ConfigureSSAO() {
            var random = new Random();

            GenerateNoiseTexture(random);

            GL.GenFramebuffers(1, out _ssaoId);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _ssaoId);

            GL.GenTextures(1, out _ssaoColorTexture);
            GL.BindTexture(TextureTarget.Texture2D, _ssaoColorTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8,
                _width * SsaoTextureScale / 100, _height * SsaoTextureScale / 100, 0,
                PixelFormat.Red,
                PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) All.Linear);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, _ssaoColorTexture, 0);

            var attachment = DrawBuffersEnum.ColorAttachment0;
            GL.DrawBuffers(1, ref attachment);
            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete) {
                throw new System.Exception("SSAO Framebuffer thrown error " + status + ".");
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            _ssaoKernel = new Vector3[KernelSize];
            for (var i = 0; i < _ssaoKernel.Length; i++) {
                var sample = new Vector3((float) random.NextDouble() * 2.0f - 1,
                                 (float) random.NextDouble() * 2.0f - 1,
                                 (float) random.NextDouble()).Normalized() * (float) random.NextDouble();
                var scale = i / (float) KernelSize;
                _ssaoKernel[i] = sample * Lerp(0.1f, 1.0f, scale * scale);
            }

            _ssaoShader.Use();
            SendKernelSamplesToShader();
        }

        private float Lerp(float a, float b, float f) {
            return a + f * (b - a);
        }

        private void SendKernelSamplesToShader() {
            for (var i = 0; i < _ssaoKernel.Length; i++) {
                _ssaoShader.SetUVector("samples[" + i + "]", _ssaoKernel[i]);
            }
        }


        private void GenerateNoiseTexture(Random random) {
            var noise = new Vector3[16];
            for (var i = 0; i < noise.Length; i++) {
                noise[i] = new Vector3((float) random.NextDouble() * 2.0f - 1,
                    (float) random.NextDouble() * 2.0f - 1, 0.0f);
            }

            GL.GenTextures(1, out _noiseTexture);
            GL.BindTexture(TextureTarget.Texture2D, _noiseTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, 4, 4, 0, PixelFormat.Rgb,
                PixelType.Float, noise);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) All.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) All.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) All.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) All.Repeat);
        }

        public void ConfigureSSAOBlur() {
            GL.GenFramebuffers(1, out _ssaoBlurId);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _ssaoBlurId);
            GL.GenTextures(1, out _blurTexture);
            GL.BindTexture(TextureTarget.Texture2D, _blurTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8, _width, _height, 0, PixelFormat.Red,
                PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) All.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) All.Nearest);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, _blurTexture, 0);

            var attachment = DrawBuffersEnum.ColorAttachment0;
            GL.DrawBuffers(1, ref attachment);
            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete) {
                throw new System.Exception("SSAO blur Framebuffer thrown error " + status + ".");
            }
        }

        #endregion

        private void DrawQuad() {
            _quadVao.DrawArrays(0, 6);
        }
    }
}