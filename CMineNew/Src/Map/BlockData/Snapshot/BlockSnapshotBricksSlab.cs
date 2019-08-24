using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using CMineNew.Map.BlockData.Type;

namespace CMineNew.Map.BlockData.Snapshot{
    public class BlockSnapshotBricksSlab : BlockSnapshot{
        public BlockSnapshotBricksSlab() : base("default:bricks_slab") {
        }

        public override BlockModel BlockModel => BlockModelManager.GetModelOrNull(SlabBlockModel.Key);
        public override bool Passable => false;
        
        public override Block ToBlock(Chunk chunk, Vector3i position) {
            return new BlockBricksSlab(chunk, position);
        }
        
        public override bool CanBePlaced(Vector3i position, World world) {
            return true;
        }
    }
}