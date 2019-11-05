using System;
using CMine.Map.Generator.Noise;
using CMineNew.Color;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Snapshot;
using CMineNew.Map.Generator.Population;
using OpenTK.Graphics;

namespace CMineNew.Map.Generator.Biomes.Type{
    public class BiomePlains : Biome{
        private readonly OctaveGenerator _heightGenerator;

        private readonly OakTreeGenerator _treeGenerator;
        private readonly OctaveGenerator _treeOctaveGenerator;

        public BiomePlains(World world, int seed)
            : base("default:plains", BiomeTemperature.Normal, 62, 66,
                new Rgba32I(53, 233, 83, 255), world, seed) {
            _heightGenerator = new SimplexOctaveGenerator(seed, 4);
            _heightGenerator.SetScale(1 / 100f);

            _treeGenerator = new OakTreeGenerator(seed);
            _treeOctaveGenerator = new SimplexOctaveGenerator(seed, 1);
            _treeOctaveGenerator.SetScale(100);
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
                    var generateTreeGrass = _treeOctaveGenerator.Noise(100, 100, true,
                                           position.X, y, position.Z);
                    if (generateTreeGrass > 0.99) {
                        _treeGenerator.TryToGenerate(position + new Vector3i(0, 1, 0), _world);
                    }

                    else if (generateTreeGrass > 0.5) {
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