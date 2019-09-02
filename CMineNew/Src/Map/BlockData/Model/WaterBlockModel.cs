using CMineNew.Collision;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Render;
using CMineNew.Render;
using CMineNew.Render.Object;
using CMineNew.Util;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Map.BlockData.Model{
    public class WaterBlockModel : BlockModel{
        public const string Key = "default:water";

        private static readonly Vector3[] Vertices = {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 1),
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 1),
            new Vector3(1, 1, 0),
            new Vector3(1, 1, 1),
        };

        private readonly LineVertexArrayObject _lineVao;

        public WaterBlockModel() : base(Key, new Aabb(0, 0, 0, 1, 1, 1)) {
            _lineVao = new LineVertexArrayObject(Vertices, ModelLinesUtil.CalculateLinesIndices(Vertices));
        }

        public override BlockRender CreateBlockRender(ChunkRegion chunkRegion) {
            return new WaterBlockRender(chunkRegion);
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