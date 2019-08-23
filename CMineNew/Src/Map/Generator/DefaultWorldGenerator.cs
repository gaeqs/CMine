using CMine.Map.Generator.Noise;
using CMineNew.Map.BlockData.Snapshot;
using CMineNew.Map.BlockData.Type;

namespace CMineNew.Map.Generator{
    public class DefaultWorldGenerator : WorldGenerator{
        private static readonly BlockSnapshot Air = new BlockSnapshotAir();
        private static readonly BlockSnapshot Grass = new BlockSnapshotGrass();
        private static readonly BlockSnapshot TallGrass = new BlockSnapshotTallGrass();
        private static readonly BlockSnapshot Dirt = new BlockSnapshotDirt();
        private static readonly BlockSnapshot Stone = new BlockSnapshotStone();
        private static readonly BlockSnapshot Water = new BlockSnapshotWater(BlockWater.MaxWaterLevel);

        private static readonly BlockSnapshot[,,] Buffer =
            new BlockSnapshot[Chunk.ChunkLength, Chunk.ChunkLength, Chunk.ChunkLength];


        public DefaultWorldGenerator(World world, int seed) : base(world, seed) {
        }

        public override bool GenerateChunkData(Chunk chunk) {
            var caveGenerator = new SimplexOctaveGenerator(_seed, 4);
            var tallGrassGenerator = new SimplexOctaveGenerator(_seed, 1);
            var generator = new SimplexOctaveGenerator(_seed, 8);
            generator.SetScale(0.05);
            tallGrassGenerator.SetScale(1);
            caveGenerator.SetScale(0.05f);

            var empty = true;
            var chunkWorldPosition = chunk.Position << 4;
            for (var x = 0; x < 16; x++) {
                for (var z = 0; z < 16; z++) {
                    for (var y = 0; y < 16; y++) {
                        var wy = y + chunkWorldPosition.Y;
                        var noiseY = (int) (generator.Noise(1, 0.5, true,
                                                x + chunkWorldPosition.X, z + chunkWorldPosition.Z) * 20 + 50);

                        var grass = tallGrassGenerator.Noise(0.2f, 1, true,
                                        x + chunkWorldPosition.X, z + chunkWorldPosition.Z) > 0.3f;

                        if (wy > noiseY + 1) {
                            Buffer[x, y, z] = wy > 45 ? Air : Water;
                        }
                        else if (wy == noiseY + 1) {
                            Buffer[x, y, z] = wy > 45 ? grass ? TallGrass : Air : Water;
                        }
                        else if (wy == noiseY) {
                            empty = false;
                            Buffer[x, y, z] = wy > 44 ? Grass : Dirt;
                        }
                        else if (noiseY - wy < 4) {
                            empty = false;
                            Buffer[x, y, z] = Dirt;
                        }
                        else {
                            empty = false;
                            Buffer[x, y, z] = caveGenerator.Noise(1, 1, true,
                                                  x + chunkWorldPosition.X, y + chunkWorldPosition.Y,
                                                  z + chunkWorldPosition.Z) > 0.2f
                                ? Air
                                : Stone;
                        }
                    }
                }
            }

            chunk.FillWithBlocks(Buffer, empty);
            return empty;
        }
    }
}