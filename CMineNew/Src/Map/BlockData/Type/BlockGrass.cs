using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
        
        public override void Load(Stream stream, BinaryFormatter formatter, uint version, World2dRegion region2d) {
            base.Load(stream, formatter, version, region2d);
            var position = _position >> World2dRegion.WorldPositionShift;
            _textureFilter = region2d.InterpolatedGrassColors[position.X, position.Z];
        }
    }
}