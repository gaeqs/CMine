using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;
using CMineNew.Map.BlockData.Static.Type;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Type{
    public class BlockSand : TexturedCubicBlock{
        public BlockSand(Chunk chunk, Vector3i position)
            : base(BlockStaticDataSand.Instance, chunk, position, Color4.Transparent) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockSand(chunk, position);
        }
    }
}