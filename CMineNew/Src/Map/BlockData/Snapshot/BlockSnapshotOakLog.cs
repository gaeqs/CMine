using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using CMineNew.Map.BlockData.Type;

namespace CMineNew.Map.BlockData.Snapshot{
    public class BlockSnapshotOakLog : BlockSnapshot{
        public BlockSnapshotOakLog() : base("default:oak_log") {
        }

        public override BlockModel BlockModel => BlockModelManager.GetModelOrNull(CubicBlockModel.Key);
        public override bool Passable => false;

        public override Block ToBlock(Chunk chunk, Vector3i position) {
            return new BlockOakLog(chunk, position);
        }

        public override bool CanBePlaced(Vector3i position, World world) {
            return true;
        }
        
        public override BlockSnapshot Clone() {
            return this;
        }
    }
}