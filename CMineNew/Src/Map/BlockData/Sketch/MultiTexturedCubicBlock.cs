using CMineNew.Color;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Static;

namespace CMineNew.Map.BlockData.Sketch{
    public class MultiTexturedCubicBlock : CubicBlock{
        public MultiTexturedCubicBlock(BlockStaticDataMultiTexturedCubic staticData, Chunk chunk, Vector3i position,
            Rgba32I textureFilter)
            : base(staticData, chunk, position, textureFilter) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new MultiTexturedCubicBlock((BlockStaticDataMultiTexturedCubic) _staticData, _chunk, _position,
                _textureFilter);
        }

        public override Area2d GetTextureArea(BlockFace face) {
            return _staticData.GetTextureArea(face);
        }
    }
}