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
        private VertexArrayObject _directionalVao, _pointVao, _flashVao;
        private readonly ShaderProgram _directionalShader, _pointShader, _flashShader;
        private VboMapper<DirectionalLight> _directionalMapper;
        private VboMapper<PointLight> _pointMapper;
        private VboMapper<FlashLight> _flashMapper;

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


            _directionalShader =
                new ShaderProgram(Shaders.directional_light_vertex, Shaders.directional_light_fragment);
            _pointShader = new ShaderProgram(Shaders.point_light_vertex, Shaders.point_light_fragment);
            _flashShader = new ShaderProgram(Shaders.flash_light_vertex, Shaders.flash_light_fragment);

            _directionalShader.SetupForLight();
            _pointShader.SetupForLight();
            _flashShader.SetupForLight();

            ConfigureDirectionalLights();
            ConfigurePointLights();
            ConfigureFlashLights();

            VertexArrayObject.Unbind();
            VertexBufferObject.Unbind(BufferTarget.ArrayBuffer);
        }


        /// <summary>
        /// Adds a directional light.
        /// </summary>
        /// <param name="light">The light.</param>
        public void AddDirectionalLight(DirectionalLight light) {
            _directionalLights.Add(light);
            _directionalMapper.AddTask(new VboMapperTask<DirectionalLight>(VboMapperTaskType.Add,
                light, light.ToData(), 0));
        }

        /// <summary>
        /// Removes a directional light.
        /// </summary>
        /// <param name="light">The light.</param>
        public void RemoveDirectionalLight(DirectionalLight light) {
            if (_directionalLights.Remove(light)) {
                _directionalMapper.AddTask(new VboMapperTask<DirectionalLight>(VboMapperTaskType.Remove,
                    light, null, 0));
            }
        }

        /// <summary>
        /// Adds a point light.
        /// </summary>
        /// <param name="light">The light.</param>
        public void AddPointLight(PointLight light) {
            _pointLights.Add(light);
            _pointMapper.AddTask(new VboMapperTask<PointLight>(VboMapperTaskType.Add,
                light, light.Data, 0));
        }

        /// <summary>
        /// Removes a point light.
        /// </summary>
        /// <param name="light">The light.</param>
        public void RemovePointLight(PointLight light) {
            if (_pointLights.Remove(light)) {
                _pointMapper.AddTask(new VboMapperTask<PointLight>(VboMapperTaskType.Remove,
                    light, null, 0));
            }
        }

        /// <summary>
        /// Adds a flash light.
        /// </summary>
        /// <param name="light">The light.</param>
        public void AddFlashLight(FlashLight light) {
            _flashLights.Add(light);
            _flashMapper.AddTask(new VboMapperTask<FlashLight>(VboMapperTaskType.Add,
                light, light.ToData(), 0));
        }

        /// <summary>
        /// Removes a flash light.
        /// </summary>
        /// <param name="light">The light.</param>
        public void RemoveFlashLight(FlashLight light) {
            if (_flashLights.Remove(light)) {
                _flashMapper.AddTask(new VboMapperTask<FlashLight>(VboMapperTaskType.Remove,
                    light, null, 0));
            }
        }

        /// <summary>
        /// The list where directional lights are stored.
        /// To edit this list, use the methods AddDirectionalLight() and RemoveDirectionalLight();
        /// </summary>
        public List<DirectionalLight> DirectionalLights => _directionalLights;

        /// <summary>
        /// The list where point lights are stored.
        /// To edit this list, use the methods AddPointLight() and RemovePointLight();
        /// </summary>
        public List<PointLight> PointLights => _pointLights;

        /// <summary>
        /// The list where flash lights are stored.
        /// To edit this list, use the methods AddFlashLight() and RemoveFlashLight();
        /// </summary>
        public List<FlashLight> FlashLights => _flashLights;

        /// <summary>
        /// Draws all lights into the WorldGBuffer.
        /// </summary>
        /// <param name="camera">The camera of the world.</param>
        /// <param name="gBuffer">The GBuffer.</param>
        public void Draw(Camera camera, WorldGBuffer gBuffer) {
            gBuffer.Bind();
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(false);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.One);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            gBuffer.BindPositionAndNormalTextures();

            var inverseViewProjection = camera.ViewProjection.Inverted();

            _directionalVao.Bind();
            _directionalShader.Use();
            _directionalShader.SetUVector("cameraPosition", camera.Position);
            _directionalShader.SetUMatrix("invertedViewProjection", inverseViewProjection);
            _directionalMapper.FlushQueue();
            _directionalVao.DrawnArraysInstanced(0, 6, _directionalMapper.Amount);

            _pointVao.Bind();
            _pointShader.Use();
            _pointShader.SetUVector("cameraPosition", camera.Position);
            _pointShader.SetUMatrix("invertedViewProjection", inverseViewProjection);
            _pointShader.SetUInt("lightAmount", _pointLights.Count);
            _pointMapper.FlushQueue();
            _pointVao.DrawArrays(0, 6);

            _flashVao.Bind();
            _flashShader.Use();
            _flashShader.SetUVector("cameraPosition", camera.Position);
            _flashShader.SetUMatrix("invertedViewProjection", inverseViewProjection);
            _flashMapper.FlushQueue();
            _flashVao.DrawnArraysInstanced(0, 6, _flashMapper.Amount);
        }

        /// <summary>
        /// Configures all OpenGL objects necessary to render directional lights.
        /// </summary>
        private void ConfigureDirectionalLights() {
            _directionalVao = WorldGBuffer.GenerateQuadVao();
            var directionVbo = new VertexBufferObject();
            _directionalVao.LinkBuffer(directionVbo);
            _directionalMapper = new VboMapper<DirectionalLight>(directionVbo, _directionalVao, 12,
                1000, (o, buffer, newBuffer) => { });
            _directionalVao.Bind();
            directionVbo.Bind(BufferTarget.ArrayBuffer);
            directionVbo.SetData(BufferTarget.ArrayBuffer, 1000 * 12 * sizeof(float), BufferUsageHint.StreamDraw);
            var builder = new AttributePointerBuilder(_directionalVao, 12, 2);
            builder.AddPointer(3, true);
            builder.AddPointer(3, true);
            builder.AddPointer(3, true);
            builder.AddPointer(3, true);
        }

        /// <summary>
        /// Configures all OpenGL objects necessary to render point lights.
        /// </summary>
        private void ConfigurePointLights() {
            const int structSize = 20;
            const int structBytes = structSize * 4;
            _pointVao = WorldGBuffer.GenerateQuadVao();
            var uniformBuffer = new VertexBufferObject();
            uniformBuffer.BindBase(BufferTarget.ShaderStorageBuffer, 0);
            uniformBuffer.SetData(BufferTarget.ShaderStorageBuffer, structBytes * 2000, BufferUsageHint.DynamicDraw);
            var loc = GL.GetProgramResourceIndex(_pointShader.Id, ProgramInterface.ShaderStorageBlock, "LightsBlock");
            GL.ShaderStorageBlockBinding(_pointShader.Id, loc, 0);
            _pointMapper = new VboMapper<PointLight>(uniformBuffer, _pointVao, structSize, 2000,
                (o, buffer, newBuffer) => { }, BufferTarget.ShaderStorageBuffer);
        }

        /// <summary>
        /// Configures all OpenGL objects necessary to render flash lights.
        /// </summary>
        private void ConfigureFlashLights() {
            _flashVao = WorldGBuffer.GenerateQuadVao();
            var flashVbo = new VertexBufferObject();
            _flashVao.LinkBuffer(flashVbo);
            _flashMapper = new VboMapper<FlashLight>(flashVbo, _flashVao, 20, 1000,
                (o, buffer, newBuffer) => { });
            _flashVao.Bind();
            flashVbo.Bind(BufferTarget.ArrayBuffer);
            flashVbo.SetData(BufferTarget.ArrayBuffer, 1000 * 20 * sizeof(float), BufferUsageHint.StreamDraw);
            var builder = new AttributePointerBuilder(_flashVao, 20, 2);
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
        }
    }
}