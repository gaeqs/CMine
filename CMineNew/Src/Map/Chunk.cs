using CMineNew.Geometry;
using CMineNew.Map.BlockData;

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
            var old = _blocks[chunkPosition.X, chunkPosition.Y, chunkPosition.Z];
            _blocks[chunkPosition.X, chunkPosition.Y, chunkPosition.Z] = block;
            var position = (_position << WorldPositionShift) + chunkPosition;
            if (block != null) {
                block.Chunk = this;
                block.Position = position;
            }

            var neighbours = GetNeighbourBlocks(position, chunkPosition);

            old?.OnRemove(block);
            block?.OnPlace0(old, neighbours);
            for (var i = 0; i < neighbours.Length; i++) {
                neighbours[i]?.OnNeighbourBlockChange0(old, block,
                    BlockFaceMethods.GetOpposite((BlockFace) i));
            }
        }

        public void SetBlockFromWorldPosition(Block block, Vector3i worldPosition) {
            SetBlock(block, worldPosition - (_position << WorldPositionShift));
        }

        private Block[] GetNeighbourBlocks(Vector3i position, Vector3i chunkPosition) {
            var blocks = new Block[6];
            //East block
            blocks[(int) BlockFace.East] = chunkPosition.X == 15
                ? World.GetBlock(position + new Vector3i(1, 0, 0))
                : _blocks[chunkPosition.X + 1, chunkPosition.Y, chunkPosition.Z];
            //West block
            blocks[(int) BlockFace.West] = chunkPosition.X == 0
                ? World.GetBlock(position - new Vector3i(1, 0, 0))
                : _blocks[chunkPosition.X - 1, chunkPosition.Y, chunkPosition.Z];
            //South
            blocks[(int) BlockFace.South] = chunkPosition.Z == 15
                ? World.GetBlock(position + new Vector3i(0, 0, 1))
                : _blocks[chunkPosition.X, chunkPosition.Y, chunkPosition.Z + 1];
            //North
            blocks[(int) BlockFace.North] = chunkPosition.Z == 0
                ? World.GetBlock(position - new Vector3i(0, 0, 1))
                : _blocks[chunkPosition.X, chunkPosition.Y, chunkPosition.Z - 1];
            //Up
            blocks[(int) BlockFace.Up] = chunkPosition.Y == 15
                ? World.GetBlock(position + new Vector3i(0, 1, 0))
                : _blocks[chunkPosition.X, chunkPosition.Y + 1, chunkPosition.Z];
            //Down
            blocks[(int) BlockFace.Down] = chunkPosition.Y == 0
                ? World.GetBlock(position - new Vector3i(0, 1, 0))
                : _blocks[chunkPosition.X, chunkPosition.Y - 1, chunkPosition.Z];
            return blocks;
        }
    }
}