using CMineNew.Color;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Static;

namespace CMineNew.Map.BlockData.Sketch{
    public class TexturedSlabBlock : SlabBlock{
        public TexturedSlabBlock(BlockStaticDataTexturedSlab staticData, Chunk chunk, Vector3i position,
            Rgba32I textureFilter, bool upside)
            : base(staticData, chunk, position, textureFilter, upside) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new TexturedSlabBlock((BlockStaticDataTexturedSlab) _staticData, _chunk, _position, _textureFilter,
                _upside);
        }

        public override int GetTextureIndex(BlockFace face) {
            return _staticData.GetTextureIndex(face);
            ;
        }
    }
}