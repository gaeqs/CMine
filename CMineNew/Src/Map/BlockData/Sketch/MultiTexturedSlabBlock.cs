using CMineNew.Color;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Static;

namespace CMineNew.Map.BlockData.Sketch{
    public class MultiTexturedSlabBlock : SlabBlock{
        public MultiTexturedSlabBlock(BlockStaticDataMultiTexturedSlab staticData, Chunk chunk, Vector3i position,
            Rgba32I textureFilter, bool upside)
            : base(staticData, chunk, position, textureFilter, upside) {
        }

        public Area2d[] TextureAreas => ((BlockStaticDataMultiTexturedSlab) _staticData).TextureAreas;

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new MultiTexturedSlabBlock((BlockStaticDataMultiTexturedSlab) _staticData, _chunk, _position,
                _textureFilter, _upside);
        }

        public override Area2d GetTextureArea(BlockFace face) {
            return TextureAreas[(int) face];
        }
    }
}