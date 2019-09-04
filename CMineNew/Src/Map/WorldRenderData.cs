using CMineNew.Entities;
using CMineNew.Light;
using CMineNew.Map.BlockData;
using CMineNew.Map.BlockData.Model;
using CMineNew.RayTrace;
using CMineNew.Render;
using CMineNew.Resources.Textures;
using OpenTK;

namespace CMineNew.Map{
    public class WorldRenderData{

        private readonly PhysicCamera _camera;
        private readonly WorldGBuffer _gBuffer;
        private readonly LightManager _lightManager;
        private readonly SkyBox _skyBox;

        public WorldRenderData() {
            _camera = new PhysicCamera(new Vector3(0), new Vector2(0, 0), new Vector3(0, 1, 0), 110);
            _gBuffer = new WorldGBuffer(CMine.Window);
            _lightManager = new LightManager();
            _skyBox = new SkyBox(Textures.sky_box_right, Textures.sky_box_left, Textures.sky_box_top,
                Textures.sky_box_bottom, Textures.sky_box_front, Textures.sky_box_back);

            var vec = new Vector3(0.5f, 0.5f, 0.5f);
            _lightManager.AddDirectionalLight(new DirectionalLight(new Vector3(-1, -1, 0),
                vec, vec, vec));
        }

        public PhysicCamera Camera => _camera;

        public WorldGBuffer GBuffer => _gBuffer;

        public LightManager LightManager => _lightManager;

        public SkyBox SkyBox => _skyBox;

        public void BindGBuffer() {
            _gBuffer.Bind();
        }

        public void DrawSkyBox() {
            _skyBox.Draw(_camera);
        }
        
        public void DrawSelectedBlock(Block block) {
            block?.BlockModel.DrawLines(_camera, block);
        }

        public void TransferDepthBufferToMainFbo() {
            _gBuffer.TransferDepthBufferToMainFbo();
        }

        public void DrawPointer() {
            Pointer.Draw(_camera);
        }
        
        public void DrawLights() {
            _gBuffer.DrawLights(_lightManager, _camera.Position);
        }

        public void DrawGBuffer(bool waterShader) {
            _gBuffer.Draw(_camera.Position, Vector3.One, 0.2f, waterShader, _skyBox);
        }

        public void CameraTick(Player player, long delay) {
            _camera.ToPosition = player.Position + new Vector3(0, player.EyesHeight, 0);
            _camera.ToRotation = player.HeadRotation;
            _camera.Tick(delay);
        }
    }
}