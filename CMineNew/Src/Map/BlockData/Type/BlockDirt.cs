using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;

namespace CMineNew.Map.BlockData.Type{
    public class BlockDirt : TexturedCubicBlock{
        public BlockDirt(Chunk chunk, Vector3i position)
            : base("default:dirt", chunk, position, "default:dirt") {
        }
    }
}