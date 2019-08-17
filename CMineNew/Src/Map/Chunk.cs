using CMineNew.Geometry;

namespace CMineNew.Map{
    public class Chunk{
        public const int ChunkLength = 16;
        public const int ChunkSize = ChunkLength * ChunkLength;
        public const int ChunkVolume = ChunkLength * ChunkLength * ChunkLength;
        public const int WorldPositionShift = 4;

        private readonly ChunkRegion _region;
        private Vector3i _position;
        private readonly Block[,,] _blocks;

        public Chunk(ChunkRegion region, Vector3i position) {
            _region = region;
            _position = position;
            _blocks = new Block[ChunkLength, ChunkLength, ChunkLength];
        }

        public World World => _region.World;

        public ChunkRegion Region => _region;

        public Vector3i Position {
            get => _position;
            set => _position = value;
        }

        public Block[,,] Blocks => _blocks;

        public Block GetBlock(Vector3i chunkPosition) {
            return _blocks[chunkPosition.X, chunkPosition.Y, chunkPosition.Z];
        }

        public Block GetBlockFromWorldPosition(Vector3i worldPosition) {
            return GetBlock(worldPosition - (_position << WorldPositionShift));
        }

        public void SetBlock(Block block, Vector3i chunkPosition) {
            _blocks[chunkPosition.X, chunkPosition.Y, chunkPosition.Z] = block;
            block.Chunk = this;
            block.Position = (_position << WorldPositionShift) + chunkPosition;
            //block.UpdateVisibleFaces(true);
        }

        public void SetBlockFromWorldPosition(Block block, Vector3i worldPosition) {
            var chunkPosition = worldPosition - (_position << WorldPositionShift);
            _blocks[chunkPosition.X, chunkPosition.Y, chunkPosition.Z] = block;
            block.Chunk = this;
            block.Position = worldPosition;
            //block.UpdateVisibleFaces(true);
        }
    }
}