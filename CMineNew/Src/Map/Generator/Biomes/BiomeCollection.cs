using System.Collections.Generic;

namespace CMineNew.Map.Generator.Biomes{
    public class BiomeCollection{
        private readonly List<Biomes.Biome> _biomes;
        private readonly List<int> _percentage;
        private int _totalPercentage;

        public BiomeCollection() {
            _biomes = new List<Biomes.Biome>();
            _percentage = new List<int>();
        }

        public void AddBiome(Biomes.Biome biome, int percentage) {
            _biomes.Add(biome);
            _percentage.Add(percentage);
            _totalPercentage += percentage;
        }

        public Biomes.Biome GetBiome(float value) {
            value *= _totalPercentage;
            for (var i = 0; i < _percentage.Count; i++) {
                var percentage = _percentage[i];
                if (value <= percentage) {
                    return _biomes[i];
                }

                value -= percentage;
            }

            return _biomes[_percentage.Count - 1];
        }
    }
}