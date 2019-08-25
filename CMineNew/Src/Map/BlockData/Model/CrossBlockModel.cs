using System;
using CMine.Collision;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Render;
using CMineNew.Render;
using CMineNew.Render.Object;
using CMineNew.Util;
using OpenTK;
using OpenTK.Graphics.ES11;

namespace CMineNew.Map.BlockData.Model{
    public class CrossBlockModel : BlockModel{
        public const string Key = "default:cross";

        public static readonly Vector3[] Vertices = {
            new Vector3(0.1f, 0, 0.1f),
            new Vector3(0.1f, 0, 0.9f),
            new Vector3(0.1f, 0.7f, 0.1f),
            new Vector3(0.1f, 0.7f, 0.9f),
            new Vector3(0.9f, 0, 0.1f),
            new Vector3(0.9f, 0, 0.9f),
            new Vector3(0.9f, 0.7f, 0.1f),
            new Vector3(0.9f, 0.7f, 0.9f),
        };

        private readonly LineVertexArrayObject _lineVao;

        public CrossBlockModel() : base(Key, new Aabb(0.1f, 0, 0.1f, 0.8f, 0.7f, 0.8f)) {
            _lineVao = new LineVertexArrayObject(Vertices, ModelLinesUtil.CalculateLinesIndices(Vertices));
        }

        public override BlockRender CreateBlockRender(ChunkRegion chunkRegion) {
            return new CrossBlockRender(chunkRegion);
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