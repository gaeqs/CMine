using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;
using CMineNew.Map.BlockData.Static.Type;
using CMineNew.Texture;

namespace CMineNew.Map.BlockData.Type{
    public class BlockOakLeaves : TexturedCubicBlock{
        public BlockOakLeaves(Chunk chunk, Vector3i position)
            : base(BlockStaticDataOakLeaves.Instance, chunk, position, new Rgba32I(0, 255, 0, 255)) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockOakLog(chunk, position);
        }

        public override bool IsFaceOpaque(BlockFace face) {
            return false;
        }
    }
}