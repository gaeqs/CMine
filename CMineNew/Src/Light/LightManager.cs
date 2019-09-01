using System.Collections.Generic;

namespace CMineNew.Light{
    public class LightManager{
        private readonly List<DirectionalLight> _directionalLights;
        private readonly List<PointLight> _pointLights;
        private readonly List<FlashLight> _flashLights;

        public LightManager() {
            _directionalLights = new List<DirectionalLight>();
            _pointLights = new List<PointLight>();
            _flashLights = new List<FlashLight>();
        }

        public List<DirectionalLight> DirectionalLights => _directionalLights;

        public List<PointLight> PointLights => _pointLights;

        public List<FlashLight> FlashLights => _flashLights;
    }
}