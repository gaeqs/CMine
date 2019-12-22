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

        public WaterBlockModel() : base(Key, new Aabb(0, 0, 0, 1, 1, 1)) {
        }

        public override BlockRender CreateBlockRender(ChunkRegion chunkRegion) {
            return new WaterBlockRender(chunkRegion);
        }
    }
}