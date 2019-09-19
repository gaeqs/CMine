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
        private readonly Dictionary<Vector3i, UnloadedChunkGeneration> _generations;
        private object _generationLock = new object();

        public UnloadedChunkGenerationManager(World world) {
            _world = world;
            _generations = new Dictionary<Vector3i, UnloadedChunkGeneration>();
        }

        public World World => _world;

        public void AddBlock(Vector3i position, BlockSnapshot snapshot, bool overrideBlocks) {
            var chunkPosition = position >> Chunk.WorldPositionShift;
            var chunkWorldPosition = chunkPosition << Chunk.WorldPositionShift;
            lock (_generationLock) {
                if (_generations.TryGetValue(chunkPosition, out var generation)) {
                    generation.AddBlock(position - chunkWorldPosition, snapshot, overrideBlocks);
                    return;
                }
            }

            var newGeneration = new UnloadedChunkGeneration();
            newGeneration.AddBlock(position - chunkWorldPosition, snapshot, overrideBlocks);
            lock (_generationLock) {
                _generations.Add(chunkPosition, newGeneration);
            }
        }

        public bool PostGenerateChunk(Chunk chunk, BlockSnapshot[,,] snapshots, World2dRegion region2d) {
            UnloadedChunkGeneration generation;
            lock (_generationLock) {
                if (!_generations.TryGetValue(chunk.Position, out generation)) return true;
                _generations.Remove(chunk.Position);
            }

            return generation.OnChunkLoad(snapshots, chunk, region2d);
        }

        public void OnChunkLoad(Chunk chunk) {
            if (!chunk.Natural) {
                lock (_generationLock) {
                    _generations.Remove(chunk.Position);
                }

                return;
            }

            UnloadedChunkGeneration generation;
            lock (_generationLock) {
                if (!_generations.TryGetValue(chunk.Position, out generation)) return;
                _generations.Remove(chunk.Position);
            }

            generation.AddToLoadedChunk(chunk);
        }

        public void FlushToAllChunks() {
            var list = new List<Vector3i>();
            lock (_generationLock) {
                foreach (var generation in _generations) {
                    var chunk = _world.GetChunk(generation.Key);
                    if (chunk == null) continue;
                    generation.Value.AddToLoadedChunk(chunk);
                    list.Add(generation.Key);
                }
                list.ForEach(t => _generations.Remove(t));
            }
        }

        public void Save() {
            const uint version = 0;
            var file = _world.Folder + Path.DirectorySeparatorChar + FileName;

            var stream = new DeflateStream(File.Open(file, FileMode.OpenOrCreate, FileAccess.Write),
                CompressionMode.Compress);
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, version);

            lock (_generationLock) {
                foreach (var entry in _generations) {
                    formatter.Serialize(stream, true);
                    formatter.Serialize(stream, entry.Key);
                    entry.Value.Save(stream, formatter);
                }
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
                lock (_generationLock) {
                    _generations.Add(position, value);
                }
            }
            stream.Close();
        }
    }
}