using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;
using CMineNew.Map.BlockData.Static.Type;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Type{
    public class BlockGrass : MultiTexturedCubicBlock{
        public BlockGrass(Chunk chunk, Vector3i position)
            : base(BlockStaticDataGrass.Instance, chunk, position, Color4.Green) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockGrass(chunk, position);
        }
    }
}