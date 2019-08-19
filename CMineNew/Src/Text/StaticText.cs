using CMineNew.Render;
using CMineNew.Render.Object;
using CMineNew.Resources.Shaders;
using OpenTK;
using OpenTK.Graphics;

namespace CMineNew.Text{
    public class StaticText{
        public static readonly StaticTextRenderer DefaultRenderer = LoadDefaultRenderer();

        private static StaticTextRenderer LoadDefaultRenderer() {
            var textProgram = new ShaderProgram(Shaders.static_text_vertex, Shaders.static_text_fragment);
            return new StaticTextRenderer(textProgram);
        }


        private TrueTypeFont _font;
        private string _text;
        private Vector2 _position;
        private Color4 _color;

        public StaticText(TrueTypeFont font, string text, Vector2 position, Color4 color) {
            _font = font;
            _text = text;
            _position = position;
            _color = color;
        }

        public TrueTypeFont Font {
            get => _font;
            set => _font = value;
        }

        public string Text {
            get => _text;
            set => _text = value;
        }

        public Vector2 Position {
            get => _position;
            set => _position = value;
        }

        public Color4 Color {
            get => _color;
            set => _color = value;
        }

        public void Draw() {
            DefaultRenderer.Draw(this);
        }

        public virtual void Tick(long dif, Room room) {
        }
    }
}