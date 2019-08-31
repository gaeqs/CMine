using System.Collections.Generic;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Snapshot;

namespace CMineNew.Map.Generator.Unloaded{
    public class UnloadedChunkGenerationManager{
        private readonly World _world;
        private readonly Dictionary<Vector3i, UnloadedChunkGeneration> _generations;

        public UnloadedChunkGenerationManager(World world) {
            _world = world;
            _generations = new Dictionary<Vector3i, UnloadedChunkGeneration>();
        }

        public World World => _world;

        public void AddBlock(Vector3i position, BlockSnapshot snapshot, bool overrideBlocks) {
            var chunkPosition = position >> Chunk.WorldPositionShift;
            var chunkWorldPosition = chunkPosition << Chunk.WorldPositionShift;
            if (_generations.TryGetValue(chunkPosition, out var generation)) {
                generation.AddBlock(position - chunkWorldPosition, snapshot, overrideBlocks);
                return;
            }

            generation = new UnloadedChunkGeneration();
            generation.AddBlock(position - chunkWorldPosition, snapshot, overrideBlocks);
            _generations.Add(chunkPosition, generation);
        }

        public bool PostGenerateChunk(Chunk chunk, BlockSnapshot[,,] snapshots, World2dRegion region2d) {
            if (!_generations.TryGetValue(chunk.Position, out var generation)) return true;
            _generations.Remove(chunk.Position);
            return generation.OnChunkLoad(snapshots, chunk, region2d);
        }

        public void OnChunkLoad(Chunk chunk) {
            if (!chunk.Natural) {
                _generations.Remove(chunk.Position);
                return;
            }
            if (!_generations.TryGetValue(chunk.Position, out var generation)) return;
            _generations.Remove(chunk.Position);
            generation.AddToLoadedChunk(chunk);
        }

        public void FlushToAllChunks() {
            var list = new List<Vector3i>();
            foreach (var generation in _generations) {
                var chunk = _world.GetChunk(generation.Key);
                if (chunk == null) continue;
                generation.Value.AddToLoadedChunk(chunk);
                list.Add(generation.Key);
            }

            list.ForEach(t => _generations.Remove(t));
        }
    }
}