using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using CMineNew.Map.BlockData.Static.Type;
using CMineNew.Map.BlockData.Type;

namespace CMineNew.Map.BlockData.Snapshot{
    public class BlockSnapshotSand : BlockSnapshot{
        public static readonly BlockSnapshotSand Instance = new BlockSnapshotSand();
        public BlockSnapshotSand() : base("default:sand", BlockStaticDataSand.Instance) {
        }

        public override BlockModel BlockModel => BlockModelManager.GetModelOrNull(CubicBlockModel.Key);
        public override bool Passable => false;

        public override Block ToBlock(Chunk chunk, Vector3i position) {
            return new BlockSand(chunk, position);
        }

        public override bool CanBePlaced(Vector3i position, World world) {
            return true;
        }

        public override BlockSnapshot Clone() {
            return this;
        }
    }
}