using System;
using CMineNew.Map.Generator.Biomes;

namespace CMineNew.Map.Generator{
    public abstract class WorldGenerator{
        protected readonly int _seed;
        protected readonly World _world;
        protected BiomeGrid _biomeGrid;
        protected Random _random;

        public WorldGenerator(World world, BiomeGrid biomeGrid, int seed) {
            _seed = seed;
            _world = world;
            _biomeGrid = biomeGrid;
            _random = new Random();
        }

        public World World => _world;

        public Random Random => _random;

        public int Seed => _seed;

        public BiomeGrid BiomeGrid => _biomeGrid;

        public void SetRandomSeed(int seed) {
            _random = new Random(seed);
        }

        /// <summary>
        /// Generates the terrain of the chunk.
        /// </summary>
        /// <param name="chunk"></param>
        /// <returns>true whether the chunk is empty.</returns>
        public abstract bool GenerateChunkData(Chunk chunk);
    }
}