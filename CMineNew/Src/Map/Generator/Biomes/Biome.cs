using CMineNew.Color;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Snapshot;
using OpenTK.Graphics;

namespace CMineNew.Map.Generator.Biomes{
    public abstract class Biome{
        protected readonly string _id;
        protected readonly BiomeTemperature _temperature;

        protected readonly int _minHeight;
        protected readonly int _maxHeight;

        protected readonly Rgba32I _grassColor;

        protected readonly World _world;
        protected readonly int _seed;

        public Biome(string id, BiomeTemperature temperature, int minHeight, int maxHeight, Rgba32I grassColor, World world,
            int seed) {
            _id = id;
            _temperature = temperature;
            _minHeight = minHeight;
            _maxHeight = maxHeight;
            _grassColor = grassColor;
            _world = world;
            _seed = seed;
        }

        public string Id => _id;

        public BiomeTemperature Temperature => _temperature;

        public int MinHeight => _minHeight;

        public int MaxHeight => _maxHeight;

        public Rgba32I GrassColor => _grassColor;

        public World World => _world;
        public int Seed => _seed;

        public int GetColumnHeight(Vector2i position) {
            return GetColumnHeight(position.X, position.Y);
        }
        
        public abstract int GetColumnHeight(int x, int z);

        public abstract BlockSnapshot GetBlockSnapshot(Vector3i position, int columnHeight, Rgba32I grassColor);
    }


    public enum BiomeTemperature{
        Hot = 0,
        Normal = 1,
        Cold = 2
    }
}