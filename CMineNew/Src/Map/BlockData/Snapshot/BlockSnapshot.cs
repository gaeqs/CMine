using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;

namespace CMineNew.Map.BlockData.Snapshot{
    public abstract class BlockSnapshot{
        private readonly string _id;

        public BlockSnapshot(string id) {
            _id = id;
        }

        public string Id => _id;

        public abstract BlockModel BlockModel { get; }

        public abstract bool Passable { get; }

        public abstract Block ToBlock(Chunk chunk, Vector3i position);

        public abstract bool CanBePlaced(Vector3i position, World world);

        public abstract BlockSnapshot Clone();

        public virtual void Save(Stream stream, BinaryFormatter formatter) {
        }

        public virtual void Load(Stream stream, BinaryFormatter formatter) {
        }
    }
}