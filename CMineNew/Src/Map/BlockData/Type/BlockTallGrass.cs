using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;
using CMineNew.Map.BlockData.Static.Type;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Type{
    public class BlockTallGrass : TexturedCrossBlock{
        public BlockTallGrass(Chunk chunk, Vector3i position)
            : base(BlockStaticDataTallGrass.Instance, chunk, position, Color4.Green) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockTallGrass(chunk, position);
        }
    }
}