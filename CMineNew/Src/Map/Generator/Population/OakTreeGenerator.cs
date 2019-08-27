using System;
using CMineNew.Geometry;
using CMineNew.Map.BlockData;
using CMineNew.Map.BlockData.Snapshot;
using CMineNew.Map.BlockData.Type;

namespace CMineNew.Map.Generator.Population{
    public class OakTreeGenerator{
        private static readonly BlockSnapshot Log = new BlockSnapshotOakLog();
        private static readonly BlockSnapshot Leaves = new BlockSnapshotOakLeaves();

        private int _seed;

        public OakTreeGenerator(int seed) {
            _seed = seed;
        }

        public bool CanSpawn(Vector3i position, World world, int height) {
            for (var x = -1; x < 2; x++) {
                for (var y = height - 2; y < height + 1; y++) {
                    for (var z = -1; z < 2; z++) {
                        var block = world.GetBlock(position + new Vector3i(x, y, z));
                        if (!(block is BlockAir || block is BlockTallGrass || block is BlockOakLeaves)) return false;
                    }
                }
            }

            return true;
        }

        public bool TryToGenerate(Vector3i position, World world) {
            var random = new Random(_seed + position.X + position.Y + position.Z);
            var height = random.Next(4, 6);
            if (!CanSpawn(position, world, height)) return false;
            for (var y = 0; y < height; y++) {
                world.SetBlock(Log, position + new Vector3i(0, y, 0));
            }

            for (var x = -2; x < 3; x++) {
                for (var z = -2; z < 3; z++) {
                    if (x == 0 && z == 0) continue;
                    if ((x == -2 || x == 2) && (z == -2 || z == 2)) {
                        if (random.NextDouble() > 0.5) {
                            world.SetBlock(Leaves, position + new Vector3i(x, height - 2, z), CanBeReplaced);
                        }
                    }
                    else {
                        world.SetBlock(Leaves, position + new Vector3i(x, height - 2, z), CanBeReplaced);
                    }
                }
            }

            for (var x = -1; x < 2; x++) {
                for (var z = -1; z < 2; z++) {
                    if (x == 0 && z == 0) continue;
                    if ((x == -1 || x == 1) && (z == -1 || z == 1)) {
                        if (random.NextDouble() > 0.5) {
                            world.SetBlock(Leaves, position + new Vector3i(x, height - 1, z), CanBeReplaced);
                        }
                    }
                    else {
                        world.SetBlock(Leaves, position + new Vector3i(x, height - 1, z), CanBeReplaced);
                    }
                }
            }

            for (var x = -1; x < 2; x++) {
                for (var z = -1; z < 2; z++) {
                    if (!((x == -1 || x == 1) && (z == -1 || z == 1))) {
                        world.SetBlock(Leaves, position + new Vector3i(x, height, z), CanBeReplaced);
                    }
                }
            }

            return true;
        }

        private static bool CanBeReplaced(Block block) {
            return block is BlockAir || block is BlockTallGrass;
        }
    }
}