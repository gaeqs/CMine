using System.Linq;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Snapshot;
using CMineNew.Map.BlockData.Type;

namespace CMineNew.Map.Task.Type{
    public class WorldTaskRemoveWater : WorldTask{
        private readonly World _world;
        private readonly Vector3i _position;

        public WorldTaskRemoveWater(World world, Vector3i position) : base(BlockWater.RemoveTicks) {
            _world = world;
            _position = position;
        }

        public override void Run() {
            var block = _world.GetBlock(_position);
            if (!(block is BlockWater)) return;
            _world.SetBlock(new BlockSnapshotAir(), _position);
        }
    }
}