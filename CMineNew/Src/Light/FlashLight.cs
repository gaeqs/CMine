using System;
using CMineNew.Render.Object;
using OpenTK;

namespace CMineNew.Light{
    public class FlashLight{
        protected Vector3 _position, _direction;
        protected Vector3 _ambientColor, _diffuseColor, _specularColor;
        protected float _constantAttenuation, _linearAttenuation, _quadraticAttenuation;
        protected float _cutOff, _outerCutOff, _cutOffCos, _outerCutOffCos;
        protected float _radius;

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

        public Vector3 Position {
            get => _position;
            set => _position = value;
        }

        public Vector3 Direction {
            get => _direction;
            set => _direction = value.Normalized();
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

        public float CutOff {
            get => _cutOff;
            set {
                _cutOff = value;
                _cutOffCos = (float) Math.Cos(_cutOff * Math.PI / 180);
            }
        }

        public float OuterCutOff {
            get => _outerCutOff;
            set {
                _outerCutOff = value;
                _outerCutOffCos = (float) Math.Cos(_outerCutOff * Math.PI / 180);
            }
        }

        public float CutOffCos => _cutOffCos;

        public float OuterCutOffCos => _outerCutOffCos;


        public float Radius => _radius;


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

        private void CalculateRadius() {
            var toSqrt = _linearAttenuation * _linearAttenuation -
                         4 * (_constantAttenuation - 100) * _quadraticAttenuation;
            if (toSqrt < 0) toSqrt = 0;
            toSqrt = (float) Math.Sqrt(toSqrt);
            _radius = (-_linearAttenuation + toSqrt) / (2 * _quadraticAttenuation);
        }
    }
}