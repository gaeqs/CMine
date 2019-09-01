using CMineNew.Render.Object;
using OpenTK;

namespace CMineNew.Light{
    public class DirectionalLight{
        protected Vector3 _direction;
        protected Vector3 _ambientColor, _diffuseColor, _specularColor;

        public DirectionalLight(Vector3 direction, Vector3 ambientColor, Vector3 diffuseColor, Vector3 specularColor) {
            _direction = direction;
            _ambientColor = ambientColor;
            _diffuseColor = diffuseColor;
            _specularColor = specularColor;
        }


        public Vector3 Direction {
            get => _direction;
            set => _direction = value;
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

        public virtual void ToShader(ShaderProgram shader) {
            shader.SetUVector("light.direction", _direction);
            shader.SetUVector("light.ambientColor", _ambientColor);
            shader.SetUVector("light.diffuseColor", _diffuseColor);
            shader.SetUVector("light.specularColor", _specularColor);
        }
    }
}