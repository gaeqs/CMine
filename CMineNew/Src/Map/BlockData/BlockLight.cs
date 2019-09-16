using CMineNew.Geometry;

namespace CMineNew.Map.BlockData{
    public class BlockLight{
        private BlockLightSource _source;
        private Vector3i _sunlightSource;
        private int _blockLight, _linearSunlight, _sunlight;
        private readonly int _blockLightPassReduction, _sunlightPassReduction;

        public BlockLight(int blockLightPassReduction, int sunlightPassReduction) {
            _blockLightPassReduction = blockLightPassReduction;
            _sunlightPassReduction = sunlightPassReduction;
            _blockLight = 0;
            _linearSunlight = 0;
        }


        public BlockLightSource Source {
            get => _source;
            set => _source = value;
        }

        public Vector3i SunlightSource {
            get => _sunlightSource;
            set => _sunlightSource = value;
        }

        public int Light {
            get => _blockLight;
            set => _blockLight = value;
        }

        public int LinearSunlight {
            get => _linearSunlight;
            set => _linearSunlight = value;
        }

        public int Sunlight {
            get => _sunlight;
            set => _sunlight = value;
        }

        public int BlockLightPassReduction => _blockLightPassReduction;

        public int SunlightPassReduction => _sunlightPassReduction;
    }
}