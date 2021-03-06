using System;
using CMine.Map.Generator.Noise;
using CMineNew.Color;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Snapshot;
using CMineNew.Map.Generator.Population;
using OpenTK.Graphics;

namespace CMineNew.Map.Generator.Biomes.Type{
    public class BiomeMountains : Biome{
        private readonly OctaveGenerator _heightGenerator;
        private readonly OakTreeGenerator _treeGenerator;
        private readonly OctaveGenerator _caveGenerator;
        private readonly Random _random;
       

        public BiomeMountains(World world, int seed)
            : base("default:mountains", BiomeTemperature.Normal, 70, 90,
                new Rgba32I(27, 162, 113, 255), world, seed) {
            _heightGenerator = new SimplexOctaveGenerator(seed, 1);
            _heightGenerator.SetScale(1 / 50f);
            _treeGenerator = new OakTreeGenerator(seed);
            _caveGenerator = new SimplexOctaveGenerator(seed, 7);
            _caveGenerator.SetScale(1 / 20f);
            _random = new Random();
        }

        public override int GetColumnHeight(int x, int z) {
            var normalized = (float) _heightGenerator.Noise(1, 1, true, x, z) + 1;
            normalized /= 2;
            return (int) Math.Floor(normalized * (_maxHeight - _minHeight) + _minHeight);
        }

        public override BlockSnapshot GetBlockSnapshot(Vector3i position, int columnHeight, Rgba32I grassColor) {
            var y = position.Y;
            var cave = _caveGenerator.Noise(1, 10, true, position.X, position.Y, position.Z) > 0.5;
            if (cave) {
                return  BlockSnapshotAir.Instance;
            }
            if (y > columnHeight) {
                return y > 60 ? BlockSnapshotAir.Instance : (BlockSnapshot) new BlockSnapshotWater(8);
            }

            if (y == columnHeight) {
                if (y > 59) {
                    if (_random.NextDouble() > 0.9995) {
                        _treeGenerator.TryToGenerate(position + new Vector3i(0, 1, 0), _world);
                    }

                    else if (_random.NextDouble() > 0.96) {
                        _world.UnloadedChunkGenerationManager.AddBlock(position + new Vector3i(0, 1, 0),
                            new BlockSnapshotTallGrass(grassColor), false);
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
    }
}