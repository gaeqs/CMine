using System.Collections.Generic;
using CMineNew.Color;
using CMineNew.Map.BlockData.Snapshot;
using CMineNew.Map.BlockData.Type;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData{
    public static class BlockManager{
        private static readonly Dictionary<string, BlockSnapshot> Blocks = new Dictionary<string, BlockSnapshot>();

        public static void Load() {
            Register(BlockSnapshotAir.Instance);
            Register(BlockSnapshotStone.Instance);
            Register(BlockSnapshotDirt.Instance);
            Register(new BlockSnapshotGrass(new Rgba32I(0, 255, 0, 255)));
            Register(new BlockSnapshotTallGrass(new Rgba32I(0, 255, 0, 255)));
            Register(new BlockSnapshotWater(BlockWater.MaxWaterLevel));
            Register(new BlockSnapshotBricks());
            Register(new BlockSnapshotBricksSlab(false));
            Register(new BlockSnapshotOakLog());
            Register(new BlockSnapshotOakLeaves(new Rgba32I(0, 255, 0, 255)));
            Register(BlockSnapshotSand.Instance);
            Register(new BlockSnapshotTorch());
        }

        public static void Register(BlockSnapshot snapshot) {
            Blocks.Add(snapshot.Id, snapshot);
        }

        public static bool TryGet(string id, out BlockSnapshot snapshot) {
            return Blocks.TryGetValue(id, out snapshot);
        }
    }
}