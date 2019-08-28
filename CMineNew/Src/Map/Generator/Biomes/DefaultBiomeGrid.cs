using CMineNew.Map.Generator.Biomes.Type;

namespace CMineNew.Map.Generator.Biomes{
    public class DefaultBiomeGrid : BiomeGrid{
        public DefaultBiomeGrid(World world, int seed)
            : base(seed, 8, 800, 8, 300, GetCollections(world, seed)) {
        }

        private static BiomeCollection[] GetCollections(World world, int seed) {
            var collections = new BiomeCollection[3];
            collections[(int) BiomeTemperature.Hot] = new BiomeCollection();
            collections[(int) BiomeTemperature.Hot].AddBiome(new BiomeDesert(world, seed), 30);
            collections[(int) BiomeTemperature.Normal] = new BiomeCollection();
            collections[(int) BiomeTemperature.Normal].AddBiome(new BiomePlains(world, seed), 30);
            collections[(int) BiomeTemperature.Normal].AddBiome(new BiomeForest(world, seed), 30);
            collections[(int) BiomeTemperature.Normal].AddBiome(new BiomeOcean(world, seed), 60);
            collections[(int) BiomeTemperature.Cold] = new BiomeCollection();
            collections[(int) BiomeTemperature.Cold].AddBiome(new BiomeMountains(world, seed), 30);
            return collections;
        }
    }
}