using CMine.Map.Generator.Noise;
using CMineNew.Map.BlockData.Snapshot;

namespace CMineNew.Map.Generator{
    public class DefaultWorldGenerator : WorldGenerator{
        private static readonly BlockSnapshot Air = new BlockSnapshotAir();
        private static readonly BlockSnapshot Grass = new BlockSnapshotGrass();
        private static readonly BlockSnapshot TallGrass = new BlockSnapshotTallGrass();
        private static readonly BlockSnapshot Dirt = new BlockSnapshotDirt();
        private static readonly BlockSnapshot Stone = new BlockSnapshotStone();

        private static readonly BlockSnapshot[,,] _buffer =
            new BlockSnapshot[Chunk.ChunkLength, Chunk.ChunkLength, Chunk.ChunkLength];


        public DefaultWorldGenerator(World world, int seed) : base(world, seed) {
        }

        public override bool GenerateChunkData(Chunk chunk) {
            var tallGrassGenerator = new SimplexOctaveGenerator(_seed, 1);
            var generator = new SimplexOctaveGenerator(_seed, 8);
            generator.SetScale(0.05);

            tallGrassGenerator.SetScale(1);

            var empty = true;
            var chunkWorldPosition = chunk.Position << 4;
            for (var x = 0; x < 16; x++) {
                for (var z = 0; z < 16; z++) {
                    for (var y = 0; y < 16; y++) {
                        var wy = y + chunkWorldPosition.Y;
                        var noiseY = (int) (generator.Noise(1, 0.5, true,
                                                x + chunkWorldPosition.X, z + chunkWorldPosition.Z) * 20 + 50);

                        var grass = tallGrassGenerator.Noise(0.2f, 1, true,
                                        x + chunkWorldPosition.X, z + chunkWorldPosition.Z) > 0.4f;

                        if (wy > noiseY + 1) {
                            _buffer[x, y, z] = Air;
                        }
                        else if (wy == noiseY + 1) {
                            _buffer[x, y, z] = grass ? TallGrass : Air;
                        }
                        else if (wy == noiseY) {
                            empty = false;
                            _buffer[x, y, z] = Grass;
                        }
                        else if (noiseY - wy < 4) {
                            empty = false;
                            _buffer[x, y, z] = Dirt;
                        }
                        else {
                            empty = false;
                            _buffer[x, y, z] = Stone;
                        }
                    }
                }
            }

            chunk.FillWithBlocks(_buffer, empty);
            return empty;
        }
    }
}