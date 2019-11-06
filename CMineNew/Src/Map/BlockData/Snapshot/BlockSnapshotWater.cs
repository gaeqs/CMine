using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using CMineNew.Map.BlockData.Static.Type;
using CMineNew.Map.BlockData.Type;

namespace CMineNew.Map.BlockData.Snapshot {
    public class BlockSnapshotWater : BlockSnapshot {
        private int _waterLevel;

        public BlockSnapshotWater(int waterLevel) : base("default:water", BlockStaticDataWater.Instance) {
            _waterLevel = waterLevel;
        }

        public int WaterLevel {
            get => _waterLevel;
            set => _waterLevel = value;
        }

        public override BlockModel BlockModel => BlockModelManager.GetModelOrNull(WaterBlockModel.Key);
        public override bool Passable => true;

        public override Block ToBlock(Chunk chunk, Vector3i position) {
            return new BlockWater(chunk, position, _waterLevel);
        }

        public override bool CanBePlaced(Vector3i position, World world) {
            return true;
        }

        public override BlockSnapshot Clone() {
            return new BlockSnapshotWater(_waterLevel);
        }

        public override void Save(Stream stream, BinaryFormatter formatter) {
            formatter.Serialize(stream, _waterLevel);
        }

        public override void Load(Stream stream, BinaryFormatter formatter) {
            _waterLevel = (int) formatter.Deserialize(stream);
        }
    }
}