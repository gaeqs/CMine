namespace CMineNew.Map.BlockData{
    public class BlockLight{
        private BlockLightSource _source;
        private int _blockLight, _sunlight;
        private readonly int _blockLightPassReduction, _sunlightPassReduction;

        public BlockLight(int blockLightPassReduction, int sunlightPassReduction) {
            _blockLightPassReduction = blockLightPassReduction;
            _sunlightPassReduction = sunlightPassReduction;
            _blockLight = 0;
            _sunlight = 0;
        }


        public BlockLightSource Source {
            get => _source;
            set => _source = value;
        }

        public int Light {
            get => _blockLight;
            set => _blockLight = value;
        }

        public int Sunlight {
            get => _sunlight;
            set => _sunlight = value;
        }

        public int BlockLightPassReduction => _blockLightPassReduction;

        public int SunlightPassReduction => _sunlightPassReduction;
    }
}