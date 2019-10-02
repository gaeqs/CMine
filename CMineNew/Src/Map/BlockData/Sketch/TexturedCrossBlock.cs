using CMineNew.Geometry;
using CMineNew.Map.BlockData.Static;
using CMineNew.Texture;

namespace CMineNew.Map.BlockData.Sketch{
    public class TexturedCrossBlock : CrossBlock{
        public TexturedCrossBlock(BlockStaticDataTexturedCross staticData, Chunk chunk, Vector3i position,
            Rgba32I textureFilter)
            : base(staticData, chunk, position, textureFilter) {
        }

        public override Area2d TextureArea => ((BlockStaticDataTexturedCross) _staticData).TextureArea;

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new TexturedCrossBlock((BlockStaticDataTexturedCross) _staticData, _chunk, _position,
                _textureFilter);
        }
    }
}