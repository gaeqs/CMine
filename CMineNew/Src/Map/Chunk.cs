using System;
using System.Collections.Generic;
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
            return SetBlock(snapshot, chunkPosition, f => true);
        }

        public Block SetBlock(BlockSnapshot snapshot, Vector3i chunkPosition, Func<Block, bool> canBeReplaced) {
            var old = _blocks[chunkPosition.X, chunkPosition.Y, chunkPosition.Z];
            if (!canBeReplaced.Invoke(old)) return null;

            var position = (_position << WorldPositionShift) + chunkPosition;
            var block = snapshot.ToBlock(this, position);

            _blocks[chunkPosition.X, chunkPosition.Y, chunkPosition.Z] = block;

            var neighbours = old == null ? GetNeighbourBlocks(new Block[6], position, chunkPosition) : old.Neighbours;
            if (block != null) {
                block.Neighbours = neighbours;
            }

            for (var i = 0; i < neighbours.Length; i++) {
                neighbours[i]?.OnNeighbourBlockChange0(old, block,
                    BlockFaceMethods.GetOpposite((BlockFace) i));
            }

            if (old != null) {
                old.OnRemove0(block, out var blockList, out var sunList);
                block?.OnPlace0(old, true, true);
                blockList?.Remove(old);
                blockList?.Add(block);
                sunList?.Remove(old);
                sunList?.Add(block);
                var queue = new Queue<Block>();
                BlockLightMethods.ExpandNearbyLights(blockList, queue);
                SunlightMethods.ExpandNearbyLights(sunList, queue);
                while (queue.Count > 0) {
                    queue.Dequeue().TriggerLightChange();
                }
            }
            else {
                block?.OnPlace0(old, true, true);
            }

            _modified = true;
            _natural = false;
            return block;
        }

        public Block SetBlockFromWorldPosition(BlockSnapshot snapshot, Vector3i worldPosition) {
            return SetBlock(snapshot, worldPosition - (_position << WorldPositionShift));
        }

        public Block SetBlockFromWorldPosition(BlockSnapshot snapshot, Vector3i worldPosition,
            Func<Block, bool> canBeReplaced) {
            return SetBlock(snapshot, worldPosition - (_position << WorldPositionShift), canBeReplaced);
        }

        public void FillWithBlocks(BlockSnapshot[,,] snapshots) {
            var pos = _position << WorldPositionShift;
            ForEachChunkPosition((x, y, z) => {
                var snapshot = snapshots[x, y, z];
                var blockPos = pos + new Vector3i(x, y, z);
                var block = snapshot == null ? new BlockAir(this, blockPos) : snapshot.ToBlock(this, blockPos);
                _blocks[x, y, z] = block;
            });
            
            SendOnPlaceEventToAllBlocks(true, false);
            ForEachChunkPosition((x, y, z, block) => block.ExpandSunlight());
            _modified = true;
        }

        public void SendOnPlaceEventToAllBlocks(bool triggerWorldUpdates, bool expandSunlight) {
            var blocks = new Block[6];
            ForEachChunkPosition((x, y, z, block) => {
                GetNeighbourBlocks(blocks, block.Position, new Vector3i(x, y, z));
                block.Neighbours = blocks;
            });
            ForEachChunkPosition((x, y, z, block) => {
                if (block == null) return;
                block.OnPlace0(null, triggerWorldUpdates, expandSunlight);
                blocks = block.Neighbours;
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
                var empty = block == null;
                buffer.WriteByte(empty ? (byte) 0 : (byte) 1);
                if (empty) return;
                formatter.Serialize(buffer, block.Id);
                block.Save(buffer, formatter);
            });
            _saveBuffer = buffer.ToArray();
        }

        public void Load(Stream stream, BinaryFormatter formatter, uint version, World2dRegion region2d) {
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
                        new System.Exception("Couldn't load chunk " + _position + ". Block Id "+id+" missing.");
                var block = snapshot.ToBlock(this, blockPos);
                _blocks[x, y, z] = block;
           
                block.Load(stream, formatter, version, region2d);
            });
            GenerateSaveBuffer(formatter);
            _modified = false;
        }

        public static void ForEachChunkPosition(Action<int, int, int> action) {
            for (var x = 0; x < ChunkLength; x++) {
                for (var y = ChunkLength - 1; y >= 0; y--) {
                    for (var z = 0; z < ChunkLength; z++) {
                        action.Invoke(x, y, z);
                    }
                }
            }
        }

        private void ForEachChunkPosition(Action<int, int, int, Block> action) {
            for (var x = 0; x < ChunkLength; x++) {
                for (var y = ChunkLength - 1; y >= 0; y--) {
                    for (var z = 0; z < ChunkLength; z++) {
                        action.Invoke(x, y, z, _blocks[x, y, z]);
                    }
                }
            }
        }
    }
}