using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Type{
    public class BlockBricksSlab : TexturedSlabBlock{
        public BlockBricksSlab(Chunk chunk, Vector3i position)
            : base("default:bricks_slab", chunk, position, "default:bricks", Color4.Transparent) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockBricksSlab(chunk, position);
        }
    }
}