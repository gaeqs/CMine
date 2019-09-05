using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Type{
    public class BlockGrass : MultiTexturedCubicBlock{
        public BlockGrass(Chunk chunk, Vector3i position)
            : base("default:grass", chunk, position,
                new[] {
                    "default:grass_top", "default:dirt", "default:grass_side",
                    "default:grass_side", "default:grass_side", "default:grass_side"
                }, Color4.Green) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockGrass(chunk, position);
        }
    }
}