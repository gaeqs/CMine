using CMineNew.Color;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;
using CMineNew.Map.BlockData.Static.Type;

namespace CMineNew.Map.BlockData.Type{
    public class BlockGrass : MultiTexturedCubicBlock{
        public BlockGrass(Chunk chunk, Vector3i position)
            : base(BlockStaticDataGrass.Instance, chunk, position, new Rgba32I(0, 255, 0, 255)) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockGrass(chunk, position);
        }
    }
}