using System.Collections.Generic;
using CMineNew.Geometry;
using CMineNew.RayTrace;
using CMineNew.Render;
using OpenTK;

namespace CMineNew.Map{
    public class World : Room{
        private readonly Camera _camera;
        private readonly BlockRayTracer _blockRayTracer;

        private readonly Dictionary<Vector3i, ChunkRegion> _chunkRegions;

        public World(string name) : base(name) {
            _camera = new Camera(new Vector3(0), new Vector3(0, 0, 1), new Vector3(0, 1, 0), 110);

            _blockRayTracer = new BlockRayTracer(this, _camera.Position, _camera.LookAt, 5);
            _chunkRegions = new Dictionary<Vector3i, ChunkRegion>();
        }

        public Camera Camera => _camera;

        public BlockRayTracer BlockRayTracer => _blockRayTracer;

        public Dictionary<Vector3i, ChunkRegion> ChunkRegions => _chunkRegions;
        
        public ChunkRegion GetChunkRegion(Vector3i position) {
            return ChunkRegions.TryGetValue(position, out var region) ? region : null;
        }

        public ChunkRegion GetChunkRegionFromWorldPosition(Vector3i position) {
            return ChunkRegions.TryGetValue(position >> 6, out var region) ? region : null;
        }

        public Chunk GetChunk(Vector3i position) {
            return ChunkRegions.TryGetValue(position >> 2, out var region)
                ? region.GetChunkFromChunkPosition(position)
                : null;
        }

        public Chunk GetChunkFromWorldPosition(Vector3i position) {
            return ChunkRegions.TryGetValue(position >> 6, out var region)
                ? region.GetChunkFromChunkPosition(position >> 4)
                : null;
        }

        public Block GetBlock(Vector3i position) {
            return GetChunkFromWorldPosition(position)?.GetBlockFromWorldPosition(position);
        }

        public void SetBlock(Block block, Vector3i position) {
            var regionPosition = position >> 6;
            var chunkPosition = position >> 4;

            var region = GetChunkRegion(regionPosition);
            if (region == null) {
                region = new ChunkRegion(this, regionPosition);
                ChunkRegions[regionPosition] = region;
            }

            var chunk = region.GetChunkFromChunkPosition(chunkPosition);
            if (chunk == null) {
                chunk = new Chunk(region, chunkPosition);
                region.SetChunk(chunk, chunkPosition - (regionPosition << 2));
            }

            chunk.SetBlockFromWorldPosition(block, position);
        }
    }
}