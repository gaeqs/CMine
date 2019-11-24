using CMineNew.Map.BlockData.Type;
using CMineNew.Render.Mapper;
using CMineNew.Render.Object;
using CMineNew.Resources.Shaders;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Map.BlockData.Render{
    public class TorchBlockRender : BlockRender{
        private const int MaxBlocks = 800;
        private const int InstanceDataLength = 3 + 4 + 1 + 1 + 1;
        private const int InstanceFloatDataLength = InstanceDataLength * sizeof(float);

        public static readonly Vertex[] Vertices = {
            //North 0
            new Vertex(0, 0, 0, 0, 0, -1, 2f / 16, 1),
            new Vertex(0, 1, 0, 0, 0, -1, 2f / 16, 1 - 9f / 16),
            new Vertex(1, 0, 0, 0, 0, -1, 0, 1),
            new Vertex(1, 1, 0, 0, 0, -1, 0, 1 - 9f / 16),

            //South 4
            new Vertex(0, 0, 1, 0, 0, 1, 0, 1),
            new Vertex(0, 1, 1, 0, 0, 1, 0, 1 - 9f / 16),
            new Vertex(1, 0, 1, 0, 0, 1, 2f / 16, 1),
            new Vertex(1, 1, 1, 0, 0, 1, 2f / 16, 1 - 9f / 16),

            //West 8
            new Vertex(0, 0, 0, -1, 0, 0, 0, 1),
            new Vertex(0, 0, 1, -1, 0, 0, 2f / 16, 1),
            new Vertex(0, 1, 0, -1, 0, 0, 0, 1 - 9f / 16),
            new Vertex(0, 1, 1, -1, 0, 0, 2f / 16, 1 - 9f / 16),

            //East 12
            new Vertex(1, 0, 0, 1, 0, 0, 2f / 16, 1),
            new Vertex(1, 0, 1, 1, 0, 0, 0, 1),
            new Vertex(1, 1, 0, 1, 0, 0, 2f / 16, 1 - 9f / 16),
            new Vertex(1, 1, 1, 1, 0, 0, 0, 1 - 9f / 16),

            //Bottom 16
            new Vertex(0, 0, 0, 0, -1, 0, 0, 1 - 2f / 16),
            new Vertex(0, 0, 1, 0, -1, 0, 0, 1),
            new Vertex(1, 0, 0, 0, -1, 0, 2f / 16, 1 - 2f / 16),
            new Vertex(1, 0, 1, 0, -1, 0, 2f / 16, 1),

            //Top 20
            new Vertex(0, 1, 0, 0, 1, 0, 0, 1 - 11f / 16),
            new Vertex(0, 1, 1, 0, 1, 0, 0, 1 - 9f / 16),
            new Vertex(1, 1, 0, 0, 1, 0, 2f / 16, 1 - 11f / 16),
            new Vertex(1, 1, 1, 0, 1, 0, 2f / 16, 1 - 9f / 16),
        };

        private static readonly int[] Indices = {
            0, 2, 3, 0, 3, 1,
            4, 6, 7, 4, 7, 5,
            8, 9, 11, 8, 11, 10,
            12, 13, 15, 12, 15, 14,
            16, 18, 19, 16, 19, 17,
            20, 22, 23, 20, 23, 21
        };

        private readonly ChunkRegion _chunkRegion;
        private ShaderProgram _shader;
        private VertexArrayObject _vao;
        private VertexBufferObject _dataBuffer;
        private readonly VboMapper<Block> _mapper;
        private bool _generated;

        public TorchBlockRender(ChunkRegion chunkRegion) {
            _chunkRegion = chunkRegion;
            _vao = null;
            _dataBuffer = null;
            _mapper = new ArrayBlockVboMapper(chunkRegion, null, null, InstanceDataLength, MaxBlocks);
            _generated = false;
        }

        private Vertex[] GenTransformedVertices() {
            const float minX = 7 / 16f;
            const float maxX = 9 / 16f;
            const float minY = 0;
            const float maxY = 9 / 16f;
            const float minZ = 7 / 16f;
            const float maxZ = 9 / 16f;
            var array = new Vertex[Vertices.Length];

            for (var i = 0; i < array.Length; i++) {
                array[i] = Vertices[i];
                array[i]._position.X = array[i]._position.X * (maxX - minX) + minX;
                array[i]._position.Y = array[i]._position.Y * (maxY - minY) + minY;
                array[i]._position.Z = array[i]._position.Z * (maxZ - minZ) + minZ;
            }

            return array;
        }

        public override void AddData(int mapperIndex, Block block, int blockLight, int sunlight) {
            if (!(block is BlockTorch torch)) return;
            var pos = block.Position;
            var filter = block.TextureFilter;
            var index = torch.StaticData.GetTextureIndex(BlockFace.Up);
            _mapper.AddTask(new VboMapperTask<Block>(VboMapperTaskType.Add, block,
                new[] {
                    pos.X, pos.Y, index,
                    filter.ValueF,
                    blockLight / Block.MaxBlockLightF,
                    sunlight / Block.MaxBlockLightF
                }, 0));
        }

        public override void RemoveData(int mapperIndex, Block block) {
            _mapper.AddTask(new VboMapperTask<Block>(VboMapperTaskType.Remove, block, null, 0));
        }

        public override void Draw(bool first) {
            CheckVbo();
            _shader.Use();
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            _vao.Bind();
            _mapper.OnBackground = false;
            _mapper.FlushQueue();
            _vao.DrawnInstanced(_mapper.Amount);
        }

        public override void DrawAfterPostRender(bool first) {
        }

        public virtual void FlushInBackground() {
            CheckVbo();
            _mapper.OnBackground = true;
            _mapper.FlushQueue();
        }

        public override void CleanUp() {
            _vao?.CleanUp();
        }

        private void Generate() {
            _shader = ShaderManager.GetOrCreateShader("cubic_block", Shaders.block_vertex, Shaders.block_fragment);
            _vao = new VertexArrayObject(GenTransformedVertices(), Indices);
            _vao.Bind();
            _dataBuffer = new VertexBufferObject();
            _vao.LinkBuffer(_dataBuffer);
            _dataBuffer.Bind(BufferTarget.ArrayBuffer);
            _dataBuffer.SetData(BufferTarget.ArrayBuffer, MaxBlocks * InstanceFloatDataLength,
                BufferUsageHint.StreamDraw);
            var builder = new AttributePointerBuilder(_vao, InstanceDataLength, 3);
            builder.AddPointer(3, true);
            builder.AddPointer(1, true);
            builder.AddPointer(1, true);
            builder.AddPointer(1, true);
            builder.AddPointer(1, true);
            VertexBufferObject.Unbind(BufferTarget.ArrayBuffer);
            VertexArrayObject.Unbind();

            _mapper.Vao = _vao;
            _mapper.Vbo = _dataBuffer;
        }

        private void CheckVbo() {
            if (!_generated) {
                Generate();
                _generated = true;
            }
        }
    }
}