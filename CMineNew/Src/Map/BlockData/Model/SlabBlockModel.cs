using CMineNew.Collision;
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

        private static readonly Vector3[] Vertices = {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(0, 0.5f, 0),
            new Vector3(0, 0.5f, 1),
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 1),
            new Vector3(1, 0.5f, 0),
            new Vector3(1, 0.5f, 1),
        };

        private readonly LineVertexArrayObject _lineVao;

        public SlabBlockModel() : base(Key, new Aabb(0, 0, 0, 1, 0.5f, 1)) {
            _lineVao = new LineVertexArrayObject(Vertices, ModelLinesUtil.CalculateLinesIndices(Vertices));
        }

        public override uint FloatsPerBlock => 66;
        public override uint DefaultVboBlocks => 500;

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

        public override float[] GetData(Block block) {
            if (!(block is SlabBlock slab)) return new float[0];
            var data = new float[FloatsPerBlock];

            var pointer = 0;

            for (var i = 0; i < BlockFaceMethods.All.Length; i++) {
                if (!slab.IsFaceVisible(i)) {
                    for (var j = 0; j < FloatsPerBlock / 6; j++)
                        data[pointer++] = -1;
                    continue;
                }

                var pos = block.Position;
                var area = slab.GetTextureArea((BlockFace) i);
                var filter = block.TextureFilter;

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
                data[pointer++] = slab.Upside ? 1 : 0;
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