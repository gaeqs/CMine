using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;
using CMineNew.Map.BlockData.Static.Type;
using CMineNew.Texture;

namespace CMineNew.Map.BlockData.Type{
    public class BlockSand : TexturedCubicBlock{
        public BlockSand(Chunk chunk, Vector3i position)
            : base(BlockStaticDataSand.Instance, chunk, position, new Rgba32I(0, 0, 0, 0)) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockSand(chunk, position);
        }
    }
}