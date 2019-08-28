using System;
using CMine.Map.Generator.Noise;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Snapshot;

namespace CMineNew.Map.Generator.Biomes.Type{
    public class BiomePlains : Biome{
        private readonly OctaveGenerator _heightGenerator;

        public BiomePlains(int seed)
            : base(BiomeTemperature.Normal, 62, 66, seed) {
            _heightGenerator = new SimplexOctaveGenerator(seed, 4);
            _heightGenerator.SetScale(1 / 100f);
        }

        public override int GetColumnHeight(int x, int z) {
            var normalized = (float) _heightGenerator.Noise(1, 1, true, x, z) + 1;
            normalized /= 2;
            return (int) Math.Floor(normalized * (_maxHeight - _minHeight) + _minHeight);
        }

        public override BlockSnapshot GetBlockSnapshot(Vector3i position, int columnHeight) {
            var y = position.Y;
            if (y > columnHeight) {
                return BlockSnapshotAir.Instance;
            }

            if (y == columnHeight) {
                return BlockSnapshotGrass.Instance;
            }

            if (y > columnHeight - 4) {
                return BlockSnapshotDirt.Instance;
            }

            return BlockSnapshotStone.Instance;
        }
    }
}