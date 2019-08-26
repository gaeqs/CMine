using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using CMineNew.Geometry;

namespace CMineNew.Map{
    public class ChunkRegion{
        public const int RegionChunkLength = 4;
        public const int RegionChunkSize = RegionChunkLength * RegionChunkLength;
        public const int RegionChunkVolume = RegionChunkLength * RegionChunkLength * RegionChunkLength;
        public const int RegionLength = 64;
        public const int RegionSize = RegionLength * RegionLength;
        public const int RegionVolume = RegionLength * RegionLength * RegionLength;
        public const int ChunkPositionShift = 2;
        public const int WorldPositionShift = 6;
        public static readonly float RegionRadius = (float) Math.Sqrt(32 * 32 * 3);

        private readonly World _world;
        private readonly Vector3i _position;
        private readonly Chunk[,,] _savedChunks;
        private readonly Chunk[,,] _chunks;

        private ChunkRegionRender _render;
        private bool _deleted;

        private object _deletionLock = new object();

        public ChunkRegion(World world, Vector3i position) {
            _world = world;
            _position = position;
            _chunks = new Chunk[RegionChunkLength, RegionChunkLength, RegionChunkLength];
            _savedChunks = new Chunk[RegionChunkLength, RegionChunkLength, RegionChunkLength];
            _render = new ChunkRegionRender(this);
            _deleted = true;
        }

        public World World => _world;

        public Vector3i Position => _position;

        public Chunk[,,] Chunks => _chunks;

        public ChunkRegionRender Render => _render;

        public bool Deleted => _deleted;

        public Chunk GetChunk(Vector3i regionPosition) {
            return _chunks[regionPosition.X, regionPosition.Y, regionPosition.Z];
        }

        public Chunk GetChunkFromChunkPosition(Vector3i chunkPosition) {
            chunkPosition -= _position << ChunkPositionShift;
            return _chunks[chunkPosition.X, chunkPosition.Y, chunkPosition.Z];
        }

        public void SetChunk(Chunk chunk, Vector3i regionPosition) {
            lock (_deletionLock) {
                if (chunk != null) {
                    chunk.Position = (_position << ChunkPositionShift) + regionPosition;
                }

                _chunks[regionPosition.X, regionPosition.Y, regionPosition.Z] = chunk;
                _savedChunks[regionPosition.X, regionPosition.Y, regionPosition.Z] = chunk;
                if (!_deleted || chunk == null) return;
                _deleted = false;
            }
        }

        public bool TryLoadSavedChunk(Vector3i regionPosition, out Chunk chunk) {
            chunk = _savedChunks[regionPosition.X, regionPosition.Y, regionPosition.Z];
            if (chunk == null) return false;
            _chunks[regionPosition.X, regionPosition.Y, regionPosition.Z] = chunk;
            chunk.SendOnPlaceEventToAllBlocks(false);
            return true;
        }

        public void DeleteIfEmpty() {
            if (_deleted) return;
            if (_chunks.Cast<Chunk>().Any(chunk => chunk != null)) return;
            Delete();
        }

        public void LoadIfDeleted() {
            if (!_deleted) return;
            var file = _world.Folder + Path.DirectorySeparatorChar + _position.X + "-" + _position.Y + "-" +
                       _position.Z + ".reg";
            if (!File.Exists(file)) return;

            var pos = _position << ChunkPositionShift;

            var stream = new DeflateStream(File.Open(file, FileMode.Open, FileAccess.Read), CompressionMode.Decompress);
            var formatter = new BinaryFormatter();
            var version = (uint) formatter.Deserialize(stream);
            Console.WriteLine("Loading region " + _position + " (Version: " + version + ")");
            Console.WriteLine("Thread: " + Thread.CurrentThread.Name);
            var now = DateTime.Now.Ticks;
            for (var x = 0; x < RegionChunkLength; x++) {
                for (var y = 0; y < RegionChunkLength; y++) {
                    for (var z = 0; z < RegionChunkLength; z++) {
                        if (stream.ReadByte() == 0) {
                            _savedChunks[x, y, z] = null;
                            continue;
                        }

                        var chunkPos = pos + new Vector3i(x, y, z);
                        var chunk = new Chunk(this, chunkPos);
                        _savedChunks[x, y, z] = chunk;
                        chunk.Load(stream, formatter, version);
                    }
                }
            }

            stream.Close();
            _deleted = false;


            var delay = (DateTime.Now.Ticks - now) * 1000 / CMine.TicksPerSecondF;
            Console.WriteLine("Region loaded in " + delay + " millis.");
        }

        public void Save() {
            const uint version = 0;
            var file = _world.Folder + Path.DirectorySeparatorChar + _position.X + "-" + _position.Y + "-" +
                       _position.Z + ".reg";

            var stream = new DeflateStream(File.Open(file, FileMode.OpenOrCreate, FileAccess.Write),
                CompressionMode.Compress);
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, version);

            var now = DateTime.Now.Ticks;

            for (var x = 0; x < RegionChunkLength; x++) {
                for (var y = 0; y < RegionChunkLength; y++) {
                    for (var z = 0; z < RegionChunkLength; z++) {
                        var chunk = _savedChunks[x, y, z];
                        stream.WriteByte(chunk == null ? (byte) 0 : (byte) 1);
                        chunk?.Save(stream, formatter);
                    }
                }
            }

            stream.Close();


            var delay = (DateTime.Now.Ticks - now) * 1000 / CMine.TicksPerSecondF;
            Console.WriteLine("Region saved in " + delay + " millis.");
        }

        public void Delete() {
            lock (_deletionLock) {
                _deleted = true;
                _render.CleanUp();
            }

            for (var x = 0; x < RegionChunkLength; x++) {
                for (var y = 0; y < RegionChunkLength; y++) {
                    for (var z = 0; z < RegionChunkLength; z++) {
                        _savedChunks[x, y, z] = null;
                    }
                }
            }
        }

        public void Tick(long delay) {
            foreach (var chunk in _chunks) {
                chunk?.TaskManager.Tick(delay);
            }
        }
    }
}