using CMineNew.Collision;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Render;
using CMineNew.Map.BlockData.Sketch;
using CMineNew.Render;
using CMineNew.Render.Object;
using CMineNew.Util;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Map.BlockData.Model{
    public class SlabBlockModel : BlockModel{
        public const string Key = "default:slab";

        private readonly LineVertexArrayObject _lineVao;

        public SlabBlockModel() : base(Key, new Aabb(0, 0, 0, 1, 0.5f, 1)) {
        }

        public override BlockRender CreateBlockRender(ChunkRegion chunkRegion) {
            return new SlabBlockRender(chunkRegion);
        }

        public override void DrawLines(Camera camera, Block block) {
            if (!(block is SlabBlock slab)) return;
            _lineVao.Bind();
            BlockLinesShaderProgram.Use();
            BlockLinesShaderProgram.SetUMatrix("viewProjection", camera.ViewProjection);

            var position = block.Position.ToFloat() + new Vector3(0, slab.Upside ? SlabBlock.SlabHeight : 0, 0);
            BlockLinesShaderProgram.SetUVector("worldPosition", position);
            GL.LineWidth(2);
            _lineVao.Draw();
        }
    }
}