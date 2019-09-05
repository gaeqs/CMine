using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Type{
    public class BlockOakLeaves : TexturedCubicBlock{
        public BlockOakLeaves(Chunk chunk, Vector3i position)
            : base("default:oak_leaves", chunk, position, "default:oak_leaves", Color4.Green) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockOakLog(chunk, position);
        }

        public override bool IsFaceOpaque(BlockFace face) {
            return false;
        }
        
        public override void Load(Stream stream, BinaryFormatter formatter, uint version, World2dRegion region2d) {
            base.Load(stream, formatter, version, region2d);
            var position = _position >> World2dRegion.WorldPositionShift;
            _textureFilter = region2d.InterpolatedGrassColors[position.X, position.Z];
        }
    }
}