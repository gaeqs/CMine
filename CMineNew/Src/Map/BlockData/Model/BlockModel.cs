using CMineNew.Collision;
using CMineNew.Map.BlockData.Render;
using CMineNew.Render;
using CMineNew.Render.Object;
using CMineNew.Resources.Shaders;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Map.BlockData.Model{
    public abstract class BlockModel{
        public static readonly ShaderProgram BlockLinesShaderProgram =
            new ShaderProgram(Shaders.block_lines_vertex, Shaders.block_lines_fragment);

        protected string _id;
        protected readonly Aabb _blockCollision;
        protected readonly LineVertexArrayObject _lineVao;

        public BlockModel(string id, Aabb blockCollision) {
            _id = id;
            _blockCollision = blockCollision;
            _lineVao = new LineVertexArrayObject(new[] {blockCollision});
        }

        public Aabb BlockCollision => _blockCollision;

        public string Id => _id;

        public LineVertexArrayObject LineVao => _lineVao;

        public abstract BlockRender CreateBlockRender(ChunkRegion chunkRegion);

        public virtual void DrawLines(Camera camera, Block block) {
            _lineVao.Bind();
            BlockLinesShaderProgram.Use();
            BlockLinesShaderProgram.SetUMatrix("viewProjection", camera.ViewProjection);
            BlockLinesShaderProgram.SetUVector("worldPosition", block.Position);
            GL.LineWidth(2);
            _lineVao.Draw();
        }
    }
}