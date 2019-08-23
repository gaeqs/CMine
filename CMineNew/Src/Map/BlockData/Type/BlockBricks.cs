using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Type{
    public class BlockBricks : TexturedCubicBlock{
        public BlockBricks(Chunk chunk, Vector3i position)
            : base("default:bricks", chunk, position, "default:bricks", Color4.Transparent) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockBricks(chunk, position);
        }
    }
}