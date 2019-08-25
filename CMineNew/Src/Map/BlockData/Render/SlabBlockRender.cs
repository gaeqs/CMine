using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;
using CMineNew.Render.Mapper;
using CMineNew.Render.Object;
using CMineNew.Resources.Shaders;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Map.BlockData.Render{
    public class SlabBlockRender : BlockRender{
        private const int MaxFaces = 50;
        private const int InstanceDataLength = 3 + 4 + 4 + 1;
        private const int InstanceFloatDataLength = sizeof(float) * InstanceDataLength;

        private const float SlabHeight = 0.5f;

        private readonly ChunkRegion _chunkRegion;
        private ShaderProgram _shader;
        private readonly VertexArrayObject[] _vaos;
        private readonly VertexBufferObject[] _dataBuffers;
        private readonly VboMapper<Vector3i>[] _mappers;
        private bool _generated;

        public SlabBlockRender(ChunkRegion chunkRegion) {
            _chunkRegion = chunkRegion;

            _vaos = new VertexArrayObject[6];
            _dataBuffers = new VertexBufferObject[6];
            _mappers = new VboMapper<Vector3i>[6];
            _generated = false;

            foreach (var face in BlockFaceMethods.All) {
                _mappers[(int) face] = new VboMapper<Vector3i>(null, null, InstanceDataLength, MaxFaces, OnResize);
            }
        }

        public ChunkRegion ChunkRegion => _chunkRegion;

        public override void AddData(int mapperIndex, Block block) {
            if (!(block is SlabBlock slabBlock)) return;
            var mapper = _mappers[mapperIndex];
            var pos = block.Position;
            var filter = block.TextureFilter;
            var area = slabBlock.GetTextureArea((BlockFace) mapperIndex);
            mapper.AddTask(new VboMapperTask<Vector3i>(VboMapperTaskType.Add, block.Position,
                new[] {
                    pos.X, pos.Y, pos.Z, area.MinX, area.MinY, area.MaxX, area.MaxY,
                    filter.R, filter.G, filter.B, filter.A, slabBlock.Upside ? 1 : 0
                }, 0));
        }

        public override void RemoveData(int mapperIndex, Block block) {
            var mapper = _mappers[mapperIndex];
            mapper.AddTask(new VboMapperTask<Vector3i>(VboMapperTaskType.Remove, block.Position, null, 0));
        }

        public override void Draw() {
            CheckVbos();
            _shader.Use();
            _shader.SetUMatrix("viewProjection", _chunkRegion.World.Camera.ViewProjection);
            foreach (var face in BlockFaceMethods.All) {
                var vao = _vaos[(int) face];
                var mapper = _mappers[(int) face];
                vao.Bind();
                mapper.OnBackground = false;
                mapper.FlushQueue();
                vao.DrawnInstanced(mapper.Amount);
            }
        }

        public override void DrawAfterPostRender() {
        }

        public override void FlushInBackground() {
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
            _shader = ShaderManager.GetOrCreateShader("slab_block", Shaders.slab_vertex, Shaders.slab_fragment);
            foreach (var face in BlockFaceMethods.All) {
                var vao = BlockFaceVertices.CreateVao(face, SlabHeight);
                vao.Bind();
                var vbo = new VertexBufferObject();
                vao.LinkBuffer(vbo);
                vbo.Bind(BufferTarget.ArrayBuffer);
                vbo.SetData(BufferTarget.ArrayBuffer, MaxFaces * InstanceFloatDataLength, BufferUsageHint.StreamDraw);
                var builder = new AttributePointerBuilder(vao, InstanceDataLength, 3);
                builder.AddPointer(3, true);
                builder.AddPointer(4, true);
                builder.AddPointer(4, true);
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
            VertexBufferObject.Unbind(BufferTarget.ArrayBuffer);
        }

        private void CheckVbos() {
            if (!_generated) {
                Generate();
                _generated = true;
            }
        }
    }
}