using System.Collections.Generic;

namespace CMineNew.Light{

    /// <summary>
    /// Represents a manager that stores all lights in a world.
    /// </summary>
    public class LightManager{
        private readonly List<DirectionalLight> _directionalLights;
        private readonly List<PointLight> _pointLights;
        private readonly List<FlashLight> _flashLights;

        /// <summary>
        /// Creates a light manager.
        /// </summary>
        public LightManager() {
            _directionalLights = new List<DirectionalLight>();
            _pointLights = new List<PointLight>();
            _flashLights = new List<FlashLight>();
        }

        /// <summary>
        /// The mutable list where directional lights are stored.
        /// </summary>
        public List<DirectionalLight> DirectionalLights => _directionalLights;

        /// <summary>
        /// The mutable list where point lights are stored.
        /// </summary>
        public List<PointLight> PointLights => _pointLights;

        /// <summary>
        /// The mutable list where flash lights are stored.
        /// </summary>
        public List<FlashLight> FlashLights => _flashLights;
    }
}