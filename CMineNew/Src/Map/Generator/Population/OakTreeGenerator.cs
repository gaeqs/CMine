using CMineNew.Geometry;
using CMineNew.Map.BlockData.Snapshot;
using CMineNew.Map.BlockData.Type;

namespace CMineNew.Map.Generator.Population{
    public class OakTreeGenerator{
        private static readonly BlockSnapshot Log = new BlockSnapshotOakLog();

        private int _height;

        public OakTreeGenerator(int height) {
            _height = height;
        }

        public bool CanSpawn(Vector3i position, World world) {
            for (var x = -1; x < 2; x++) {
                for (var y = 0; y < _height + 1; y++) {
                    for (var z = -1; z < 2; z++) {
                        var block = world.GetBlock(position + new Vector3i(x, y, z));
                        if (!(block is BlockAir || block is BlockTallGrass)) return false;
                    }
                }
            }

            return true;
        }

        public bool TryToGenerate(Vector3i position, World world) {
            if (!CanSpawn(position, world)) return false;
            for (var y = 0; y < _height; y++) {
                world.SetBlock(Log, position + new Vector3i(0, y, 0));
            }


            return true;
        }
    }
}