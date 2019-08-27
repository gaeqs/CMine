using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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

        private byte[] _saveBuffer;
        private bool _modified, _natural;

        public Chunk(ChunkRegion region, Vector3i position) {
            _region = region;
            _position = position;
            _blocks = new Block[ChunkLength, ChunkLength, ChunkLength];
            _taskManager = new WorldTaskManager();
            _saveBuffer = null;
            _modified = true;
            _natural = false;
        }

        public World World => _region.World;

        public ChunkRegion Region => _region;

        public Vector3i Position {
            get => _position;
            set => _position = value;
        }

        public bool Modified {
            get => _modified;
            set => _modified = value;
        }

        public bool Natural {
            get => _natural;
            set => _natural = value;
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
            block?.OnPlace0(old, neighbours, true);
            for (var i = 0; i < neighbours.Length; i++) {
                neighbours[i]?.OnNeighbourBlockChange0(old, block,
                    BlockFaceMethods.GetOpposite((BlockFace) i));
            }

            _modified = true;
            _natural = false;
            return block;
        }

        public Block SetBlockFromWorldPosition(BlockSnapshot snapshot, Vector3i worldPosition) {
            return SetBlock(snapshot, worldPosition - (_position << WorldPositionShift));
        }

        public void FillWithBlocks(BlockSnapshot[,,] snapshots, bool empty) {
            var pos = _position << WorldPositionShift;
            ForEachChunkPosition((x, y, z) => {
                var snapshot = snapshots[x, y, z];
                var blockPos = pos + new Vector3i(x, y, z);
                var block = snapshot == null ? new BlockAir(this, blockPos) : snapshot.ToBlock(this, blockPos);
                _blocks[x, y, z] = block;
            });

            if (empty) return;
            SendOnPlaceEventToAllBlocks(false);
            _modified = true;
        }

        public void SendOnPlaceEventToAllBlocks(bool triggerWorldUpdates) {
            var blocks = new Block[6];
            ForEachChunkPosition((x, y, z, block) => {
                if (block == null || block is BlockAir) return;
                GetNeighbourBlocks(blocks, block.Position, new Vector3i(x, y, z));
                block.OnPlace0(null, blocks, triggerWorldUpdates);
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
            });
        }

        public void RemoveAllBlockFacesFromRender() {
            if (Region.Deleted) return;
            ForEachChunkPosition((x, y, z, block) => block?.RemoveFromRender());
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

        public void RefreshEdgeBlocks() {
            var blocks = new Block[6];
            for (var x = 0; x < ChunkLength; x++) {
                var edgeX = x == 0 || x == 15;
                for (var y = 0; y < ChunkLength; y++) {
                    var edgeY = y == 0 || y == 15;
                    for (var z = 0; z < ChunkLength; z++) {
                        if (edgeX || edgeY || z == 0 || z == 15) {
                            var block = _blocks[x, y, z];
                            if (block == null || block is BlockAir) continue;
                            block.OnPlace0(null,
                                GetNeighbourBlocks(blocks, block.Position,
                                    new Vector3i(x, y, z)), false);
                        }
                    }
                }
            }
        }

        public void Save(Stream stream, BinaryFormatter formatter) {
            if (_modified) {
                GenerateSaveBuffer(formatter);
                _modified = false;
            }

            stream.Write(_saveBuffer, 0, _saveBuffer.Length);
        }

        private void GenerateSaveBuffer(BinaryFormatter formatter) {
            var buffer = new MemoryStream();
            ForEachChunkPosition((x, y, z, block) => {
                var empty = block == null || block is BlockAir;
                buffer.WriteByte(empty ? (byte) 0 : (byte) 1);
                if (empty) return;
                formatter.Serialize(buffer, block.Id);
                block.Save(buffer, formatter);
            });
            _saveBuffer = buffer.ToArray();
        }

        public void Load(Stream stream, BinaryFormatter formatter, uint version) {
            var pos = _position << WorldPositionShift;
            ForEachChunkPosition((x, y, z) => {
                var blockPos = pos + new Vector3i(x, y, z);
                if (stream.ReadByte() == 0) {
                    _blocks[x, y, z] = new BlockAir(this, blockPos);
                    return;
                }

                var id = (string) formatter.Deserialize(stream);
                if (!BlockManager.TryGet(id, out var snapshot))
                    throw
                        new System.Exception("Couldn't load chunk " + _position + ". Block Id missing.");
                var block = snapshot.ToBlock(this, blockPos);
                _blocks[x, y, z] = block;
                block.Load(stream, formatter, version);
            });
            GenerateSaveBuffer(formatter);
            _modified = false;
        }

        private static void ForEachChunkPosition(Action<int, int, int> action) {
            for (var x = 0; x < ChunkLength; x++) {
                for (var y = 0; y < ChunkLength; y++) {
                    for (var z = 0; z < ChunkLength; z++) {
                        action.Invoke(x, y, z);
                    }
                }
            }
        }

        private void ForEachChunkPosition(Action<int, int, int, Block> action) {
            for (var x = 0; x < ChunkLength; x++) {
                for (var y = 0; y < ChunkLength; y++) {
                    for (var z = 0; z < ChunkLength; z++) {
                        action.Invoke(x, y, z, _blocks[x, y, z]);
                    }
                }
            }
        }
    }
}