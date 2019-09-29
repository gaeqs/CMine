using System;
using CMineNew.Render.Object;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Text{
    public class StaticTextRenderer{
        private static readonly int MaxObjects = 3000;
        private static readonly int InstanceDataLength = sizeof(float) * (2 + 2 + 4 + 4);

        private static readonly Vertex[] RectangleVerticesArray = {
            new Vertex(new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector2(0, 1)),
            new Vertex(new Vector3(1, 0, 0), new Vector3(0, 0, 1), new Vector2(1, 1)),
            new Vertex(new Vector3(0, 1, 0), new Vector3(0, 0, 1), new Vector2(0, 0)),
            new Vertex(new Vector3(1, 1, 0), new Vector3(0, 0, 1), new Vector2(1, 0))
        };

        private static readonly int[] RectangleIndicesArray = {0, 1, 3, 0, 3, 2};

        private float[] _buffer = new float[MaxObjects * InstanceDataLength / sizeof(float)];

        private ShaderProgram _shaderProgram;
        private VertexArrayObject _vertexArrayObject;
        private VertexBufferObject _dataVBO;

        public ShaderProgram ShaderProgram {
            get => _shaderProgram;
            set => _shaderProgram = value;
        }

        public StaticTextRenderer(ShaderProgram shader) {
            _shaderProgram = shader;
            _vertexArrayObject = new VertexArrayObject(RectangleVerticesArray, RectangleIndicesArray);
            _vertexArrayObject.Bind(); 
            _dataVBO = new VertexBufferObject();
            _dataVBO.Bind(BufferTarget.ArrayBuffer);
            _dataVBO.SetData(BufferTarget.ArrayBuffer, MaxObjects * InstanceDataLength, BufferUsageHint.StreamDraw);
            
            var builder = new AttributePointerBuilder(_vertexArrayObject, InstanceDataLength / sizeof(float), 3);
            builder.AddPointer(2, true);
            builder.AddPointer(2, true);
            builder.AddPointer(4, true);
            builder.AddPointer(4, true);
            
            VertexBufferObject.Unbind(BufferTarget.ArrayBuffer);
            VertexArrayObject.Unbind();
        }

        public void Draw(StaticText text) {
            var units = CMine.Window.UnitsPerPixel;
            var textCoords = text.Font.CharacterMap;
            var sizes = text.Font.CharactersSize;
            var amount = 0;
            var x = text.Position.X;
            var pointer = 0;

            foreach (var c in text.Text.ToCharArray()) {
                if (!textCoords.ContainsKey(c)) continue;
                var charSize = sizes[c];
                var coords = textCoords[c];
                _buffer[pointer++] = x;
                _buffer[pointer++] = text.Position.Y;

                _buffer[pointer++] = charSize.Width * units.X;
                _buffer[pointer++] = charSize.Height * units.Y;

                _buffer[pointer++] = text.Color.R;
                _buffer[pointer++] = text.Color.G;
                _buffer[pointer++] = text.Color.B;
                _buffer[pointer++] = text.Color.A;

                _buffer[pointer++] = coords.MinX;
                _buffer[pointer++] = coords.MinY;
                _buffer[pointer++] = coords.MaxX;
                _buffer[pointer++] = coords.MaxY;
                x += charSize.Width * units.X * 0.62f;
                amount++;
            }

            
            
            _dataVBO.Bind(BufferTarget.ArrayBuffer);
            _dataVBO.SetData(BufferTarget.ArrayBuffer, _buffer, BufferUsageHint.StreamDraw);

            _vertexArrayObject.Bind();
            _shaderProgram.Use();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, text.Font.TextureId);
            _vertexArrayObject.DrawnInstanced(amount);
            VertexBufferObject.Unbind(BufferTarget.ArrayBuffer);
            VertexArrayObject.Unbind();
        }
    }
}