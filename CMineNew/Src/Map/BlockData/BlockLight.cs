using System.Collections.Generic;
using System.Linq;

namespace CMineNew.Map.BlockData{
    public class BlockLight{
        private readonly Dictionary<BlockLightSource, int> _sources;
        private BlockLightSource _source;
        private int _light;

        private readonly int _lightPassReduction;
        private readonly UpdateRender _updateRender;

        public BlockLight(int lightPassReduction, UpdateRender updateRender) {
            _sources = new Dictionary<BlockLightSource, int>();
            _lightPassReduction = lightPassReduction;
            _updateRender = updateRender;
        }

        public BlockLightSource Source => _source;

        public Dictionary<BlockLightSource, int> Sources => _sources;

        public int Light => _light;
        public int LightPassReduction => _lightPassReduction;

        public bool TryGetSourceLight(BlockLightSource source, out int light) {
            return _sources.TryGetValue(source, out light);
        }

        public bool ContainsBrightestSource(BlockLightSource source, int light) {
            return _sources.TryGetValue(source, out var oldLight) && oldLight >= light;
        }
        
        public bool AddSource(BlockLightSource source, int light) {
            if (_sources.TryGetValue(source, out var oldLight) && oldLight >= light) {
                return false;
            }

            source.Blocks.Add(this);
            _sources[source] = light;
            if (CalculateLight()) {
                _updateRender.Invoke(_light);
            }

            return true;
        }


        public void RemoveSource(BlockLightSource source) {
            _sources.Remove(source);
            if (CalculateLight()) {
                _updateRender.Invoke(_light);
            }
        }

        private bool CalculateLight() {
            var oldLight = _light;
            _light = 0;
            foreach (var source in _sources.Where(source => _light < source.Value)) {
                _light = source.Value;
                _source = source.Key;
            }

            return _light != oldLight;
        }


        public delegate void UpdateRender(int light);
    }
}