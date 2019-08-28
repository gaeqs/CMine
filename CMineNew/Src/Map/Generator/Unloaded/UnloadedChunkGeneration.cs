using CMineNew.Geometry;
using CMineNew.Map.BlockData.Snapshot;
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
            Chunk.ForEachChunkPosition((x, y, z) => {
                var pos = new Vector3i(x, y, z);
                var data = _data[x, y, z];
                if (data == null) return;
                if (data.OverrideBlocks) {
                    chunk.SetBlock(data.Snapshot, pos);
                    return;
                }

                var block = chunk.GetBlock(pos);
                if (block != null && !(block is BlockAir) && !(block is BlockTallGrass)) return;
                chunk.SetBlock(data.Snapshot, pos);
            });
        }

        public bool OnChunkLoad(BlockSnapshot[,,] snapshots) {
            var empty = true;
            Chunk.ForEachChunkPosition((x, y, z) => {
                var data = _data[x, y, z];
                if (data == null) return;
                if (data.OverrideBlocks) {
                    if (!(data.Snapshot is BlockSnapshotAir)) {
                        empty = false;
                    }

                    snapshots[x, y, z] = data.Snapshot;
                    return;
                }

                var snapshot = snapshots[x, y, z];
                if (snapshot != null && !(snapshot is BlockSnapshotAir) &&
                    !(snapshot is BlockSnapshotTallGrass)) return;
                if (!(data.Snapshot is BlockSnapshotAir)) {
                    empty = false;
                }

                snapshots[x, y, z] = data.Snapshot;
            });
            return empty;
        }
    }
}