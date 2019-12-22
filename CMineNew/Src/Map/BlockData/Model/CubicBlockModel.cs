using CMineNew.Collision;
using CMineNew.Map.BlockData.Render;
using CMineNew.Render;
using CMineNew.Render.Object;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Map.BlockData.Model{
    public class CubicBlockModel : BlockModel{
        public const string Key = "default:cubic";

        private readonly LineVertexArrayObject _lineVao;

        public CubicBlockModel() : base(Key, new Aabb(0, 0, 0, 1, 1, 1)) {
            _lineVao = new LineVertexArrayObject(new[] {BlockCollision});
        }

        public override BlockRender CreateBlockRender(ChunkRegion chunkRegion) {
            return new CubicBlockRender(chunkRegion);
        }

        public override void DrawLines(Camera camera, Block block) {
            _lineVao.Bind();
            BlockLinesShaderProgram.Use();
            BlockLinesShaderProgram.SetUMatrix("viewProjection", camera.ViewProjection);
            BlockLinesShaderProgram.SetUVector("worldPosition", block.Position);
            GL.LineWidth(2);
            _lineVao.Draw();
        }
    }
}