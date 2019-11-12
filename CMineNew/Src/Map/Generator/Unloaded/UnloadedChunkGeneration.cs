using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Snapshot;
using CMineNew.Map.BlockData.Snapshot.Interface;
using CMineNew.Map.BlockData.Type;

namespace CMineNew.Map.Generator.Unloaded{
    public class UnloadedChunkGeneration{
        private readonly UnloadedChunkGenerationBlock[,,] _data;

        public UnloadedChunkGeneration() {
            _data = new UnloadedChunkGenerationBlock[Chunk.ChunkLength, Chunk.ChunkLength, Chunk.ChunkLength];
        }

        public void AddBlock(Vector3i position, BlockSnapshot snapshot, bool overrideBlocks) {
            if (overrideBlocks) {
                _data[position.X, position.Y, position.Z] = new UnloadedChunkGenerationBlock(snapshot, true);
            }
            else {
                var old = _data[position.X, position.Y, position.Z];
                if (old == null || old.Snapshot is BlockSnapshotAir || old.Snapshot is BlockSnapshotTallGrass
                    || old.Snapshot is BlockSnapshotOakLeaves) {
                    _data[position.X, position.Y, position.Z] = new UnloadedChunkGenerationBlock(snapshot, false);
                }
            }
        }

        public void AddToLoadedChunk(Chunk chunk) {
            var chunkPos = chunk.Position << Chunk.WorldPositionShift;
            var regionPos = new Vector2i(chunk.Position.X, chunk.Position.Z) >> World2dRegion.ChunkPositionShift;
            var region = chunk.World.Regions2d[regionPos];

            Chunk.ForEachChunkPosition((x, y, z) => {
                var pos = new Vector3i(x, y, z);
                var data = _data[x, y, z];
                if (data == null) return;
                chunk.Natural = false;
                chunk.Modified = true;
                if (data.OverrideBlocks) {
                    chunk.SetBlock(UpdateSnapshot(data.Snapshot, chunkPos + pos, region), pos);
                    return;
                }

                var block = chunk.GetBlock(pos);
                if (block != null && !(block is BlockAir) && !(block is BlockTallGrass)) return;
                chunk.SetBlock(UpdateSnapshot(data.Snapshot, chunkPos + pos, region), pos);
            });
        }

        public bool OnChunkLoad(BlockSnapshot[,,] snapshots, Chunk chunk, World2dRegion region2d) {
            var empty = true;
            var chunkPos = chunk.Position << Chunk.WorldPositionShift;
            Chunk.ForEachChunkPosition((x, y, z) => {
                var blockPos = chunkPos + new Vector3i(x, y, z);
                var data = _data[x, y, z];
                if (data == null) return;
                chunk.Natural = false;
                chunk.Modified = true;
                if (data.OverrideBlocks) {
                    if (!(data.Snapshot is BlockSnapshotAir)) {
                        empty = false;
                    }

                    snapshots[x, z, y] = UpdateSnapshot(data.Snapshot, blockPos, region2d);
                    return;
                }

                var snapshot = snapshots[x, z, y];
                if (snapshot != null && !(snapshot is BlockSnapshotAir) &&
                    !(snapshot is BlockSnapshotTallGrass)) return;
                if (!(data.Snapshot is BlockSnapshotAir)) {
                    empty = false;
                }

                snapshots[x, z, y] = UpdateSnapshot(data.Snapshot, blockPos, region2d);
            });
            return empty;
        }

        public void Save(Stream stream, BinaryFormatter formatter) {
            for (var x = 0; x < Chunk.ChunkLength; x++) {
                for (var y = 0; y < Chunk.ChunkLength; y++) {
                    for (var z = 0; z < Chunk.ChunkLength; z++) {
                        var data = _data[x, y, z];
                        formatter.Serialize(stream, data == null ? (byte) 0 : (byte) 1);
                        data?.Save(stream, formatter);
                    }
                }
            }
        }

        public void Load(Stream stream, BinaryFormatter formatter) {
            for (var x = 0; x < Chunk.ChunkLength; x++) {
                for (var y = 0; y < Chunk.ChunkLength; y++) {
                    for (var z = 0; z < Chunk.ChunkLength; z++) {
                        if ((byte) formatter.Deserialize(stream) == 0) continue;
                        _data[x, y, z] = new UnloadedChunkGenerationBlock(stream, formatter);
                    }
                }
            }
        }

        private BlockSnapshot UpdateSnapshot(BlockSnapshot snapshot, Vector3i position,
            World2dRegion region) {
            if (!(snapshot is IGrass grass)) return snapshot;
            var pos2d = new Vector2i(position.X, position.Z);
            var relative = pos2d - (region.Position << World2dRegion.WorldPositionShift);
            grass.GrassColor = region.InterpolatedGrassColors[relative.X, relative.Y];

            return snapshot;
        }
    }
}