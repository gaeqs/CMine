using CMineNew.Geometry;
using CMineNew.Map;
using CMineNew.Map.BlockData.Snapshot;
using CMineNew.Map.BlockData.Snapshot.Interface;
using CMineNew.Render.Object;
using OpenTK;

namespace CMineNew.Render.Gui {
    public class GuiBlockElement {
        protected readonly string _name;
        protected BlockSnapshot _snapshot;
        protected Vector2 _position;
        protected Vector3 _size;
        protected readonly int[] _textureIndices;
        protected AspectRatioMode _aspectRatioMode;

        protected Matrix4 _matrix;

        public GuiBlockElement(string name, Vector2 position, Vector3 size, BlockSnapshot snapshot,
            AspectRatioMode aspectRatioMode) {
            _name = name;
            _position = position;
            _size = size;
            _snapshot = snapshot;
            _aspectRatioMode = aspectRatioMode;

            _textureIndices = new int[6];
            RefreshTextures();
            GenerateMatrix();
        }

        public string Name => _name;

        public Vector2 Position {
            get => _position;
            set => _position = value;
        }

        public Vector3 Size {
            get => _size;
            set => _size = value;
        }

        public BlockSnapshot Snapshot {
            get => _snapshot;
            set {
                _snapshot = value;
                RefreshTextures();
            }
        }

        public AspectRatioMode AspectRatioMode {
            get => _aspectRatioMode;
            set => _aspectRatioMode = value;
        }

        public Vector3 GetSizeWithAspectRatio(float aspectRatio) {
            return _aspectRatioMode switch {
                AspectRatioMode.PreserveXModifyY => (_size * new Vector3(1, aspectRatio, 1)),
                AspectRatioMode.PreserveYModifyX => (_size * new Vector3(1 / aspectRatio, 1, 1)),
                _ => _size
            };
        }

        public virtual void Draw(World world, ShaderProgram shader, VertexArrayObject vao) {
            if(_snapshot == null) return;
            GenerateMatrix();
            
            shader.SetUVector("instancePosition", _position);
            shader.SetUVector("instanceSize", GetSizeWithAspectRatio(world.Camera.Frustum.AspectRatio));
            shader.SetUVector("colorFilter", _snapshot is IGrass grass ? grass.GrassColor.ToVector() : Vector4.Zero);
            shader.SetUInt("instanceTextureIndices[0]", _textureIndices[0]);
            shader.SetUInt("instanceTextureIndices[1]", _textureIndices[1]);
            shader.SetUInt("instanceTextureIndices[2]", _textureIndices[2]);
            shader.SetUInt("instanceTextureIndices[3]", _textureIndices[3]);
            shader.SetUInt("instanceTextureIndices[4]", _textureIndices[4]);
            shader.SetUInt("instanceTextureIndices[5]", _textureIndices[5]);
            shader.SetUMatrix("model", _matrix);

            vao.Draw();
        }

        private void RefreshTextures() {
            if(_snapshot == null) return;
            for (var i = 0; i < 6; i++) {
                _textureIndices[i] = _snapshot.Data.GetTextureIndex((BlockFace) i);
            }
        }

        private void GenerateMatrix() {
            _matrix = Matrix4.CreateRotationY(0.3f) * Matrix4.CreateRotationX(0.6f) * Matrix4.CreateRotationZ(0);
        }
    }
}