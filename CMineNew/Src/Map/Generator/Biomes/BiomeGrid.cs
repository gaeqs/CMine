using System.Collections.Generic;
using CMine.Map.Generator.Noise;
using CMineNew.Geometry;

namespace CMineNew.Map.Generator.Biomes{
    public class BiomeGrid{
        private readonly Dictionary<string, Biome> _allBiomes;

        private readonly OctaveGenerator _temperatureGenerator;
        private readonly OctaveGenerator _biomeGenerator;

        private readonly BiomeCollection[] _biomes;
        private readonly Biome _defaultBiome;

        public BiomeGrid(int seed, int temperatureOctaves, int temperatureSize, int biomeOctaves, int biomeSize,
            BiomeCollection[] biomes, Biome defaultBiome) {
            _temperatureGenerator = new SimplexOctaveGenerator(seed, temperatureOctaves);
            _biomeGenerator = new SimplexOctaveGenerator(seed, biomeOctaves);
            _temperatureGenerator.SetScale(1f / temperatureSize);
            _biomeGenerator.SetScale(1f / biomeSize);
            _biomes = biomes;
            _defaultBiome = defaultBiome;
            _allBiomes = new Dictionary<string, Biome>();
            GenerateBiomesMap();
        }

        public BiomeGrid(OctaveGenerator temperatureGenerator, OctaveGenerator biomeGenerator,
            BiomeCollection[] biomes, Biome defaultBiome) {
            _temperatureGenerator = temperatureGenerator;
            _biomeGenerator = biomeGenerator;
            _biomes = biomes;
            _defaultBiome = defaultBiome;
            _allBiomes = new Dictionary<string, Biome>();
            GenerateBiomesMap();
        }

        private void GenerateBiomesMap() {
            foreach (var collection in _biomes) {
                foreach (var biome in collection.Biomes) {
                    _allBiomes.Add(biome.Id, biome);
                }
            }
        }

        public OctaveGenerator TemperatureGenerator => _temperatureGenerator;

        public OctaveGenerator BiomeGenerator => _biomeGenerator;

        public BiomeCollection[] Biomes => _biomes;

        public Dictionary<string, Biome> AllBiomes => _allBiomes;

        public Biome DefaultBiome => _defaultBiome;

        public Biome GetBiome(Vector3i position) {
            return GetBiome(position.X, position.Z);
        }

        public Biome GetBiome(Vector2i position) {
            return GetBiome(position.X, position.Y);
        }

        public virtual Biome GetBiome(int x, int z) {
            var tNoise = _temperatureGenerator.Noise(1, 4, true, x, z);

            var temperature = BiomeTemperature.Cold;
            if (tNoise > 0.333) {
                temperature = BiomeTemperature.Hot;
            }
            else if (tNoise > -0.333) {
                temperature = BiomeTemperature.Normal;
            }

            var biomes = _biomes[(int) temperature];
            var bNoise = (float) _biomeGenerator.Noise(1, 500, true, x, z);
            return biomes.GetBiome(bNoise / 2 + 0.5f);
        }

        public Biome GetBiomeOrDefault(string id) {
            return _allBiomes.TryGetValue(id, out var value) ? value : _defaultBiome;
        }
    }
}