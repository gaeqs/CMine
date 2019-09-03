using CMineNew.Render;
using CMineNew.Render.Object;
using OpenTK;

namespace CMineNew.Light{
    public class CameraFlashLight : FlashLight{
        protected Camera _camera;

        public CameraFlashLight(Camera camera, Vector3 ambientColor, Vector3 diffuseColor, Vector3 specularColor,
            float constantAttenuation, float linearAttenuation, float quadraticAttenuation, float cutOff,
            float outerCutOff) :
            base(camera.Position, camera.LookAt, ambientColor, diffuseColor, specularColor, constantAttenuation,
                linearAttenuation,
                quadraticAttenuation, cutOff, outerCutOff) {
            _camera = camera;
        }

        public override void ToShader(ShaderProgram shader) {
            _position = _camera.Position;
            _direction = _camera.LookAt;
            base.ToShader(shader);
        }
    }
}