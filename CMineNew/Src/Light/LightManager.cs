using System.Collections.Generic;
using CMineNew.Map;
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
            _directionalShader = new ShaderProgram(Shaders.post_render_vertex, Shaders.directional_light_fragment);
            _pointShader = new ShaderProgram(Shaders.post_render_vertex, Shaders.point_light_fragment);
            _flashShader = new ShaderProgram(Shaders.post_render_vertex, Shaders.flash_light_fragment);

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
            _flashMapper = new VboMapper<FlashLight>(pointVbo, _pointVao, 20, 1000,
                (o, buffer, newBuffer) => { });

            _directionalVao.Bind();
            directionVbo.Bind(BufferTarget.ArrayBuffer);
            var builder = new AttributePointerBuilder(_directionalVao, 12, 0);
            builder.AddPointer(3, true);
            builder.AddPointer(3, true);
            builder.AddPointer(3, true);
            builder.AddPointer(3, true);
            
            _pointVao.Bind();
            pointVbo.Bind(BufferTarget.ArrayBuffer);
            builder = new AttributePointerBuilder(_pointVao, 15, 0);
            builder.AddPointer(3, true);
            builder.AddPointer(3, true);
            builder.AddPointer(3, true);
            builder.AddPointer(3, true);
            builder.AddPointer(1, true);
            builder.AddPointer(1, true);
            builder.AddPointer(1, true);
            
            _flashVao.Bind();
            flashVbo.Bind(BufferTarget.ArrayBuffer);
            builder = new AttributePointerBuilder(_flashVao, 15, 0);
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

        public void Draw() {
            _directionalVao.Bind();
            _directionalShader.Use();
            _directionalMapper.FlushQueue();
            _directionalVao.DrawnInstanced(_directionalMapper.Amount);
            
            _pointVao.Bind();
            _pointShader.Use();
            _pointMapper.FlushQueue();
            _pointVao.DrawnInstanced(_pointMapper.Amount);
            
            _flashVao.Bind();
            _flashShader.Use();
            _flashMapper.FlushQueue();
            _flashVao.DrawnInstanced(_flashMapper.Amount);
        }
    }
}