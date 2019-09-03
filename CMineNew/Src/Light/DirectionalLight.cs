using CMineNew.Render.Object;
using OpenTK;

namespace CMineNew.Light{
    
    /// <summary>
    /// Represents a light that covers the whole world, pointing to a direction.
    /// These lights are used to recreate global light sources, such as the Sun.
    /// </summary>
    public class DirectionalLight{
        protected Vector3 _direction;
        protected Vector3 _ambientColor, _diffuseColor, _specularColor;

        /// <summary>
        /// Creates a directional light.
        /// </summary>
        /// <param name="direction">The direction the light points to.</param>
        /// <param name="ambientColor">The ambient color of the light.</param>
        /// <param name="diffuseColor">The diffuse color of the light.</param>
        /// <param name="specularColor">The specular color of the light.</param>
        public DirectionalLight(Vector3 direction, Vector3 ambientColor, Vector3 diffuseColor, Vector3 specularColor) {
            _direction = direction;
            _ambientColor = ambientColor;
            _diffuseColor = diffuseColor;
            _specularColor = specularColor;
        }


        /// <summary>
        /// The direction the light points to.
        /// </summary>
        public Vector3 Direction {
            get => _direction;
            set => _direction = value;
        }

        /// <summary>
        /// The ambient color of the light.
        /// </summary>
        public Vector3 AmbientColor {
            get => _ambientColor;
            set => _ambientColor = value;
        }

        /// <summary>
        /// The diffuse color of the light.
        /// </summary>
        public Vector3 DiffuseColor {
            get => _diffuseColor;
            set => _diffuseColor = value;
        }

        /// <summary>
        /// The specular color of the light.º
        /// </summary>
        public Vector3 SpecularColor {
            get => _specularColor;
            set => _specularColor = value;
        }

        /// <summary>
        /// Sets the data of the light into a shader's uniforms.
        /// </summary>
        /// <param name="shader">The shader.</param>
        public virtual void ToShader(ShaderProgram shader) {
            shader.SetUVector("light.direction", _direction);
            shader.SetUVector("light.ambientColor", _ambientColor);
            shader.SetUVector("light.diffuseColor", _diffuseColor);
            shader.SetUVector("light.specularColor", _specularColor);
        }
    }
}