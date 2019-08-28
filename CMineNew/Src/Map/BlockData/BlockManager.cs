using System.Collections.Generic;
using CMineNew.Map.BlockData.Snapshot;
using CMineNew.Map.BlockData.Type;

namespace CMineNew.Map.BlockData{
    public class BlockManager{
        private static Dictionary<string, BlockSnapshot> Blocks = new Dictionary<string, BlockSnapshot>();

        public static void Load() {
            Register(BlockSnapshotAir.Instance);
            Register(BlockSnapshotStone.Instance);
            Register(BlockSnapshotDirt.Instance);
            Register(BlockSnapshotGrass.Instance);
            Register(new BlockSnapshotTallGrass());
            Register(new BlockSnapshotWater(BlockWater.MaxWaterLevel));
            Register(new BlockSnapshotBricks());
            Register(new BlockSnapshotBricksSlab(false));
            Register(new BlockSnapshotOakLog());
            Register(new BlockSnapshotOakLeaves());
            Register(BlockSnapshotSand.Instance);
        }

        public static void Register(BlockSnapshot snapshot) {
            Blocks.Add(snapshot.Id, snapshot);
        }

        public static bool TryGet(string id, out BlockSnapshot snapshot) {
            return Blocks.TryGetValue(id, out snapshot);
        }
    }
}