using System;
using System.Collections.Generic;

namespace CMineNew.Map.Generator.Biomes{
    public class BiomeCollection{
        private readonly List<Biome> _biomes;
        private readonly List<int> _percentage;
        private int _totalPercentage;

        public List<Biome> Biomes => _biomes;

        public List<int> Percentage => _percentage;
        
        public BiomeCollection() {
            _biomes = new List<Biomes.Biome>();
            _percentage = new List<int>();
        }

        public void AddBiome(Biomes.Biome biome, int percentage) {
            _biomes.Add(biome);
            _percentage.Add(percentage);
            _totalPercentage += percentage;
        }

        public Biome GetBiome(float value) {
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