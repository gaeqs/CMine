using System.Collections.Generic;
using System.Linq;

namespace CMineNew.Map.BlockData{
    public class BlockLight{
        private BlockLightSource _source;
        private int _light;
        private readonly int _lightPassReduction;

        public BlockLight(int lightPassReduction) {
            _lightPassReduction = lightPassReduction;
            _light = 0;
        }


        public BlockLightSource Source {
            get => _source;
            set => _source = value;
        }
        
        public int Light {
            get => _light;
            set => _light = value;
        }

        public int LightPassReduction => _lightPassReduction;
    }
}