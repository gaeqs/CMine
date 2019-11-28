using CMineNew.Entities;
using CMineNew.Geometry;
using CMineNew.Map.BlockData;
using CMineNew.Render;
using CMineNew.Render.Gui;
using CMineNew.Resources.Textures;
using OpenTK;

namespace CMineNew.Map{
    public class WorldRenderData{
        private readonly PhysicCamera _camera;
        private readonly WorldGBuffer _gBuffer;
        private readonly SkyBox _skyBox;
        private readonly WorldShaderData _shaderData;

        public WorldRenderData() {
            _camera = new PhysicCamera(new Vector3(0), new Vector2(0, 0), new Vector3(0, 1, 0), 110);
            _gBuffer = new WorldGBuffer(CMine.Window);
            _skyBox = new SkyBox(Textures.sky_box_right, Textures.sky_box_left, Textures.sky_box_top,
                Textures.sky_box_bottom, Textures.sky_box_front, Textures.sky_box_back);
            _shaderData = new WorldShaderData();
        }

        public PhysicCamera Camera => _camera;

        public WorldGBuffer GBuffer => _gBuffer;
        public SkyBox SkyBox => _skyBox;

        public WorldShaderData ShaderData => _shaderData;

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

        public void DrawGBuffer(bool waterShader) {
            _gBuffer.DrawSSAO(_camera.Frustum.Matrix.Inverted());
            _gBuffer.Draw(_camera, Vector3.One, 0.000f, waterShader, _skyBox);
        }

        public void CameraTick(Player player, long delay) {
            _camera.ToPosition = player.RenderPosition + new Vector3(0, player.EyesHeight, 0);
            _camera.ToRotation = player.HeadRotation;
            _camera.Tick(delay);
        }

        public void SetShaderData(Vector3 sunlightDirection, bool waterShader, long ticks) {
            const int min = (CMine.ChunkRadius - 2) << 4;
            const int max = (CMine.ChunkRadius - 1) << 4;
            _shaderData.SetData(_camera.Matrix, _camera.Frustum.Matrix, _camera.Position,
                sunlightDirection, min * min, max * max, waterShader, ticks,
                CMine.TextureMap.SpriteSizeNormalized, CMine.TextureMap.TextureLength,
                new Vector2i(CMine.Window.Width, CMine.Window.Height));
        }
    }
}