using System;

namespace CMine.Map.Generator.Noise{
    public class PerlinOctaveGenerator : OctaveGenerator{
        public PerlinOctaveGenerator(int seed, int octaves) : this(new Random(seed), octaves) {
        }

        public PerlinOctaveGenerator(Random random, int octaves) : base(CreateOctaves(random, octaves)) {
        }


        private static NoiseGenerator[] CreateOctaves(Random random, int octaves) {
            var array = new NoiseGenerator[octaves];
            for (var i = 0; i < octaves; i++) {
                array[i] = new PerlinNoiseGenerator(random);
            }

            return array;
        }
    }
}