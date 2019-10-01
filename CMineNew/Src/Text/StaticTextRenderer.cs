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

        public void Draw(float[] buffer, int amount, TrueTypeFont font) {
            _dataVBO.Bind(BufferTarget.ArrayBuffer);
            _dataVBO.SetSubData(BufferTarget.ArrayBuffer, buffer, 0);

            _vertexArrayObject.Bind();
            _shaderProgram.Use();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, font.TextureId);
            _vertexArrayObject.DrawnInstanced(amount);
            VertexBufferObject.Unbind(BufferTarget.ArrayBuffer);
            VertexArrayObject.Unbind();
        }
    }
}