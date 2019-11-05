using CMineNew.Color;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;
using CMineNew.Map.BlockData.Static.Type;

namespace CMineNew.Map.BlockData.Type{
    public class BlockBricksSlab : TexturedSlabBlock{
        public BlockBricksSlab(Chunk chunk, Vector3i position, bool upside)
            : base(BlockStaticDataBricksSlab.Instance, chunk, position, new Rgba32I(0, 0, 0, 0), upside) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockBricksSlab(chunk, position, _upside);
        }
    }
}