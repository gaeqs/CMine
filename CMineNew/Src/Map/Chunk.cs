using CMineNew.Geometry;
using CMineNew.Map.BlockData;
using CMineNew.Map.BlockData.Snapshot;
using CMineNew.Map.BlockData.Type;
using CMineNew.Map.Task;

namespace CMineNew.Map{
    public class Chunk{
        public const int ChunkLength = 16;
        public const int ChunkSize = ChunkLength * ChunkLength;
        public const int ChunkVolume = ChunkLength * ChunkLength * ChunkLength;
        public const int WorldPositionShift = 4;

        private readonly ChunkRegion _region;
        private Vector3i _position;
        private readonly Block[,,] _blocks;
        private WorldTaskManager _taskManager;

        public Chunk(ChunkRegion region, Vector3i position) {
            _region = region;
            _position = position;
            _blocks = new Block[ChunkLength, ChunkLength, ChunkLength];
            _taskManager = new WorldTaskManager();
        }

        public World World => _region.World;

        public ChunkRegion Region => _region;

        public Vector3i Position {
            get => _position;
            set => _position = value;
        }

        public Block[,,] Blocks => _blocks;

        public WorldTaskManager TaskManager => _taskManager;

        public Block GetBlock(Vector3i chunkPosition) {
            return _blocks[chunkPosition.X, chunkPosition.Y, chunkPosition.Z];
        }

        public Block GetBlockFromWorldPosition(Vector3i worldPosition) {
            return GetBlock(worldPosition - (_position << WorldPositionShift));
        }

        public Block SetBlock(BlockSnapshot snapshot, Vector3i chunkPosition) {
            var position = (_position << WorldPositionShift) + chunkPosition;
            var block = snapshot.ToBlock(this, position);
            var old = _blocks[chunkPosition.X, chunkPosition.Y, chunkPosition.Z];
            _blocks[chunkPosition.X, chunkPosition.Y, chunkPosition.Z] = block;

            var neighbours = GetNeighbourBlocks(new Block[6], position, chunkPosition);

            old?.OnRemove(block);
            block?.OnPlace0(old, neighbours);
            for (var i = 0; i < neighbours.Length; i++) {
                neighbours[i]?.OnNeighbourBlockChange0(old, block,
                    BlockFaceMethods.GetOpposite((BlockFace) i));
            }

            return block;
        }

        public Block SetBlockFromWorldPosition(BlockSnapshot snapshot, Vector3i worldPosition) {
            return SetBlock(snapshot, worldPosition - (_position << WorldPositionShift));
        }

        public void FillWithBlocks(BlockSnapshot[,,] snapshots, bool empty) {
            var pos = _position << WorldPositionShift;
            for (var x = 0; x < ChunkLength; x++) {
                for (var y = 0; y < ChunkLength; y++) {
                    for (var z = 0; z < ChunkLength; z++) {
                        var snapshot = snapshots[x, y, z];
                        var blockPos = pos + new Vector3i(x, y, z);
                        var block = snapshot == null ? new BlockAir(this, blockPos) : snapshot.ToBlock(this, blockPos);
                        _blocks[x, y, z] = block;
                    }
                }
            }

            if (empty) return;
            var blocks = new Block[6];
            for (var x = 0; x < ChunkLength; x++) {
                for (var y = 0; y < ChunkLength; y++) {
                    for (var z = 0; z < ChunkLength; z++) {
                        var block = _blocks[x, y, z];
                        GetNeighbourBlocks(blocks, block.Position, new Vector3i(x, y, z));
                        block.OnPlace0(null, blocks);
                        if (x == 0) {
                            blocks[(int) BlockFace.West]?.OnNeighbourBlockChange0(null, block, BlockFace.East);
                        }
                        else if (x == 15) {
                            blocks[(int) BlockFace.East]?.OnNeighbourBlockChange0(null, block, BlockFace.West);
                        }

                        if (y == 0) {
                            blocks[(int) BlockFace.Down]?.OnNeighbourBlockChange0(null, block, BlockFace.Up);
                        }
                        else if (y == 15) {
                            blocks[(int) BlockFace.Up]?.OnNeighbourBlockChange0(null, block, BlockFace.Down);
                        }

                        if (z == 0) {
                            blocks[(int) BlockFace.North]?.OnNeighbourBlockChange0(null, block, BlockFace.South);
                        }
                        else if (z == 15) {
                            blocks[(int) BlockFace.South]?.OnNeighbourBlockChange0(null, block, BlockFace.North);
                        }
                    }
                }
            }
        }

        public void RemoveAllBlockFacesFromRender() {
            if (Region.Deleted) return;
            for (var x = 0; x < 16; x++) {
                for (var y = 0; y < 16; y++) {
                    for (var z = 0; z < 16; z++) {
                        _blocks[x, y, z]?.RemoveFromRender();
                    }
                }
            }
        }

        public Block[] GetNeighbourBlocks(Block[] blocks, Vector3i position, Vector3i chunkPosition) {
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