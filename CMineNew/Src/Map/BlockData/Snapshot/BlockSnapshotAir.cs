using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using CMineNew.Map.BlockData.Type;

namespace CMineNew.Map.BlockData.Snapshot{
    public class BlockSnapshotAir : BlockSnapshot{
        public BlockSnapshotAir() : base("default:air") {
        }

        public override BlockModel BlockModel => null;
        public override bool Passable => true;

        public override Block ToBlock(Chunk chunk, Vector3i position) {
            return new BlockAir(chunk, position);
        }

        public override bool CanBePlaced(Vector3i position, World world) {
            return true;
        }

        public override BlockSnapshot Clone() {
            return this;
        }
    }
}