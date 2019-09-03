using System;
using CMineNew.Render.Object;
using OpenTK;

namespace CMineNew.Light{
    
    /// <summary>
    /// Represents a light with position and attenuation.
    /// </summary>
    public class PointLight{
        protected Vector3 _position;
        protected Vector3 _ambientColor, _diffuseColor, _specularColor;
        protected float _constantAttenuation, _linearAttenuation, _quadraticAttenuation;

        protected float _radius;

        /// <summary>
        /// Creates a point light.
        /// </summary>
        /// <param name="position">The position of the light.</param>
        /// <param name="ambientColor">The ambient color of the light.</param>
        /// <param name="diffuseColor">The diffuse color of the light.</param>
        /// <param name="specularColor">The specular color of the light.</param>
        /// <param name="constantAttenuation">The constant attenuation of the light.</param>
        /// <param name="linearAttenuation">The linear attenuation of the light.</param>
        /// <param name="quadraticAttenuation">The quadratic attenuation of the light.</param>
        public PointLight(Vector3 position, Vector3 ambientColor, Vector3 diffuseColor, Vector3 specularColor,
            float constantAttenuation, float linearAttenuation, float quadraticAttenuation) {
            _position = position;
            _ambientColor = ambientColor;
            _diffuseColor = diffuseColor;
            _specularColor = specularColor;
            _constantAttenuation = constantAttenuation;
            _linearAttenuation = linearAttenuation;
            _quadraticAttenuation = quadraticAttenuation;
            CalculateRadius();
        }

        /// <summary>
        /// THe position of the light.
        /// </summary>
        public Vector3 Position {
            get => _position;
            set => _position = value;
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
        /// The constant attenuation of the light.
        /// </summary>
        public float ConstantAttenuation {
            get => _constantAttenuation;
            set {
                _constantAttenuation = value;
                CalculateRadius();
            }
        }

        /// <summary>
        /// The linear attenuation of the light.
        /// </summary>
        public float LinearAttenuation {
            get => _linearAttenuation;
            set {
                _linearAttenuation = value;
                CalculateRadius();
            }
        }

        /// <summary>
        /// The quadratic attenuation of the light
        /// </summary>
        public float QuadraticAttenuation {
            get => _quadraticAttenuation;
            set {
                _quadraticAttenuation = value;
                CalculateRadius();
            }
        }

        /// <summary>
        /// The radius of the light.
        /// </summary>
        public float Radius => _radius;
        
        /// <summary>
        /// Sets the data of the light into a shader's uniforms.
        /// </summary>
        /// <param name="shader">The shader.</param>
        public virtual void ToShader(ShaderProgram shader) {
            shader.SetUVector("light.position", _position);
            shader.SetUVector("light.ambientColor", _ambientColor);
            shader.SetUVector("light.diffuseColor", _diffuseColor);
            shader.SetUVector("light.specularColor", _specularColor);
            shader.SetUFloat("light.constantAttenuation", _constantAttenuation);
            shader.SetUFloat("light.linearAttenuation", _linearAttenuation);
            shader.SetUFloat("light.quadraticAttenuation", _quadraticAttenuation);
        }

        /// <summary>
        /// Calculates the radius of the light.
        /// </summary>
        private void CalculateRadius() {
            var toSqrt = _linearAttenuation * _linearAttenuation -
                         4 * (_constantAttenuation - 100) * _quadraticAttenuation;
            if (toSqrt < 0) toSqrt = 0;
            toSqrt = (float) Math.Sqrt(toSqrt);
            _radius = (-_linearAttenuation + toSqrt) / (2 * _quadraticAttenuation);
        }
    }
}