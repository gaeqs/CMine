using System;
using CMineNew.Collision;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Render;
using CMineNew.Map.BlockData.Type;
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
            new Vector3(0.4375f, 0.5625f, 0.4375f), 
            new Vector3(0.4375f, 0.5625f, 0.5625f), 
            new Vector3(0.5625f, 0, 0.4375f), 
            new Vector3(0.5625f, 0, 0.5625f), 
            new Vector3(0.5625f, 0.5625f, 0.4375f), 
            new Vector3(0.5625f, 0.5625f, 0.5625f), 
        };

        private readonly LineVertexArrayObject _lineVao;

        public TorchBlockModel() : base(Key, new Aabb(0.4f, 0, 0.4f, 0.2f, 0.6f, 0.2f)) {
            _lineVao = new LineVertexArrayObject(Vertices, ModelLinesUtil.CalculateLinesIndices(Vertices));
        }

        public override uint FloatsPerBlock => 10;
        public override uint DefaultVboBlocks => 200;

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

        public override float[] GetData(Block block) {
            if (!(block is BlockTorch)) return new float[0];
            var pos = block.Position;
            var area = block.StaticData.GetTextureArea(BlockFace.Up);
            var filter = block.TextureFilter;
            var blockLight = block.BlockLight.Light;
            var sunlight = block.BlockLight.Sunlight;
            return new[] {
                pos.X, pos.Y, pos.Z, area.MinX, area.MinY, area.MaxX, area.MaxY,
                filter.ValueF, blockLight / Block.MaxBlockLightF, sunlight / Block.MaxBlockLightF
            };
        }
        
        public override void SetupVbo(VertexBufferObject vbo) {
         
        }

        public override void Draw(int amount) {
  
        }

        public override void DrawAfterPostRender(int amount) {
        }
    }
}