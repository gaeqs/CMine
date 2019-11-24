using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using CMineNew.Geometry;
using CMineNew.Util;

namespace CMineNew.Map{
    public class ChunkRegion{
        public const string RegionsFolder = "regions";

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

        private World2dRegion _world2dRegion;
        private bool _deleted;

        private object _deletionLock = new object();

        public ChunkRegion(World world, Vector3i position) {
            _world = world;
            _position = position;
            _chunks = new Chunk[RegionChunkLength, RegionChunkLength, RegionChunkLength];
            _savedChunks = new Chunk[RegionChunkLength, RegionChunkLength, RegionChunkLength];
            _deleted = true;

            _world2dRegion = world.GetOrCreate2dRegion(new Vector2i(position.X, position.Z));
        }

        public World World => _world;

        public Vector3i Position => _position;

        public Chunk[,,] Chunks => _chunks;

        public bool Deleted => _deleted;

        public World2dRegion World2dRegion => _world2dRegion;

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
            var now = DateTime.Now.Ticks;
            _chunks[regionPosition.X, regionPosition.Y, regionPosition.Z] = chunk;
            chunk.SendOnPlaceEventToAllBlocks(false, false, false);
            
            chunk.ForEachChunkPosition((x, y, z, block) => block.AddToRender());
            
            chunk.World.DelayViewer.AddLoad(DateTime.Now.Ticks - now);
            return true;
        }

        public bool DeleteIfEmpty() {
            if (_deleted) return false;
            if (_chunks.Cast<Chunk>().Any(chunk => chunk != null)) return false;
            Delete();
            return true;
        }

        public void LoadIfDeleted() {
            if (!_deleted) return;
            var regionsFolder = _world.Folder + Path.DirectorySeparatorChar + RegionsFolder;
            FolderUtils.CreateRegionFolderIfNotExist(regionsFolder);
            var file = regionsFolder + Path.DirectorySeparatorChar + _position.X + "-" + _position.Y + "-" + _position.Z + ".reg";
            if (!File.Exists(file)) return;

            var pos = _position << ChunkPositionShift;

            var stream = new DeflateStream(File.Open(file, FileMode.Open, FileAccess.Read), CompressionMode.Decompress);
            var formatter = new BinaryFormatter();
            var version = (uint) formatter.Deserialize(stream);
            var region2d = _world.GetOrCreate2dRegion(new Vector2i(_position.X, _position.Z));
            ForEachRegionPosition((x, y, z) => {
                if (stream.ReadByte() == 0) {
                    _savedChunks[x, y, z] = null;
                    return;
                }

                var chunkPos = pos + new Vector3i(x, y, z);
                var chunk = new Chunk(this, chunkPos);
                _savedChunks[x, y, z] = chunk;
                chunk.Load(stream, formatter, version, region2d);
            });

            stream.Close();
            _deleted = false;
        }

        public void Save() {
            const uint version = 0;
            var regionsFolder = _world.Folder + Path.DirectorySeparatorChar + RegionsFolder;
            FolderUtils.CreateRegionFolderIfNotExist(regionsFolder);
            var file = regionsFolder + Path.DirectorySeparatorChar + _position.X + "-" + _position.Y + "-" + _position.Z + ".reg";

            var stream = new DeflateStream(File.Open(file, FileMode.OpenOrCreate, FileAccess.Write), CompressionMode.Compress);
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, version);

            ForEachRegionPosition((x, y, z) => {
                var chunk = _savedChunks[x, y, z];
                var save = chunk != null && !chunk.Natural;
                stream.WriteByte(save ? (byte) 1 : (byte) 0);
                if (!save) return;
                chunk.Save(stream, formatter);
            });

            stream.Close();
        }

        public void Delete() {
            lock (_deletionLock) {
                _deleted = true;
            }

            ForEachRegionPosition((x, y, z) => _savedChunks[x, y, z] = null);
        }

        public void Tick(long delay) {
            foreach (var chunk in _chunks) {
                chunk?.TaskManager.Tick(delay);
            }
        }

        public static void ForEachRegionPosition(Action<int, int, int> action) {
            for (var x = 0; x < RegionChunkLength; x++) {
                for (var y = 0; y < RegionChunkLength; y++) {
                    for (var z = 0; z < RegionChunkLength; z++) {
                        action.Invoke(x, y, z);
                    }
                }
            }
        }
    }
}