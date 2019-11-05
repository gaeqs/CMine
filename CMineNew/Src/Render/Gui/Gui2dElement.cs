using CMineNew.Color;
using CMineNew.Map;
using CMineNew.Render.Object;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Render.Gui {
    public class Gui2dElement {
        private readonly string _name;
        private Vector2 _position;
        private Vector2 _size;
        private Texture _texture;
        private AspectRatioMode _aspectRatioMode;

        public Gui2dElement(string name, Vector2 position, Vector2 size, Texture texture,
            AspectRatioMode aspectRatioMode) {
            _name = name;
            _position = position;
            _size = size;
            _texture = texture;
            _aspectRatioMode = aspectRatioMode;
        }

        public string Name => _name;

        public Vector2 Position {
            get => _position;
            set => _position = value;
        }

        public Vector2 Size {
            get => _size;
            set => _size = value;
        }

        public Texture Texture {
            get => _texture;
            set => _texture = value;
        }

        public Vector2 GetSizeWithAspectRatio(float aspectRatio) {
            return _aspectRatioMode switch {
                AspectRatioMode.PreserveXModifyY => (_size * new Vector2(1, aspectRatio)),
                AspectRatioMode.PreserveYModifyX => (_size * new Vector2(1 / aspectRatio, 1)),
                _ => _size
            };
        }

        public virtual void Draw(World world, ShaderProgram shader, VertexArrayObject vao) {
            shader.SetUVector("instancePosition", _position);
            shader.SetUVector("instanceSize", GetSizeWithAspectRatio(world.Camera.Frustum.AspectRatio));
            _texture.BindAs(TextureUnit.Texture0);
            vao.Draw();
        }
    }
}