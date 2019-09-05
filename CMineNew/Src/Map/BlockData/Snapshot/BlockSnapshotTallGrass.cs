using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using CMineNew.Map.BlockData.Snapshot.Interface;
using CMineNew.Map.BlockData.Type;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Snapshot{
    public class BlockSnapshotTallGrass : BlockSnapshot, IGrass{
        private Color4 _grassColor;

        public BlockSnapshotTallGrass(Color4 grassColor) : base("default:tall_grass") {
            _grassColor = grassColor;
        }

        public override BlockModel BlockModel => BlockModelManager.GetModelOrNull(CrossBlockModel.Key);
        public override bool Passable => true;

        public override Block ToBlock(Chunk chunk, Vector3i position) {
            return new BlockTallGrass(chunk, position) {TextureFilter = _grassColor};
        }

        public override bool CanBePlaced(Vector3i position, World world) {
            var block = world.GetBlock(position + new Vector3i(0, -1, 0));
            return block is BlockGrass;
        }

        public override BlockSnapshot Clone() {
            return new BlockSnapshotTallGrass(_grassColor);
        }

        public Color4 GrassColor {
            get => _grassColor;
            set => _grassColor = value;
        }
        
        public override void Save(Stream stream, BinaryFormatter formatter) {
            formatter.Serialize(stream, _grassColor.R);
            formatter.Serialize(stream, _grassColor.G);
            formatter.Serialize(stream, _grassColor.B);
            formatter.Serialize(stream, _grassColor.A);
        }

        public override void Load(Stream stream, BinaryFormatter formatter) {
            var r = (float) formatter.Deserialize(stream);
            var g = (float) formatter.Deserialize(stream);
            var b = (float) formatter.Deserialize(stream);
            var a = (float) formatter.Deserialize(stream);
            _grassColor = new Color4(r, g, b, a);
        }
    }
}