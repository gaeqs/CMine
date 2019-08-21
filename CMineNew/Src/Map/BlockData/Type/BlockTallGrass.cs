using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;
using CMineNew.Map.BlockData.Snapshot;

namespace CMineNew.Map.BlockData.Type{
    public class BlockTallGrass : TexturedCrossBlock{
        public BlockTallGrass(Chunk chunk, Vector3i position)
            : base("default:tall_grass", chunk, position, "default:tall_grass", true) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockTallGrass(chunk, position);
        }

        public override void OnNeighbourBlockChange(Block from, Block to, BlockFace relative) {
            if (relative == BlockFace.Down && !(to is BlockGrass)) {
                World.SetBlock(new BlockSnapshotAir(), _position);
            }
        }
    }
}