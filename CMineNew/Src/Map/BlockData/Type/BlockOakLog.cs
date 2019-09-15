using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Type{
    public class BlockOakLog : MultiTexturedCubicBlock{
        public BlockOakLog(Chunk chunk, Vector3i position)
            : base("default:oak_log", chunk, position,
                new[] {
                    "default:oak_log_top", "default:oak_log_top", "default:oak_log_side",
                    "default:oak_log_side", "default:oak_log_side", "default:oak_log_side"
                }, Color4.Green,
                false, false, 0, MaxBlockLight, MaxBlockLight) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockOakLog(chunk, position);
        }
    }
}