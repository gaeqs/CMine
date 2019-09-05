using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using CMineNew.Map.BlockData.Type;

namespace CMineNew.Map.BlockData.Snapshot{
    public class BlockSnapshotBricksSlab : BlockSnapshot{
        private bool _upside;

        public BlockSnapshotBricksSlab(bool upside) : base("default:bricks_slab") {
            _upside = upside;
        }

        public bool Upside {
            get => _upside;
            set => _upside = value;
        }

        public override BlockModel BlockModel => BlockModelManager.GetModelOrNull(SlabBlockModel.Key);
        public override bool Passable => false;

        public override Block ToBlock(Chunk chunk, Vector3i position) {
            return new BlockBricksSlab(chunk, position, _upside);
        }

        public override bool CanBePlaced(Vector3i position, World world) {
            return true;
        }

        public override BlockSnapshot Clone() {
            return new BlockSnapshotBricksSlab(_upside);
        }

        public override void Save(Stream stream, BinaryFormatter formatter) {
            formatter.Serialize(stream, _upside);
        }

        public override void Load(Stream stream, BinaryFormatter formatter) {
            _upside = (bool) formatter.Deserialize(stream);
        }
    }
}