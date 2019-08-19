using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;

namespace CMineNew.Map.BlockData.Type{
    public class BlockStone : TexturedCubicBlock{
        public BlockStone(Chunk chunk, Vector3i position)
            : base("default:stone", chunk, position, CMine.Textures.Areas["default:stone"]) {
        }
    }
}