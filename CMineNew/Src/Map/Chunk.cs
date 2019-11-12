using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CMineNew.Geometry;
using CMineNew.Map.BlockData;
using CMineNew.Map.BlockData.Snapshot;
using CMineNew.Map.BlockData.Type;
using CMineNew.Map.Task;

namespace CMineNew.Map {
    public class Chunk {
        public const int ChunkLength = 16;
        public const int ChunkSize = ChunkLength * ChunkLength;
        public const int ChunkVolume = ChunkLength * ChunkLength * ChunkLength;
        public const int WorldPositionShift = 4;

        private readonly ChunkRegion _region;

        private Vector3i _position;

        //X, Z, Y
        private readonly Block[,,] _blocks;
        private readonly WorldTaskManager _taskManager;

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
            return _blocks[chunkPosition.X, chunkPosition.Z, chunkPosition.Y];
        }

        public Block GetBlockFromWorldPosition(Vector3i worldPosition) {
            return GetBlock(worldPosition - (_position << WorldPositionShift));
        }

        public Block SetBlock(BlockSnapshot snapshot, Vector3i chunkPosition) {
            return SetBlock(snapshot, chunkPosition, f => true);
        }

        public Block SetBlock(BlockSnapshot snapshot, Vector3i chunkPosition, Func<Block, bool> canBeReplaced) {
            var old = _blocks[chunkPosition.X, chunkPosition.Z, chunkPosition.Y];
            if (!canBeReplaced.Invoke(old)) return null;

            var position = (_position << WorldPositionShift) + chunkPosition;
            var block = snapshot.ToBlock(this, position);

            _blocks[chunkPosition.X, chunkPosition.Z, chunkPosition.Y] = block;

            var references = old.NeighbourReferences;
            if (references != null) {
                if (block != null) {
                    block.NeighbourReferences = references;
                }

                for (var i = 0; i < references.Length; i++) {
                    if (!references[i].TryGetTarget(out var n)) continue;
                    n.OnNeighbourBlockChange0(old, block, BlockFaceMethods.GetOpposite((BlockFace) i));
                }
            }
            else {
                var neighbours = GetNeighbourBlocks(new Block[6], position, chunkPosition);
                if (block != null) {
                    block.Neighbours = neighbours;
                }

                for (var i = 0; i < neighbours.Length; i++) {
                    neighbours[i]?.OnNeighbourBlockChange0(old, block,
                        BlockFaceMethods.GetOpposite((BlockFace) i));
                }
            }


            if (old != null) {
                old.OnRemove0(block, out var blockList, out var sunList);
                block?.OnPlace0(old, true, true, true);
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
                block?.OnPlace0(old, true, true, true);
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
            AddBlockForEachChunkPosition((x, y, z, wPos) => snapshots[x, z, y].ToBlock(this, wPos));

            SendOnPlaceEventToAllBlocks(true, false, false);
            ForEachChunkPosition((x, y, z, block) => {
                block.AddToRender();
                block.ExpandSunlight();
            });
            _modified = true;
        }

        public void SendOnPlaceEventToAllBlocks(bool triggerWorldUpdates, bool expandSunlight, bool addToRender) {
            var blocks = new Block[6];
            ForEachChunkPosition((x, y, z, block) => {
                GetNeighbourBlocks(blocks, block.Position, new Vector3i(x, y, z));
                block.Neighbours = blocks;
            });
            ForEachChunkPosition((x, y, z, block) => {
                if (block == null) return;
                block.OnPlace0(null, triggerWorldUpdates, expandSunlight, addToRender);
                switch (x) {
                    case 0: {
                        var target = block.GetNeighbour(BlockFace.West);
                        target?.OnNeighbourBlockChange0(null, block, BlockFace.East);
                        break;
                    }
                    case 15: {
                        var target = block.GetNeighbour(BlockFace.East);
                        target?.OnNeighbourBlockChange0(null, block, BlockFace.West);
                        break;
                    }
                }

                switch (y) {
                    case 0: {
                        var target = block.GetNeighbour(BlockFace.Down);
                        target?.OnNeighbourBlockChange0(null, block, BlockFace.Up);
                        break;
                    }
                    case 15: {
                        var target = block.GetNeighbour(BlockFace.Up);
                        target?.OnNeighbourBlockChange0(null, block, BlockFace.Down);
                        break;
                    }
                }

                switch (z) {
                    case 0: {
                        var target = block.GetNeighbour(BlockFace.North);
                        target?.OnNeighbourBlockChange0(null, block, BlockFace.South);
                        break;
                    }
                    case 15: {
                        var target = block.GetNeighbour(BlockFace.South);
                        target?.OnNeighbourBlockChange0(null, block, BlockFace.North);
                        break;
                    }
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
                : _blocks[chunkPosition.X + 1, chunkPosition.Z, chunkPosition.Y];
            //West block
            blocks[(int) BlockFace.West] = chunkPosition.X == 0
                ? World.GetBlock(position - new Vector3i(1, 0, 0))
                : _blocks[chunkPosition.X - 1, chunkPosition.Z, chunkPosition.Y];
            //South
            blocks[(int) BlockFace.South] = chunkPosition.Z == 15
                ? World.GetBlock(position + new Vector3i(0, 0, 1))
                : _blocks[chunkPosition.X, chunkPosition.Z + 1, chunkPosition.Y];
            //North
            blocks[(int) BlockFace.North] = chunkPosition.Z == 0
                ? World.GetBlock(position - new Vector3i(0, 0, 1))
                : _blocks[chunkPosition.X, chunkPosition.Z - 1, chunkPosition.Y];
            //Up
            blocks[(int) BlockFace.Up] = chunkPosition.Y == 15
                ? World.GetBlock(position + new Vector3i(0, 1, 0))
                : _blocks[chunkPosition.X, chunkPosition.Z, chunkPosition.Y + 1];
            //Down
            blocks[(int) BlockFace.Down] = chunkPosition.Y == 0
                ? World.GetBlock(position - new Vector3i(0, 1, 0))
                : _blocks[chunkPosition.X, chunkPosition.Z, chunkPosition.Y - 1];
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
                    _blocks[x, z, y] = new BlockAir(this, blockPos);
                    return;
                }

                var id = (string) formatter.Deserialize(stream);
                if (!BlockManager.TryGet(id, out var snapshot))
                    throw
                        new System.Exception("Couldn't load chunk " + _position + ". Block Id " + id + " missing.");
                var block = snapshot.ToBlock(this, blockPos);
                _blocks[x, z, y] = block;

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

        public void AddBlockForEachChunkPosition(Func<int, int, int, Vector3i, Block> action) {
            var pos = _position << WorldPositionShift;
            for (var x = 0; x < ChunkLength; x++) {
                for (var z = 0; z < ChunkLength; z++) {
                    for (var y = ChunkLength - 1; y >= 0; y--) {
                        _blocks[x, z, y] = action.Invoke(x, y, z, new Vector3i(x + pos.X, y + pos.Y, z + pos.Z));
                    }
                }
            }
        }

        public void ForEachChunkPosition(Action<int, int, int, Block> action) {
            for (var x = 0; x < ChunkLength; x++) {
                for (var z = 0; z < ChunkLength; z++) {
                    for (var y = ChunkLength - 1; y >= 0; y--) {
                        action.Invoke(x, y, z, _blocks[x, z, y]);
                    }
                }
            }
        }
    }
}