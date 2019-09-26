using CMineNew.Geometry;

namespace CMineNew.Map.BlockData{
    public class BlockLight{
        private BlockLightSource _source;
        private Vector3i _sunlightSource;
        private sbyte _blockLight, _linearSunlight, _sunlight;

        public BlockLight() {
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

        public sbyte Light {
            get => _blockLight;
            set => _blockLight = value;
        }

        public sbyte LinearSunlight {
            get => _linearSunlight;
            set => _linearSunlight = value;
        }

        public sbyte Sunlight {
            get => _sunlight;
            set => _sunlight = value;
        }
    }
}