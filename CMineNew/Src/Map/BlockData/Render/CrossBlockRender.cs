using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;
using CMineNew.Render.Mapper;
using CMineNew.Render.Object;
using CMineNew.Resources.Shaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Map.BlockData.Render{
    public class CrossBlockRender : BlockRender{
        private const int MaxBlocks = 800;
        private const int InstanceDataLength = 3 + 4 + 4 + 1 + 1;
        private const int InstanceFloatDataLength = InstanceDataLength * sizeof(float);

        public static readonly Vertex[] Vertices = {
            new Vertex(0.1f, 0, 0.1f, -1, -1, -1, 0, 1),
            new Vertex(0.1f, 0, 0.9f, -1, -1, 1, 0, 1),
            new Vertex(0.1f, 1f, 0.1f, -1, 1, -1, 0, 0),
            new Vertex(0.1f, 1f, 0.9f, -1, 1, 1, 0, 0),
            new Vertex(0.9f, 0, 0.1f, 1, -1, -1, 1, 1),
            new Vertex(0.9f, 0, 0.9f, 1, -1, 1, 1, 1),
            new Vertex(0.9f, 1f, 0.1f, 1, 1, -1, 1, 0),
            new Vertex(0.9f, 1f, 0.9f, 1, 1, 1, 1, 0),
        };

        private static readonly int[] Indices = {0, 5, 7, 0, 7, 2, 1, 4, 6, 1, 6, 3};

        private readonly ChunkRegion _chunkRegion;
        private ShaderProgram _shader;
        private VertexArrayObject _vao;
        private VertexBufferObject _dataBuffer;
        private readonly VboMapper<Block> _mapper;
        private bool _generated;

        public CrossBlockRender(ChunkRegion chunkRegion) {
            _chunkRegion = chunkRegion;
            _vao = null;
            _dataBuffer = null;
            _mapper = new BlockVboMapper(chunkRegion, null, null, InstanceDataLength, MaxBlocks, OnResize);
            _generated = false;
        }

        public override void AddData(int mapperIndex, Block block, int blockLight, int sunlight) {
            if (!(block is CrossBlock crossBlock)) return;
            var pos = block.Position;
            var filter = block.TextureFilter;
            var area = crossBlock.TextureArea;
            _mapper.AddTask(new VboMapperTask<Block>(VboMapperTaskType.Add, block,
                new[] {
                    pos.X, pos.Y, pos.Z, area.MinX, area.MinY, area.MaxX, area.MaxY,
                    filter.R, filter.G, filter.B, filter.A, blockLight / Block.MaxBlockLightF,
                    sunlight / Block.MaxBlockLightF
                }, 0));
        }

        public override void RemoveData(int mapperIndex, Block block) {
            _mapper.AddTask(new VboMapperTask<Block>(VboMapperTaskType.Remove, block, null, 0));
        }

        public override void Draw() {
            CheckVbo();
            _shader.Use();
            _vao.Bind();
            _mapper.OnBackground = false;
            _mapper.FlushQueue();
            _vao.DrawnInstanced(_mapper.Amount);
        }

        public override void DrawAfterPostRender() {
        }

        public override void FlushInBackground() {
            CheckVbo();
            _mapper.OnBackground = true;
            _mapper.FlushQueue();
        }

        public override void CleanUp() {
            _vao?.CleanUp();
        }

        private void Generate() {
            _shader = ShaderManager.GetOrCreateShader("cubic_block", Shaders.block_vertex, Shaders.block_fragment);
            _vao = new VertexArrayObject(Vertices, Indices);
            _vao.Bind();
            _dataBuffer = new VertexBufferObject();
            _vao.LinkBuffer(_dataBuffer);
            _dataBuffer.Bind(BufferTarget.ArrayBuffer);
            _dataBuffer.SetData(BufferTarget.ArrayBuffer, MaxBlocks * InstanceFloatDataLength,
                BufferUsageHint.StreamDraw);
            var builder = new AttributePointerBuilder(_vao, InstanceDataLength, 3);
            builder.AddPointer(3, true);
            builder.AddPointer(4, true);
            builder.AddPointer(4, true);
            builder.AddPointer(1, true);
            builder.AddPointer(1, true);
            VertexBufferObject.Unbind(BufferTarget.ArrayBuffer);
            VertexArrayObject.Unbind();

            _mapper.Vao = _vao;
            _mapper.Vbo = _dataBuffer;
        }

        private void OnResize(VertexArrayObject vao, VertexBufferObject oldBuffer, VertexBufferObject newBuffer) {
            vao.LinkBuffer(newBuffer);
            vao.UnlinkBuffer(oldBuffer);

            vao.Bind();

            newBuffer.Bind(BufferTarget.ArrayBuffer);
            var builder = new AttributePointerBuilder(vao, InstanceDataLength, 3);
            builder.AddPointer(3, true);
            builder.AddPointer(4, true);
            builder.AddPointer(4, true);
            builder.AddPointer(1, true);
            builder.AddPointer(1, true);
            VertexBufferObject.Unbind(BufferTarget.ArrayBuffer);
        }

        private void CheckVbo() {
            if (!_generated) {
                Generate();
                _generated = true;
            }
        }
    }
}