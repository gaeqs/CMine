using CMineNew.Geometry;
using CMineNew.Map.BlockData.Snapshot;
using OpenTK.Graphics;

namespace CMineNew.Map.Generator.Biomes{
    public abstract class Biome{
        protected readonly BiomeTemperature _temperature;

        protected readonly int _minHeight;
        protected readonly int _maxHeight;

        protected readonly Color4 _grassColor;

        protected readonly World _world;
        protected readonly int _seed;

        public Biome(BiomeTemperature temperature, int minHeight, int maxHeight, Color4 grassColor, World world,
            int seed) {
            _temperature = temperature;
            _minHeight = minHeight;
            _maxHeight = maxHeight;
            _grassColor = grassColor;
            _world = world;
            _seed = seed;
        }

        public BiomeTemperature Temperature => _temperature;

        public int MinHeight => _minHeight;

        public int MaxHeight => _maxHeight;

        public Color4 GrassColor => _grassColor;

        public World World => _world;
        public int Seed => _seed;

        public int GetColumnHeight(Vector2i position) {
            return GetColumnHeight(position.X, position.Y);
        }
        
        public abstract int GetColumnHeight(int x, int z);

        public abstract BlockSnapshot GetBlockSnapshot(Vector3i position, int columnHeight, Color4 grassColor);
    }


    public enum BiomeTemperature{
        Hot = 0,
        Normal = 1,
        Cold = 2
    }
}