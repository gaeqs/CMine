using CMineNew.Color;
using CMineNew.Map.BlockData.Type;
using CMineNew.Render.Mapper;
using CMineNew.Render.Object;
using CMineNew.Resources.Shaders;
using CMineNew.Util;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Map.BlockData.Render{
    public class WaterBlockRender : BlockRender{
        private const int MaxFaces = 750;
        private const int InstanceDataLength = 3 + 4 + 4 + 1 + 1 + 4;
        private const int InstanceFloatDataLength = sizeof(float) * InstanceDataLength;

        private readonly ChunkRegion _chunkRegion;
        private ShaderProgram _shader, _waterShader;
        private readonly VertexArrayObject[] _vaos;
        private readonly VertexBufferObject[] _dataBuffers;
        private readonly VboMapper<Block>[] _mappers;
        private bool _generated;

        public WaterBlockRender(ChunkRegion chunkRegion) {
            _chunkRegion = chunkRegion;

            _vaos = new VertexArrayObject[6];
            _dataBuffers = new VertexBufferObject[6];
            _mappers = new VboMapper<Block>[6];
            _generated = false;

            foreach (var face in BlockFaceMethods.All) {
                _mappers[(int) face] = new ArrayBlockVboMapper(chunkRegion, null, null,
                    InstanceDataLength, MaxFaces);
            }
        }

        public ChunkRegion ChunkRegion => _chunkRegion;

        public override void AddData(int mapperIndex, Block block, int blockLight, int sunlight) {
            if (!(block is BlockWater water)) return;
            var mapper = _mappers[mapperIndex];
            var pos = BitUtils.IntBitsToFloat(block.Position);
            var filter = block.TextureFilter;
            var index = BlockWater.TextureIndex;
            var levels = water.VertexWaterLevel;
            var t = water.HasWaterOnTop;

            var waterLevel = new Rgba32I((byte) (t ? 8 : levels[0]), (byte) (t ? 8 : levels[1]),
                (byte) (t ? 8 : levels[2]),
                (byte) (t ? 8 : levels[3]));

            mapper.AddTask(new VboMapperTask<Block>(VboMapperTaskType.Add, block,
                new[] {
                    pos.X, pos.Y, pos.Z, index,
                    filter.ValueF, blockLight / Block.MaxBlockLightF,
                    sunlight / Block.MaxBlockLightF, waterLevel.ValueF
                }, 0));
        }

        public override void RemoveData(int mapperIndex, Block block) {
            var mapper = _mappers[mapperIndex];
            mapper.AddTask(new VboMapperTask<Block>(VboMapperTaskType.Remove, block, null, 0));
        }

        public override void Draw(bool first) {
        }

        public override void DrawAfterPostRender(bool first) {
            CheckVbos();
            if (_chunkRegion.World.Player.EyesOnWater) {
                _waterShader.Use();
            }
            else {
                _shader.Use();
            }

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, CMine.TextureMap.Texture);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.TextureCubeMap, _chunkRegion.World.RenderData.SkyBox.Id);


            GL.Disable(EnableCap.CullFace);

            foreach (var face in BlockFaceMethods.All) {
                var mapper = _mappers[(int) face];
                var vao = _vaos[(int) face];
                vao.Bind();
                mapper.OnBackground = false;
                mapper.FlushQueue();
                vao.DrawnInstanced(mapper.Amount);
            }
        }

        public virtual void FlushInBackground() {
            CheckVbos();
            foreach (var mapper in _mappers) {
                mapper.OnBackground = true;
                mapper.FlushQueue();
            }
        }

        public override void CleanUp() {
            foreach (var vertexArrayObject in _vaos) {
                vertexArrayObject?.CleanUp();
            }
        }

        private void Generate() {
            _shader = ShaderManager.GetOrCreateShader("water_block", Shaders.water_vertex, Shaders.water_fragment);
            _waterShader = ShaderManager.GetOrCreateShader("water_water_block", Shaders.water_vertex,
                Shaders.water_water_fragment);
            _shader.Use();
            _shader.SetupForWater();
            _waterShader.Use();
            _waterShader.SetupForWater();
            foreach (var face in BlockFaceMethods.All) {
                var vao = BlockFaceVertices.CreateVao(face);
                vao.Bind();
                var vbo = new VertexBufferObject();
                vao.LinkBuffer(vbo);
                vbo.Bind(BufferTarget.ArrayBuffer);
                vbo.SetData(BufferTarget.ArrayBuffer, MaxFaces * InstanceFloatDataLength, BufferUsageHint.StreamDraw);
                var builder = new AttributePointerBuilder(vao, InstanceDataLength, 3);
                builder.AddPointer(3, true);
                builder.AddPointer(1, true);
                builder.AddPointer(1, true);
                builder.AddPointer(1, true);
                builder.AddPointer(1, true);
                builder.AddPointer(1, true);
                VertexBufferObject.Unbind(BufferTarget.ArrayBuffer);
                VertexArrayObject.Unbind();

                var i = (int) face;

                _vaos[i] = vao;
                _dataBuffers[i] = vbo;
                var mapper = _mappers[i];
                mapper.Vao = vao;
                mapper.Vbo = vbo;
            }
        }

        private void CheckVbos() {
            if (!_generated) {
                Generate();
                _generated = true;
            }
        }
    }
}