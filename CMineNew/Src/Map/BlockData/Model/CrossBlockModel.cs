using CMineNew.Collision;
using CMineNew.Map.BlockData.Render;
using CMineNew.Render;
using CMineNew.Render.Object;
using CMineNew.Util;
using OpenTK;
using OpenTK.Graphics.ES11;

namespace CMineNew.Map.BlockData.Model{
    public class CrossBlockModel : BlockModel{
        public const string Key = "default:cross";
        

        public CrossBlockModel() : base(Key, new Aabb(0.1f, 0, 0.1f, 0.8f, 0.7f, 0.8f)) {
        }

        public override BlockRender CreateBlockRender(ChunkRegion chunkRegion) {
            return new CrossBlockRender(chunkRegion);
        }
    }
}