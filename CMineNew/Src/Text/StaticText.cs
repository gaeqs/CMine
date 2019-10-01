using CMineNew.Render;
using CMineNew.Render.Object;
using CMineNew.Resources.Shaders;
using OpenTK;
using OpenTK.Graphics;

namespace CMineNew.Text{
    public class StaticText{
        public static readonly StaticTextRenderer DefaultRenderer = LoadDefaultRenderer();

        private float[] _buffer;
        private int _amount;

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
            FillBuffer();
        }

        public TrueTypeFont Font {
            get => _font;
            set => _font = value;
        }

        public string Text {
            get => _text;
            set {
                _text = value;
                FillBuffer();
            }
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
            DefaultRenderer.Draw(_buffer, _amount, _font);
        }

        public virtual void Tick(long dif, Room room) {
        }

        private void FillBuffer() {
            _buffer = new float[_text.Length * 12];
            _amount = 0;
            var units = CMine.Window.UnitsPerPixel;
            var textCoords = _font.CharacterMap;
            var sizes = _font.CharactersSize;
            var x = _position.X;
            var pointer = 0;
            foreach (var c in _text.ToCharArray()) {
                if (!textCoords.ContainsKey(c)) continue;
                var charSize = sizes[c];
                var coords = textCoords[c];
                _buffer[pointer++] = x;
                _buffer[pointer++] = _position.Y;

                _buffer[pointer++] = charSize.Width * units.X;
                _buffer[pointer++] = charSize.Height * units.Y;

                _buffer[pointer++] = _color.R;
                _buffer[pointer++] = _color.G;
                _buffer[pointer++] = _color.B;
                _buffer[pointer++] = _color.A;

                _buffer[pointer++] = coords.MinX;
                _buffer[pointer++] = coords.MinY;
                _buffer[pointer++] = coords.MaxX;
                _buffer[pointer++] = coords.MaxY;
                x += charSize.Width * units.X * 0.62f;
                _amount++;
            }
        }
    }
}