using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;
using CMineNew.Map.BlockData.Static.Type;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Type{
    public class BlockDirt : TexturedCubicBlock{
        public BlockDirt(Chunk chunk, Vector3i position)
            : base(BlockStaticDataDirt.Instance, chunk, position, Color4.Transparent) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockDirt(chunk, position);
        }
    }
}