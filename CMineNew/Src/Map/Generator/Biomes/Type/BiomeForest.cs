using System;
using CMine.Map.Generator.Noise;
using CMineNew.Color;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Snapshot;
using CMineNew.Map.Generator.Population;
using OpenTK.Graphics;

namespace CMineNew.Map.Generator.Biomes.Type{
    public class BiomeForest : Biome{
        private readonly OctaveGenerator _heightGenerator;
        private readonly OakTreeGenerator _treeGenerator;
        private readonly OctaveGenerator _treeOctaveGenerator;

        public BiomeForest(World world, int seed)
            : base("default:forest", BiomeTemperature.Normal, 62, 70, new Rgba32I(0, 255, 0, 255), world, seed) {
            _heightGenerator = new SimplexOctaveGenerator(seed, 6);
            _heightGenerator.SetScale(1 / 20f);
            _treeGenerator = new OakTreeGenerator(seed);
            _treeOctaveGenerator = new SimplexOctaveGenerator(seed, 1);
        }

        public override int GetColumnHeight(int x, int z) {
            var normalized = (float) _heightGenerator.Noise(1, 1, true, x, z) + 1;
            normalized /= 2;
            return (int) Math.Floor(normalized * (_maxHeight - _minHeight) + _minHeight);
        }

        public override BlockSnapshot GetBlockSnapshot(Vector3i position, int columnHeight, Rgba32I grassColor) {
            var y = position.Y;
            if (y > columnHeight) {
                return y > 60 ? BlockSnapshotAir.Instance : (BlockSnapshot) new BlockSnapshotWater(8);
            }

            if (y == columnHeight) {
                if (y > 59) {
                    var generateTree = _treeOctaveGenerator.Noise(100, 10, true,
                                           position.X, y, position.Z) > 0.7;
                    if (generateTree) {
                        _treeGenerator.TryToGenerate(position + new Vector3i(0, 1, 0), _world);
                    }

                    return new BlockSnapshotGrass(grassColor);
                }

                return BlockSnapshotDirt.Instance;
            }

            if (y > columnHeight - 4) {
                return BlockSnapshotDirt.Instance;
            }

            return BlockSnapshotStone.Instance;
        }

        private void TryToGenerateTree(Vector3i position) {
        }
    }
}