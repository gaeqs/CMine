using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Type{
    public class BlockDirt : TexturedCubicBlock{
        public BlockDirt(Chunk chunk, Vector3i position)
            : base("default:dirt", chunk, position, "default:dirt", Color4.Transparent,
                false, false, 0, MaxBlockLight, MaxBlockLight) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockDirt(chunk, position);
        }
    }
}