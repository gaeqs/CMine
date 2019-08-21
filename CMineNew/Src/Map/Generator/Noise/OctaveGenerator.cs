using OpenTK;

namespace CMine.Map.Generator.Noise{
    public class OctaveGenerator{
        protected readonly NoiseGenerator[] _octaves;
        protected Vector3d _scale;

        protected OctaveGenerator(NoiseGenerator[] octaves) {
            _octaves = octaves;
            _scale = Vector3d.One;
        }

        public Vector3d Scale {
            get => _scale;
            set => _scale = value;
        }

        public virtual void SetScale(double scale) {
            _scale = new Vector3d(scale);
        }

        public NoiseGenerator[] Octaves => _octaves;

        public double Noise(double frequency, double amplitude, bool normalized, double x, double y = 0, double z = 0) {
            var result = 0d;
            var amp = 1d;
            var freq = 1d;
            var max = 0d;

            x *= _scale.X;
            y *= _scale.Y;
            z *= _scale.Z;

            foreach (var octave in _octaves) {
                result += octave.Noise(x * freq, y * freq, z * freq) * amp;
                max += amp;
                freq *= frequency;
                amp *= amplitude;
            }

            if (normalized) {
                result /= max;
            }

            return result;
        }
    }
}