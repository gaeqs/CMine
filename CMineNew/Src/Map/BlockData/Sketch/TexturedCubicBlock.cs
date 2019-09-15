using CMineNew.Geometry;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Sketch{
    public class TexturedCubicBlock : CubicBlock{
        protected Area2d _textureArea;

        public TexturedCubicBlock(string id, Chunk chunk, Vector3i position, Area2d texture, Color4 textureFilter,
            bool passable = false, bool lightSource = false, int blockLight = 0,
            int blockLightPassReduction = 1, int sunlightPassReduction = 0)
            : base(id, chunk, position, textureFilter, passable, lightSource, blockLight, blockLightPassReduction,
                sunlightPassReduction) {
            _textureArea = texture;
        }

        public TexturedCubicBlock(string id, Chunk chunk, Vector3i position, string texture, Color4 textureFilter,
            bool passable = false, bool lightSource = false, int blockLight = 0, int blockLightPassReduction = 1,
            int sunlightPassReduction = 0)
            : base(id, chunk, position, textureFilter, passable, lightSource, blockLight, blockLightPassReduction,
                sunlightPassReduction) {
            _textureArea = CMine.Textures.Areas[texture];
        }

        public Area2d TextureArea => _textureArea;

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new TexturedCubicBlock(_id, _chunk, _position, _textureArea, _textureFilter);
        }

        public override Area2d GetTextureArea(BlockFace face) {
            return _textureArea;
        }
    }
}