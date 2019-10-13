using CMineNew.Map.BlockData.Render;

namespace CMineNew.Map.BlockData.Model{
    public class TallGrassBlockModel : CrossBlockModel{
        public new const string Key = "default:tall_grass";

        public override BlockRender CreateBlockRender(ChunkRegion chunkRegion) {
            return new TallGrassBlockRender(chunkRegion);
        }
    }
}