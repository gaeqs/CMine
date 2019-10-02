using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;
using CMineNew.Map.BlockData.Static.Type;
using CMineNew.Texture;

namespace CMineNew.Map.BlockData.Type{
    public class BlockBricks : TexturedCubicBlock{
        public BlockBricks(Chunk chunk, Vector3i position)
            : base(BlockStaticDataBricks.Instance, chunk, position, new Rgba32I(0, 0, 0, 0)) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockBricks(chunk, position);
        }
    }
}