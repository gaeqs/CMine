using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;
using CMineNew.Map.BlockData.Static.Type;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Type{
    public class BlockBricks : TexturedCubicBlock{
        public BlockBricks(Chunk chunk, Vector3i position)
            : base(BlockStaticDataBricks.Instance, chunk, position, Color4.Transparent) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockBricks(chunk, position);
        }
    }
}