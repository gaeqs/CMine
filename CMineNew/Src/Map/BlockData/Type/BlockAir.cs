using CMineNew.Geometry;
using OpenTK;

namespace CMineNew.Map.BlockData.Type{
    public class BlockAir : Block{
        public BlockAir(Chunk chunk, Vector3i position)
            : base("default:air", null, chunk, position, true) {
        }

        public override void OnPlace(Block oldBlock, Block[] neighbours) {
        }

        public override void OnRemove(Block newBlock) {
        }

        public override void OnNeighbourBlockChange(Block from, Block to, BlockFace relative) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockAir(chunk, position);
        }

        public override bool Collides(Vector3 origin, Vector3 direction) {
            return false;
        }

        public override bool IsFaceOpaque(BlockFace face) {
            return false;
        }
    }
}