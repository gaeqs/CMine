using System;
using CMine.Map.Generator.Noise;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Snapshot;

namespace CMineNew.Map.Generator.Biomes.Type{
    public class BiomeDesert : Biome{
        
        private static BlockSnapshot Water = new BlockSnapshotWater(8);
        
        private readonly OctaveGenerator _heightGenerator;

        public BiomeDesert(World world, int seed)
            : base(BiomeTemperature.Hot, 62, 70, world, seed) {
            _heightGenerator = new SimplexOctaveGenerator(seed, 4);
            _heightGenerator.SetScale(1 / 30f);
        }

        public override int GetColumnHeight(int x, int z) {
            var normalized = (float) _heightGenerator.Noise(1, 1, true, x, z) + 1;
            normalized /= 2;
            return (int) Math.Floor(normalized * (_maxHeight - _minHeight) + _minHeight);
        }

        public override BlockSnapshot GetBlockSnapshot(Vector3i position, int columnHeight) {
            var y = position.Y;
            if (y > columnHeight) {
                return  y > 60 ? BlockSnapshotAir.Instance : Water;
            }

            if (y > columnHeight - 4) {
                return BlockSnapshotSand.Instance;
            }

            return BlockSnapshotStone.Instance;
        }
    }
}