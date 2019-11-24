using CMineNew.Map.BlockData.Sketch;
using CMineNew.Render.Mapper;
using CMineNew.Render.Object;
using CMineNew.Resources.Shaders;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Map.BlockData.Render{
    public class CubicBlockRender : BlockRender{
        private const int MaxFaces = 6000;
        private const int InstanceDataLength = 3 + 4 + 1 + 1 + 1;
        private const int InstanceFloatDataLength = sizeof(float) * InstanceDataLength;

        private readonly ChunkRegion _chunkRegion;
        private ShaderProgram _shader;
        private readonly VertexArrayObject[] _vaos;
        private readonly VertexBufferObject[] _dataBuffers;
        private readonly VboMapper<Block>[] _mappers;
        private bool _generated;

        public CubicBlockRender(ChunkRegion chunkRegion) {
            _chunkRegion = chunkRegion;

            _vaos = new VertexArrayObject[6];
            _dataBuffers = new VertexBufferObject[6];
            _mappers = new VboMapper<Block>[6];
            _generated = false;

            foreach (var face in BlockFaceMethods.All) {
                _mappers[(int) face] = new ArrayBlockVboMapper(_chunkRegion, null, null,
                    InstanceDataLength, MaxFaces);
            }
        }

        public ChunkRegion ChunkRegion => _chunkRegion;

        public override void AddData(int mapperIndex, Block block, int blockLight, int sunlight) {
            if (!(block is CubicBlock cubicBlock)) return;
            var mapper = _mappers[mapperIndex];
            var pos = block.Position;
            var filter = block.TextureFilter;
            var index = cubicBlock.GetTextureIndex((BlockFace) mapperIndex);
            mapper.AddTask(new VboMapperTask<Block>(VboMapperTaskType.Add, block,
                new[] {
                    pos.X, pos.Y, pos.Z, index,
                    filter.ValueF,
                    blockLight / Block.MaxBlockLightF,
                    sunlight / Block.MaxBlockLightF
                }, 0));
        }

        public override void RemoveData(int mapperIndex, Block block) {
            var mapper = _mappers[mapperIndex];
            mapper.AddTask(new VboMapperTask<Block>(VboMapperTaskType.Remove, block, null, 0));
        }

        public override void Draw(bool first) {
            CheckVbos();
            _shader.Use();
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            foreach (var face in BlockFaceMethods.All) {
                var mapper = _mappers[(int) face];
                var vao = _vaos[(int) face];
                vao.Bind();
                mapper.OnBackground = false;
                mapper.FlushQueue();
                vao.DrawnInstanced(mapper.Amount);
            }
        }

        public override void DrawAfterPostRender(bool first) {
        }

        public override void CleanUp() {
            foreach (var vertexArrayObject in _vaos) {
                vertexArrayObject?.CleanUp();
            }
        }

        private void Generate() {
            _shader = ShaderManager.GetOrCreateShader("cubic_block", Shaders.block_vertex, Shaders.block_fragment);
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