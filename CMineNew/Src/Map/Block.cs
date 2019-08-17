using CMineNew.Geometry;
using OpenTK;

namespace CMineNew.Map{
    public class Block{
        protected readonly string _id;

        protected Chunk _chunk;
        protected Vector3i _position;

        public Block(string id, Chunk chunk, Vector3i position) {
            _id = id;
            _chunk = chunk;
            _position = position;
        }

        public string Id => _id;

        public World World => _chunk.World;

        public Chunk Chunk {
            get => _chunk;
            set => _chunk = value;
        }

        public Vector3i Position {
            get => _position;
            set => _position = value;
        }

        public virtual void OnPlace() {
        }

        public virtual void OnRemove() {
        }

        public virtual void OnNeighbourBlockChange(Block from, Block to, BlockFace relative) {
        }

        public virtual Block Clone(Chunk chunk, Vector3i position) {
            return new Block(_id, chunk, position);
        }

        public virtual bool Collides(Vector3 origin, Vector3 direction) {
            return true;
        }
    }
}