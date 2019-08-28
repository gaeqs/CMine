using CMineNew.Geometry;
using CMineNew.Map.BlockData.Snapshot;
using CMineNew.Map.Generator.Biomes;

namespace CMineNew.Map.Generator{
    public class DefaultWorldGenerator : WorldGenerator{
        private readonly BlockSnapshot[,,] _buffer =
            new BlockSnapshot[Chunk.ChunkLength, Chunk.ChunkLength, Chunk.ChunkLength];

        private BiomeGrid _biomeGrid;


        public DefaultWorldGenerator(World world, int seed) : base(world, seed) {
            _biomeGrid = new DefaultBiomeGrid(seed);
        }

        public override bool GenerateChunkData(Chunk chunk) {
            var empty = true;
            var chunkWorldPosition = chunk.Position << 4;
            for (var x = 0; x < 16; x++) {
                for (var z = 0; z < 16; z++) {
                    var wX = x + chunkWorldPosition.X;
                    var wZ = z + chunkWorldPosition.Z;
                    var biome = _biomeGrid.GetBiome(wX, wZ);
                    var height = InterpolateHeight(wX, wZ, biome);
                    for (var y = 0; y < 16; y++) {
                        var position = new Vector3i(x, y, z) + chunkWorldPosition;
                        var snapshot = biome.GetBlockSnapshot(position, height);
                        _buffer[x, y, z] = snapshot;
                        if (!(snapshot is BlockSnapshotAir)) {
                            empty = false;
                        }
                    }
                }
            }

            chunk.FillWithBlocks(_buffer, empty);
            return empty;
        }

        private int InterpolateHeight(int worldX, int worldZ, Biome mainBiome) {
            var height = mainBiome.GetColumnHeight(worldX, worldZ);
            var total = height;
            for (var x = -2; x < 3; x++) {
                for (var z = -2; z < 3; z++) {
                    if (x == 0 && z == 0) continue;
                    var biome = _biomeGrid.GetBiome(worldX + x, worldZ + z);
                    if (biome == mainBiome) {
                        total += height;
                    }
                    else {
                        total += biome.GetColumnHeight(worldX, worldZ);
                    }
                }
            }

            return total / 25;
        }
    }
}