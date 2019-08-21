using OpenTK;

namespace CMineNew.Render.Object{
    public struct Vertex{
        public const int Size = 8;


        public Vector3 _position;
        public Vector3 _normal;
        public Vector2 _textureCoords;

        public Vertex(float px, float py, float pz, float nx, float ny, float nz, float tx, float ty) {
            _position = new Vector3(px, py, pz);
            _normal = new Vector3(nx, ny, nz).Normalized();
            _textureCoords = new Vector2(tx, ty);
        }

        public Vertex(Vector3 position, Vector3 normal, Vector2 textureCoords) {
            _position = position;
            _normal = normal;
            _textureCoords = textureCoords;
        }

        public override string ToString() {
            return "P: " + _position + " N: " + _normal + " TC: " + _textureCoords;
        }
    }
}