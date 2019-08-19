using CMineNew.Geometry;

namespace CMineNew.Map.BlockData.Sketch{
    public class TexturedCubicBlock : CubicBlock{
        protected Area2d _textureArea;

        public TexturedCubicBlock(string id, Chunk chunk, Vector3i position, Area2d textureArea) 
            : base(id, chunk, position) {
            _textureArea = textureArea;
        }

        public Area2d TextureArea => _textureArea;

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new TexturedCubicBlock(_id, _chunk, _position, _textureArea);
        }

        public override Area2d GetTextureArea(BlockFace face) {
            return _textureArea;
        }
    }
}