using System;
using CMineNew.Render.Object;
using OpenTK;

namespace CMineNew.Light{
    public class PointLight{
        protected Vector3 _position;
        protected Vector3 _ambientColor, _diffuseColor, _specularColor;
        protected float _constantAttenuation, _linearAttenuation, _quadraticAttenuation;

        protected float _radius;

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

        public Vector3 Position {
            get => _position;
            set => _position = value;
        }

        public Vector3 AmbientColor {
            get => _ambientColor;
            set => _ambientColor = value;
        }

        public Vector3 DiffuseColor {
            get => _diffuseColor;
            set => _diffuseColor = value;
        }

        public Vector3 SpecularColor {
            get => _specularColor;
            set => _specularColor = value;
        }

        public float ConstantAttenuation {
            get => _constantAttenuation;
            set {
                _constantAttenuation = value;
                CalculateRadius();
            }
        }

        public float LinearAttenuation {
            get => _linearAttenuation;
            set {
                _linearAttenuation = value;
                CalculateRadius();
            }
        }

        public float QuadraticAttenuation {
            get => _quadraticAttenuation;
            set {
                _quadraticAttenuation = value;
                CalculateRadius();
            }
        }

        public float Radius => _radius;

        public virtual void ToShader(ShaderProgram shader) {
            shader.SetUVector("light.position", _position);
            shader.SetUVector("light.ambientColor", _ambientColor);
            shader.SetUVector("light.diffuseColor", _diffuseColor);
            shader.SetUVector("light.specularColor", _specularColor);
            shader.SetUFloat("light.constantAttenuation", _constantAttenuation);
            shader.SetUFloat("light.linearAttenuation", _linearAttenuation);
            shader.SetUFloat("light.quadraticAttenuation", _quadraticAttenuation);
        }

        private void CalculateRadius() {
            var toSqrt = _linearAttenuation * _linearAttenuation -
                         4 * (_constantAttenuation - 100) * _quadraticAttenuation;
            if (toSqrt < 0) toSqrt = 0;
            toSqrt = (float) Math.Sqrt(toSqrt);
            _radius = (-_linearAttenuation + toSqrt) / (2 * _quadraticAttenuation);
        }
    }
}