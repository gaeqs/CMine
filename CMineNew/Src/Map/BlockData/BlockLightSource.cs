using System.Collections.ObjectModel;
using CMineNew.Geometry;

namespace CMineNew.Map.BlockData{
    public class BlockLightSource{

        private readonly Block _block;
        private readonly sbyte _sourceLight;
        private readonly Collection<BlockLight> _blocks;

        public BlockLightSource(Block block, sbyte sourceLight) {
            _block = block;
            _sourceLight = sourceLight;
            _blocks = new Collection<BlockLight>();
        }

        public Block Block => _block;

        public sbyte SourceLight => _sourceLight;
        
        public Collection<BlockLight> Blocks => _blocks;

        protected bool Equals(BlockLightSource other) {
            return _block.Position.Equals(other.Block.Position);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BlockLightSource) obj);
        }

        public override int GetHashCode() {
            return _block.Position.GetHashCode();
        }
    }
}