using CMine.Collision;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Render;
using CMineNew.Render;
using CMineNew.Render.Object;
using CMineNew.Util;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Map.BlockData.Model{
    public class CubicBlockModel : BlockModel{
        public const string Key = "default:cubic";

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

        public CubicBlockModel() : base(Key, new Aabb(0, 0, 0, 1, 1, 1)) {
            _lineVao = new LineVertexArrayObject(Vertices, ModelLinesUtil.CalculateLinesIndices(Vertices));
        }

        public override BlockRender CreateBlockRender(ChunkRegion chunkRegion) {
            return new CubicBlockRender(chunkRegion);
        }

        public override void DrawLines(Camera camera, Vector3i blockPosition) {
            BlockLinesShaderProgram.Use();
            BlockLinesShaderProgram.SetUMatrix("viewProjection", camera.ViewProjection);
            BlockLinesShaderProgram.SetUVector("worldPosition", blockPosition);
            GL.LineWidth(2);
            _lineVao.Bind();
            _lineVao.Draw();
        }
    }
}