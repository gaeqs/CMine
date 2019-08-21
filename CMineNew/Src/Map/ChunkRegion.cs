using System;
using System.Linq;
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
        private readonly Chunk[,,] _chunks;

        private ChunkRegionRender _render;
        private bool _deleted;

        public ChunkRegion(World world, Vector3i position) {
            _world = world;
            _position = position;
            _chunks = new Chunk[RegionChunkLength, RegionChunkLength, RegionChunkLength];
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
            if (chunk != null) {
                chunk.Position = (_position << ChunkPositionShift) + regionPosition;
            }

            _chunks[regionPosition.X, regionPosition.Y, regionPosition.Z] = chunk;
            if (!_deleted || chunk == null) return;
            _deleted = false;
        }

        public void DeleteIfEmpty() {
            if (_deleted) return;
            if (_chunks.Cast<Chunk>().Any(chunk => chunk != null)) return;
            Delete();
        }

        public void Delete() {
            _deleted = true;
            _render.CleanUp();
        }

        public void Tick(long delay) {
            foreach (var chunk in _chunks) {
                chunk?.TaskManager.Tick(delay);
            }
        }
    }
}