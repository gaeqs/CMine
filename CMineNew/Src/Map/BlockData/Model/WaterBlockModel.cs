using CMineNew.Collision;
using CMineNew.Map.BlockData.Render;
using CMineNew.Map.BlockData.Type;
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

        public override uint FloatsPerBlock => 14 * 6;
        public override uint DefaultVboBlocks => 10000;

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

        public override float[] GetData(Block block) {
            if (!(block is BlockWater water)) return new float[0];
            var data = new float[FloatsPerBlock];

            var pointer = 0;

            for (var i = 0; i < BlockFaceMethods.All.Length; i++) {
                if (!water.IsFaceVisible(i)) {
                    for (var j = 0; j < FloatsPerBlock / 6; j++)
                        data[pointer++] = -1;
                    continue;
                }
                var pos = block.Position;
                var area = BlockWater.TextureArea;
                var filter = block.TextureFilter;
                var levels = water.VertexWaterLevel;
                var onTop = water.HasWaterOnTop;

                data[pointer++] = pos.X;
                data[pointer++] = pos.Y;
                data[pointer++] = pos.Z;
                data[pointer++] = area.MinX;
                data[pointer++] = area.MinY;
                data[pointer++] = area.MaxX;
                data[pointer++] = area.MaxY;
                data[pointer++] = filter.ValueF;
                data[pointer++] = block.BlockLight.Light / Block.MaxBlockLightF;
                data[pointer++] = block.BlockLight.Sunlight / Block.MaxBlockLightF;
                data[pointer++] = onTop ? 8 : levels[0];
                data[pointer++] = onTop ? 8 : levels[1];
                data[pointer++] = onTop ? 8 : levels[2];
                data[pointer++] = onTop ? 8 : levels[3];
            }

            return data;
        }
        
        public override void SetupVbo(VertexBufferObject vbo) {
        }

        public override void Draw(int amount) {
        }

        public override void DrawAfterPostRender(int amount) {
        }
    }
}