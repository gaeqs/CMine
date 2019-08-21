using System;

namespace CMine.Map.Generator.Noise{
    public class SimplexOctaveGenerator : OctaveGenerator{
        private double _wScale = 1;

        public SimplexOctaveGenerator(int seed, int octaves) : this(new Random(seed), octaves) {
        }

        public SimplexOctaveGenerator(Random random, int octaves) : base(CreateOctaves(random, octaves)) {
        }

        public double WScale {
            get => _wScale;
            set => _wScale = value;
        }

        public override void SetScale(double scale) {
            base.SetScale(scale);
            _wScale = scale;
        }

        public double Noise(double frequency, double amplitude, bool normalized, double x, double y = 0, double z = 0,
            double w = 0) {
            var result = 0d;
            var amp = 1d;
            var freq = 1d;
            var max = 0d;

            x *= _scale.X;
            y *= _scale.Y;
            z *= _scale.Z;
            w *= _wScale;

            foreach (var octave in _octaves) {
                result += ((SimplexNoiseGenerator) octave).Noise(x * freq, y * freq, z * freq, w * freq) * amp;
                max += amp;
                freq *= frequency;
                amp *= amplitude;
            }

            if (normalized) {
                result /= max;
            }

            return result;
        }

        private static NoiseGenerator[] CreateOctaves(Random random, int octaves) {
            var array = new NoiseGenerator[octaves];
            for (var i = 0; i < octaves; i++) {
                array[i] = new SimplexNoiseGenerator(random);
            }

            return array;
        }
    }
}