using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;
using CMineNew.Map.BlockData.Static.Type;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Type{
    public class BlockOakLog : MultiTexturedCubicBlock{
        public BlockOakLog(Chunk chunk, Vector3i position)
            : base(BlockStaticDataOakLog.Instance, chunk, position, Color4.Transparent) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockOakLog(chunk, position);
        }
    }
}