using CMineNew.Color;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Static;

namespace CMineNew.Map.BlockData.Sketch{
    public class TexturedCrossBlock : CrossBlock{
        public TexturedCrossBlock(BlockStaticDataTexturedCross staticData, Chunk chunk, Vector3i position,
            Rgba32I textureFilter)
            : base(staticData, chunk, position, textureFilter) {
        }

        public override int TextureIndex => ((BlockStaticDataTexturedCross) _staticData).TextureIndex;

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new TexturedCrossBlock((BlockStaticDataTexturedCross) _staticData, _chunk, _position,
                _textureFilter);
        }
    }
}