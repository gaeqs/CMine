using System.Collections.Generic;
using System.Linq;
using CMineNew.Geometry;

namespace CMineNew.Map.BlockData{
    public class BlockLight{
        private readonly Dictionary<Vector3i, int> _sources;
        private Vector3i _source;
        private int _light;

        private bool _isSource;
        private int _sourceLight;
        private int _lightPassReduction;

        public BlockLight(bool isSource, int sourceLight, int lightPassReduction) {
            _sources = new Dictionary<Vector3i, int>();
            _isSource = isSource;
            _sourceLight = sourceLight;
            _lightPassReduction = lightPassReduction;
        }

        public Vector3i Source => _source;

        public Dictionary<Vector3i, int> Sources => _sources;

        public int Light => _light;

        public bool IsSource => _isSource;

        public int SourceLight => _sourceLight;

        public int LightPassReduction => _lightPassReduction;

        public bool TryGetSourceLight(Vector3i source, out int light) {
            return _sources.TryGetValue(source, out light);
        }

        public bool AddSource(Vector3i source, int light) {
            if (_sources.TryGetValue(source, out var oldLight) && oldLight >= light) {
                return false;
            }

            _sources[source] = light;
            return CalculateLight();
        }

        public bool RemoveSource(Vector3i source) {
            _sources.Remove(source);
            return _source == source && CalculateLight();
        }


        private bool CalculateLight() {
            var oldLight = _light;
            _light = 0;
            foreach (var source in _sources.Where(source => _light < source.Value)) {
                _light = source.Value;
                _source = source.Key;
            }

            return _light == oldLight;
        }
    }
}