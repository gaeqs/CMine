using CMineNew.Geometry;
using CMineNew.Map.BlockData.Snapshot;

namespace CMineNew.Map.Generator.Biomes{
    public abstract class Biome{
        protected readonly BiomeTemperature _temperature;

        protected readonly int _minHeight;
        protected readonly int _maxHeight;

        protected readonly int _seed;

        public Biome(BiomeTemperature temperature, int minHeight, int maxHeight, int seed) {
            _temperature = temperature;
            _minHeight = minHeight;
            _maxHeight = maxHeight;
            _seed = seed;
        }

        public BiomeTemperature Temperature => _temperature;

        public int MinHeight => _minHeight;

        public int MaxHeight => _maxHeight;

        public int Seed => _seed;

        public abstract int GetColumnHeight(int x, int z);

        public abstract BlockSnapshot GetBlockSnapshot(Vector3i position, int columnHeight);
    }


    public enum BiomeTemperature{
        Hot = 0,
        Normal = 1,
        Cold = 2
    }
}