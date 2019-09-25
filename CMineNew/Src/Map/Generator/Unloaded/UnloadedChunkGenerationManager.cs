using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Snapshot;

namespace CMineNew.Map.Generator.Unloaded{
    public class UnloadedChunkGenerationManager{
        public const string FileName = "generation_cache.dat";

        private readonly World _world;
        private readonly ConcurrentDictionary<Vector3i, UnloadedChunkGeneration> _generations;

        public UnloadedChunkGenerationManager(World world) {
            _world = world;
            _generations = new ConcurrentDictionary<Vector3i, UnloadedChunkGeneration>();
        }

        public World World => _world;

        public void AddBlock(Vector3i position, BlockSnapshot snapshot, bool overrideBlocks) {
            var chunkPosition = position >> Chunk.WorldPositionShift;
            var chunkWorldPosition = chunkPosition << Chunk.WorldPositionShift;

            if (_generations.TryGetValue(chunkPosition, out var generation)) {
                generation.AddBlock(position - chunkWorldPosition, snapshot, overrideBlocks);
                return;
            }


            var newGeneration = new UnloadedChunkGeneration();
            newGeneration.AddBlock(position - chunkWorldPosition, snapshot, overrideBlocks);

            _generations.TryAdd(chunkPosition, newGeneration);
        }

        public bool PostGenerateChunk(Chunk chunk, BlockSnapshot[,,] snapshots, World2dRegion region2d) {
            if (!_generations.TryRemove(chunk.Position, out var generation)) return true;
            return generation.OnChunkLoad(snapshots, chunk, region2d);
        }

        public void OnChunkLoad(Chunk chunk) {
            if (!chunk.Natural) {
                _generations.TryRemove(chunk.Position, out _);
                return;
            }

            UnloadedChunkGeneration generation;
            if (!_generations.TryRemove(chunk.Position, out generation)) return;

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

            list.ForEach(t => _generations.TryRemove(t, out _));
        }

        public void Save() {
            const uint version = 0;
            var file = _world.Folder + Path.DirectorySeparatorChar + FileName;

            var stream = new DeflateStream(File.Open(file, FileMode.OpenOrCreate, FileAccess.Write),
                CompressionMode.Compress);
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, version);

            foreach (var entry in _generations) {
                formatter.Serialize(stream, true);
                formatter.Serialize(stream, entry.Key);
                entry.Value.Save(stream, formatter);
            }

            formatter.Serialize(stream, false);
            stream.Close();
        }

        public void Load() {
            var file = _world.Folder + Path.DirectorySeparatorChar + FileName;
            if (!File.Exists(file)) return;

            var stream = new DeflateStream(File.Open(file, FileMode.Open, FileAccess.Read), CompressionMode.Decompress);
            var formatter = new BinaryFormatter();
            var version = (uint) formatter.Deserialize(stream);
            while ((bool) formatter.Deserialize(stream)) {
                var position = (Vector3i) formatter.Deserialize(stream);
                var value = new UnloadedChunkGeneration();
                value.Load(stream, formatter);
                _generations.TryAdd(position, value);
            }

            stream.Close();
        }
    }
}