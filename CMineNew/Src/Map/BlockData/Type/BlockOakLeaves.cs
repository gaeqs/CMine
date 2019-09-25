using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;
using CMineNew.Map.BlockData.Static.Type;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Type{
    public class BlockOakLeaves : TexturedCubicBlock{
        public BlockOakLeaves(Chunk chunk, Vector3i position)
            : base(BlockStaticDataOakLeaves.Instance, chunk, position, Color4.Green) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockOakLog(chunk, position);
        }

        public override bool IsFaceOpaque(BlockFace face) {
            return false;
        }
    }
}