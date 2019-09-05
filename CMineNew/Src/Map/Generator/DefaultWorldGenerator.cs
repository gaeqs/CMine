using CMineNew.Geometry;
using CMineNew.Map.BlockData.Snapshot;
using CMineNew.Map.Generator.Biomes;

namespace CMineNew.Map.Generator{
    public class DefaultWorldGenerator : WorldGenerator{
        private readonly BlockSnapshot[,,] _buffer =
            new BlockSnapshot[Chunk.ChunkLength, Chunk.ChunkLength, Chunk.ChunkLength];

        public DefaultWorldGenerator(World world, int seed) : base(world, new DefaultBiomeGrid(world, seed), seed) {
        }

        public override bool GenerateChunkData(Chunk chunk) {
            var empty = true;
            var chunkWorldPosition = chunk.Position << Chunk.WorldPositionShift;
            var chunk2dPosition = new Vector2i(chunk.Position.X, chunk.Position.Z);
            var region2dPosition = chunk2dPosition >> World2dRegion.ChunkPositionShift;
            var region2d = _world.GetOrCreate2dRegion(region2dPosition);

            var blockRegionPosition = (chunk2dPosition - (region2dPosition << World2dRegion.ChunkPositionShift))
                                      << Chunk.WorldPositionShift;

            for (var x = 0; x < 16; x++) {
                for (var z = 0; z < 16; z++) {
                    var wX = x + blockRegionPosition.X;
                    var wZ = z + blockRegionPosition.Y;

                    var biome = region2d.Biomes[wX, wZ];
                    var height = region2d.InterpolatedHeights[wX, wZ];
                    var grassColor = region2d.InterpolatedGrassColors[wX, wZ];

                    for (var y = 0; y < 16; y++) {
                        var position = new Vector3i(x, y, z) + chunkWorldPosition;
                        var snapshot = biome.GetBlockSnapshot(position, height, grassColor);

                        _buffer[x, y, z] = snapshot;
                        if (!(snapshot is BlockSnapshotAir)) {
                            empty = false;
                        }
                    }
                }
            }

            empty &= _world.UnloadedChunkGenerationManager.PostGenerateChunk(chunk, _buffer, region2d);
            chunk.FillWithBlocks(_buffer, empty);
            return empty;
        }
    }
}