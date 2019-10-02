using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;
using CMineNew.Map.BlockData.Static.Type;
using CMineNew.Texture;

namespace CMineNew.Map.BlockData.Type{
    public class BlockTallGrass : TexturedCrossBlock{
        public BlockTallGrass(Chunk chunk, Vector3i position)
            : base(BlockStaticDataTallGrass.Instance, chunk, position, new Rgba32I(0, 255, 0, 255)) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockTallGrass(chunk, position);
        }
    }
}