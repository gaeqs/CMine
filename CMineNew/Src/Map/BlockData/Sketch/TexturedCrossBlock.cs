using CMineNew.Geometry;

namespace CMineNew.Map.BlockData.Sketch{
    public class TexturedCrossBlock : CrossBlock{
        protected Area2d _textureArea;

        public TexturedCrossBlock(string id, Chunk chunk, Vector3i position, Area2d texture, bool passable = false)
            : base(id, chunk, position, passable) {
            _textureArea = texture;
        }

        public TexturedCrossBlock(string id, Chunk chunk, Vector3i position, string texture, bool passable = false)
            : base(id, chunk, position, passable) {
            _textureArea = CMine.Textures.Areas[texture];
        }

        public override Area2d TextureArea => _textureArea;

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new TexturedCubicBlock(_id, _chunk, _position, _textureArea);
        }
    }
}