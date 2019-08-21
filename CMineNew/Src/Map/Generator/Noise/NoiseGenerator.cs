using OpenTK;

namespace CMine.Map.Generator.Noise{
    public abstract class NoiseGenerator{
        protected readonly int[] _perm;
        protected Vector3d _offset;

        public NoiseGenerator() {
            _perm = new int[512];
        }

        public static int Floor(double x) {
            return x >= 0 ? (int) x : (int) x - 1;
        }

        public static double Fade(double x) {
            return x * x * x * (x * (x * 6 - 15) + 10);
        }

        public static double Lerp(double x, double y, double range) {
            return (1 - range) * x + range * y;
        }

        public static double Grad(int hash, double x, double y, double z) {
            hash &= 15;
            var u = hash < 8 ? x : y;
            var v = hash < 4 ? y : hash == 12 || hash == 14 ? x : z;
            return ((hash & 1) == 0 ? u : -u) + ((hash & 2) == 0 ? v : -v);
        }

        public abstract double Noise(double x, double y = 0, double z = 0);

        public double Noise(int octaves, double frequency, double amplitude, bool normalized,
            double x, double y = 0, double z = 0) {
            var result = 0d;
            var amp = 1d;
            var freq = 1d;
            var max = 0d;

            for (var i = 0; i < octaves; i++) {
                result += Noise(x * freq, y * freq, z * freq) * amp;
                max += amp;
                freq *= frequency;
                amp *= amplitude;
            }

            //Normalizes the result to [-1, 1]
            if (normalized) {
                result /= max;
            }

            return result;
        }
    }
}