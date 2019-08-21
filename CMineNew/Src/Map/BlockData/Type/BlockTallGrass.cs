using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;

namespace CMineNew.Map.BlockData.Type{
    public class BlockTallGrass : TexturedCrossBlock{
        public BlockTallGrass(Chunk chunk, Vector3i position)
            : base("default:tall_grass", chunk, position, "default:tall_grass", true) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockTallGrass(chunk, position);
        }
    }
}