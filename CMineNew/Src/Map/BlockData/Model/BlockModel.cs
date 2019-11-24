using CMineNew.Collision;
using CMineNew.Map.BlockData.Render;
using CMineNew.Render;
using CMineNew.Render.Object;
using CMineNew.Resources.Shaders;

namespace CMineNew.Map.BlockData.Model{
    public abstract class BlockModel{
        public static readonly ShaderProgram BlockLinesShaderProgram =
            new ShaderProgram(Shaders.block_lines_vertex, Shaders.block_lines_fragment);

        private string _id;
        private readonly Aabb _blockCollision;
        protected VertexArrayObject _vertexArrayObject;
        protected ShaderProgram _shader;

        public BlockModel(string id, Aabb blockCollision) {
            _id = id;
            _blockCollision = blockCollision;
        }
        
        public string Id => _id;

        public Aabb BlockCollision => _blockCollision;

        public VertexArrayObject VertexArrayObject => _vertexArrayObject;

        public abstract uint FloatsPerBlock { get; }
        public abstract uint DefaultVboBlocks { get; }

        public abstract BlockRender CreateBlockRender(ChunkRegion chunkRegion);

        public abstract void DrawLines(Camera camera, Block block);

        public abstract float[] GetData(Block block);

        public abstract void SetupVbo(VertexBufferObject vbo);

        public abstract void Draw(int amount);
        
        public abstract void DrawAfterPostRender(int amount);
    }
}