using CMineNew.Color;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Static;

namespace CMineNew.Map.BlockData.Sketch{
    public class TexturedCubicBlock : CubicBlock{
        public TexturedCubicBlock(BlockStaticDataTexturedCubic staticData, Chunk chunk, Vector3i position,
            Rgba32I textureFilter)
            : base(staticData, chunk, position, textureFilter) {
        }

        public Area2d TextureArea => ((BlockStaticDataTexturedCubic) _staticData).TextureArea;

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new TexturedCubicBlock((BlockStaticDataTexturedCubic) _staticData, _chunk, _position,
                _textureFilter);
        }

        public override Area2d GetTextureArea(BlockFace face) {
            return TextureArea;
        }
    }
}