using System;
using CMineNew.Render.Object;
using OpenTK;

namespace CMineNew.Light{
    
    /// <summary>
    /// Represents a light with a position that illuminates only objects that are inside of a determined cone.
    /// </summary>
    public class FlashLight{
        protected Vector3 _position, _direction;
        protected Vector3 _ambientColor, _diffuseColor, _specularColor;
        protected float _constantAttenuation, _linearAttenuation, _quadraticAttenuation;
        protected float _cutOff, _outerCutOff, _cutOffCos, _outerCutOffCos;
        protected float _radius;

        /// <summary>
        /// Creates a flash light.
        /// </summary>
        /// <param name="position">The position of the light.</param>
        /// <param name="direction">The direction the light points to.</param>
        /// <param name="ambientColor">The ambient color of the light.</param>
        /// <param name="diffuseColor">The diffuse color of the light.</param>
        /// <param name="specularColor">The specular color of the light.</param>
        /// <param name="constantAttenuation">The constant attenuation of the light.</param>
        /// <param name="linearAttenuation">The linear attenuation of the light.</param>
        /// <param name="quadraticAttenuation">The quadratic attenuation of the light.</param>
        /// <param name="cutOff">The cone angle where objects are fully illuminated.</param>
        /// <param name="outerCutOff">The cone angle where objects are illuminated using a linear interpolation.</param>
        public FlashLight(Vector3 position, Vector3 direction, Vector3 ambientColor, Vector3 diffuseColor,
            Vector3 specularColor, float constantAttenuation, float linearAttenuation, float quadraticAttenuation,
            float cutOff, float outerCutOff) {
            _position = position;
            _direction = direction.Normalized();
            _ambientColor = ambientColor;
            _diffuseColor = diffuseColor;
            _specularColor = specularColor;
            _constantAttenuation = constantAttenuation;
            _linearAttenuation = linearAttenuation;
            _quadraticAttenuation = quadraticAttenuation;
            _cutOff = cutOff;
            _outerCutOff = outerCutOff;
            _cutOffCos = (float) Math.Cos(_cutOff * Math.PI / 180);
            _outerCutOffCos = (float) Math.Cos(_outerCutOff * Math.PI / 180);
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
        /// The direction the light points to.
        /// </summary>
        public Vector3 Direction {
            get => _direction;
            set => _direction = value.Normalized();
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
        /// The cone angle where objects are fully illuminated.
        /// </summary>
        public float CutOff {
            get => _cutOff;
            set {
                _cutOff = value;
                _cutOffCos = (float) Math.Cos(_cutOff * Math.PI / 180);
            }
        }

        /// <summary>
        /// The cone angle where objects are illuminated using a linear interpolation.
        /// </summary>
        public float OuterCutOff {
            get => _outerCutOff;
            set {
                _outerCutOff = value;
                _outerCutOffCos = (float) Math.Cos(_outerCutOff * Math.PI / 180);
            }
        }

        /// <summary>
        /// The cosine of the cutoff angle.
        /// </summary>
        public float CutOffCos => _cutOffCos;

        /// <summary>
        /// The cosine of the outer cutoff angle.
        /// </summary>
        public float OuterCutOffCos => _outerCutOffCos;


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
            shader.SetUVector("light.direction", _direction);
            shader.SetUVector("light.ambientColor", _ambientColor);
            shader.SetUVector("light.diffuseColor", _diffuseColor);
            shader.SetUVector("light.specularColor", _specularColor);
            shader.SetUFloat("light.constantAttenuation", _constantAttenuation);
            shader.SetUFloat("light.linearAttenuation", _linearAttenuation);
            shader.SetUFloat("light.quadraticAttenuation", _quadraticAttenuation);
            shader.SetUFloat("light.cutOff", _cutOffCos);
            shader.SetUFloat("light.outerCutOff", _outerCutOffCos);
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