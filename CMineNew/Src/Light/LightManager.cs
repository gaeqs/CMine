using System;
using System.Collections.Generic;
using CMineNew.Map;
using CMineNew.Render;
using CMineNew.Render.Mapper;
using CMineNew.Render.Object;
using CMineNew.Resources.Shaders;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Light{
    /// <summary>
    /// Represents a manager that stores all lights in a world.
    /// </summary>
    public class LightManager{
        private readonly VertexArrayObject _directionalVao, _pointVao, _flashVao;
        private readonly ShaderProgram _directionalShader, _pointShader, _flashShader;
        private readonly VboMapper<DirectionalLight> _directionalMapper;
        private readonly VboMapper<PointLight> _pointMapper;
        private readonly VboMapper<FlashLight> _flashMapper;

        private readonly List<DirectionalLight> _directionalLights;
        private readonly List<PointLight> _pointLights;
        private readonly List<FlashLight> _flashLights;

        /// <summary>
        /// Creates a light manager.
        /// </summary>
        public LightManager() {
            _directionalLights = new List<DirectionalLight>();
            _pointLights = new List<PointLight>();
            _flashLights = new List<FlashLight>();

            _directionalVao = WorldGBuffer.GenerateQuadVao();
            _pointVao = WorldGBuffer.GenerateQuadVao();
            _flashVao = WorldGBuffer.GenerateQuadVao();
            _directionalShader =
                new ShaderProgram(Shaders.directional_light_vertex, Shaders.directional_light_fragment);
            _pointShader = new ShaderProgram(Shaders.point_light_vertex, Shaders.point_light_fragment);
            _flashShader = new ShaderProgram(Shaders.flash_light_vertex, Shaders.flash_light_fragment);

            _directionalShader.SetupForLight();
            _pointShader.SetupForLight();
            _flashShader.SetupForLight();

            var directionVbo = new VertexBufferObject();
            var pointVbo = new VertexBufferObject();
            var flashVbo = new VertexBufferObject();
            _directionalVao.LinkBuffer(directionVbo);
            _pointVao.LinkBuffer(pointVbo);
            _flashVao.LinkBuffer(flashVbo);
            _directionalMapper = new VboMapper<DirectionalLight>(directionVbo, _directionalVao, 12,
                1000, (o, buffer, newBuffer) => { });
            _pointMapper = new VboMapper<PointLight>(pointVbo, _pointVao, 15,
                1000, (o, buffer, newBuffer) => { });
            _flashMapper = new VboMapper<FlashLight>(flashVbo, _flashVao, 20, 1000,
                (o, buffer, newBuffer) => { });

            _directionalVao.Bind();
            directionVbo.Bind(BufferTarget.ArrayBuffer);
            directionVbo.SetData(BufferTarget.ArrayBuffer, 1000 * 12 * sizeof(float), BufferUsageHint.StreamDraw);
            var builder = new AttributePointerBuilder(_directionalVao, 12, 2);
            builder.AddPointer(3, true);
            builder.AddPointer(3, true);
            builder.AddPointer(3, true);
            builder.AddPointer(3, true);

            _pointVao.Bind();
            pointVbo.Bind(BufferTarget.ArrayBuffer);
            pointVbo.SetData(BufferTarget.ArrayBuffer, 1000 * 15 * sizeof(float), BufferUsageHint.StreamDraw);
            builder = new AttributePointerBuilder(_pointVao, 15, 2);
            builder.AddPointer(3, true);
            builder.AddPointer(3, true);
            builder.AddPointer(3, true);
            builder.AddPointer(3, true);
            builder.AddPointer(1, true);
            builder.AddPointer(1, true);
            builder.AddPointer(1, true);

            _flashVao.Bind();
            flashVbo.Bind(BufferTarget.ArrayBuffer);
            flashVbo.SetData(BufferTarget.ArrayBuffer, 1000 * 20 * sizeof(float), BufferUsageHint.StreamDraw);
            builder = new AttributePointerBuilder(_flashVao, 20, 2);
            builder.AddPointer(3, true);
            builder.AddPointer(3, true);
            builder.AddPointer(3, true);
            builder.AddPointer(3, true);
            builder.AddPointer(3, true);
            builder.AddPointer(1, true);
            builder.AddPointer(1, true);
            builder.AddPointer(1, true);
            builder.AddPointer(1, true);
            builder.AddPointer(1, true);

            VertexArrayObject.Unbind();
            VertexBufferObject.Unbind(BufferTarget.ArrayBuffer);
        }

        public void AddDirectionalLight(DirectionalLight light) {
            _directionalLights.Add(light);
            _directionalMapper.AddTask(new VboMapperTask<DirectionalLight>(VboMapperTaskType.Add,
                light, light.ToData(), 0));
        }

        public void AddPointLight(PointLight light) {
            _pointLights.Add(light);
            _pointMapper.AddTask(new VboMapperTask<PointLight>(VboMapperTaskType.Add,
                light, light.ToData(), 0));
        }

        public void AddFlashLight(FlashLight light) {
            _flashLights.Add(light);
            _flashMapper.AddTask(new VboMapperTask<FlashLight>(VboMapperTaskType.Add,
                light, light.ToData(), 0));
        }

        /// <summary>
        /// The mutable list where directional lights are stored.
        /// </summary>
        public List<DirectionalLight> DirectionalLights => _directionalLights;

        /// <summary>
        /// The mutable list where point lights are stored.
        /// </summary>
        public List<PointLight> PointLights => _pointLights;

        /// <summary>
        /// The mutable list where flash lights are stored.
        /// </summary>
        public List<FlashLight> FlashLights => _flashLights;

        public void Draw(Camera camera, WorldGBuffer gBuffer) {
            gBuffer.Bind();
            GL.Disable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.One);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            gBuffer.BindPositionAndNormalTextures();

            _directionalVao.Bind();
            _directionalShader.Use();
            _directionalShader.SetUVector("cameraPosition", camera.Position);
            _directionalMapper.FlushQueue();
            _directionalVao.DrawnArraysInstanced(0, 6, _directionalMapper.Amount);

            _pointVao.Bind();
            _pointShader.Use();
            _pointShader.SetUVector("cameraPosition", camera.Position);
            _pointMapper.FlushQueue();
            _pointVao.DrawnArraysInstanced(0, 6, _pointMapper.Amount);

            _flashVao.Bind();
            _flashShader.Use();
            _flashShader.SetUVector("cameraPosition", camera.Position);
            _flashMapper.FlushQueue();
            _flashVao.DrawnArraysInstanced(0, 6, _flashMapper.Amount);
        }
    }
}