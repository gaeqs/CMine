using System;
using CMineNew.Collision;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Render;
using CMineNew.Render;
using CMineNew.Render.Object;
using CMineNew.Util;
using OpenTK;
using OpenTK.Graphics.ES11;

namespace CMineNew.Map.BlockData.Model{
    public class TorchBlockModel : BlockModel{
        public const string Key = "default:torch";

        public static readonly Vector3[] Vertices = {
            new Vector3(0.4375f, 0, 0.4375f), 
            new Vector3(0.4375f, 0, 0.5625f), 
            new Vector3(0.4375f, 0.625f, 0.4375f), 
            new Vector3(0.4375f, 0.625f, 0.5625f), 
            new Vector3(0.5625f, 0, 0.4375f), 
            new Vector3(0.5625f, 0, 0.5625f), 
            new Vector3(0.5625f, 0.625f, 0.4375f), 
            new Vector3(0.5625f, 0.625f, 0.5625f), 
        };

        private readonly LineVertexArrayObject _lineVao;

        public TorchBlockModel() : base(Key, new Aabb(0.4f, 0, 0.4f, 0.2f, 0.6f, 0.2f)) {
            _lineVao = new LineVertexArrayObject(Vertices, ModelLinesUtil.CalculateLinesIndices(Vertices));
        }

        public override BlockRender CreateBlockRender(ChunkRegion chunkRegion) {
            return new TorchBlockRender(chunkRegion);
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