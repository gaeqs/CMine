using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Sketch;
using CMineNew.Map.BlockData.Snapshot;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Type{
    public class BlockTallGrass : TexturedCrossBlock{
        public BlockTallGrass(Chunk chunk, Vector3i position)
            : base("default:tall_grass", chunk, position, "default:tall_grass", Color4.Green, true) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockTallGrass(chunk, position);
        }

        public override void Load(Stream stream, BinaryFormatter formatter, uint version, World2dRegion region2d) {
            base.Load(stream, formatter, version, region2d);
            var position = _position >> World2dRegion.WorldPositionShift;
            _textureFilter = region2d.InterpolatedGrassColors[position.X, position.Z];
        }
    }
}