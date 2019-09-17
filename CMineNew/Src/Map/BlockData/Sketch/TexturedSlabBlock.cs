using CMineNew.Geometry;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Sketch{
    public class TexturedSlabBlock : SlabBlock{
        protected Area2d _textureArea;

        public TexturedSlabBlock(string id, Chunk chunk, Vector3i position, Area2d texture, Color4 textureFilter,
            bool upside, bool passable = false , bool lightSource = false, int sourceLight = 0, int blockLightPassReduction = 1,
            int sunlightPassReduction = 0)
            : base(id, chunk, position, textureFilter, upside, passable, lightSource, sourceLight, blockLightPassReduction, sunlightPassReduction) {
            _textureArea = texture;
        }

        public TexturedSlabBlock(string id, Chunk chunk, Vector3i position, string texture, Color4 textureFilter,
            bool upside, bool passable = false, bool lightSource = false, int sourceLight = 0, int blockLightPassReduction = 1,
            int sunlightPassReduction = 0)
            : base(id, chunk, position, textureFilter, upside, passable, lightSource, sourceLight, blockLightPassReduction, sunlightPassReduction) {
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