using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Type{
    public class BlockStone : TexturedCubicBlock{
        public BlockStone(Chunk chunk, Vector3i position)
            : base("default:stone", chunk, position, "default:stone", Color4.Transparent,
                false, false, 0, MaxBlockLight, MaxBlockLight) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockStone(chunk, position);
        }
    }
}