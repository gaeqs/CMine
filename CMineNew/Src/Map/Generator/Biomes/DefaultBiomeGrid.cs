using CMineNew.Map.Generator.Biomes.Type;

namespace CMineNew.Map.Generator.Biomes{
    public class DefaultBiomeGrid : BiomeGrid{
        public DefaultBiomeGrid(int seed)
            : base(seed, 8, 800, 8, 300, GetCollections(seed)) {
        }

        private static BiomeCollection[] GetCollections(int seed) {
            var collections = new BiomeCollection[3];
            collections[(int) BiomeTemperature.Hot] = new BiomeCollection();
            collections[(int) BiomeTemperature.Hot].AddBiome(new BiomeDesert(seed), 30);
            collections[(int) BiomeTemperature.Normal] = new BiomeCollection();
            collections[(int) BiomeTemperature.Normal].AddBiome(new BiomePlains(seed), 30);
            collections[(int) BiomeTemperature.Normal].AddBiome(new BiomeForest(seed), 30);
            collections[(int) BiomeTemperature.Normal].AddBiome(new BiomeOcean(seed), 25);
            collections[(int) BiomeTemperature.Cold] = new BiomeCollection();
            collections[(int) BiomeTemperature.Cold].AddBiome(new BiomeMountains(seed), 30);
            return collections;
        }
    }
}