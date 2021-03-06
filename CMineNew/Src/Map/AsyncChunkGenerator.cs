using System;
using System.Collections.Generic;
using System.Threading;
using CMineNew.DataStructure.Queue;
using CMineNew.Geometry;

namespace CMineNew.Map{
    public class AsyncChunkGenerator{
        private readonly World _world;
        private readonly EDynamicPriorityQueue<Vector3i> _chunksToGenerate;
        private Thread _thread;
        private bool _alive;
        private Vector3i _playerPosition;
        private volatile bool _generateChunkArea;

        public AsyncChunkGenerator(World world) {
            _world = world;
            _chunksToGenerate = new EDynamicPriorityQueue<Vector3i>(Comparer<Vector3i>.Create(Comparision));
            _thread = null;
            _alive = false;
        }

        public bool GenerateChunkArea {
            get => _generateChunkArea;
            set => _generateChunkArea = value;
        }

        public int ChunksToGenerateSize => _chunksToGenerate.Size();

        public void StartThread() {
            if (_thread != null) return;
            _thread = new Thread(Run) {
                Priority = ThreadPriority.Lowest, Name = _world.Name + "'s Async Chunk Generator"
            };
            _alive = true;
            _thread.Start();
        }

        public void AddToQueue(Vector3i position) {
            _chunksToGenerate.Push(position);
        }

        public void Kill() {
            _alive = false;
        }

        private void Run() {
            while (_alive) {
                _playerPosition = new Vector3i(_world.Player.Position) >> 4;
                if (_generateChunkArea) {
                    TriggerGenerateChunkArea();
                    _generateChunkArea = false;
                }

                if (_chunksToGenerate.Size() <= 0) continue;
                var position = _chunksToGenerate.Pop();
                _world.CreateChunk(position);
            }

            _thread = null;
        }

        private void TriggerGenerateChunkArea() {
            const int chunkRadius = CMine.ChunkRadius;
            const int chunkRadiusSquared = chunkRadius * chunkRadius;
            var regions = _world.ChunkRegions;
            foreach (var region in regions.Values) {
                var chunks = region.Chunks;
                lock (region) {
                    for (var x = 0; x < 4; x++) {
                        for (var y = 0; y < 4; y++) {
                            for (var z = 0; z < 4; z++) {
                                var chunk = chunks[x, y, z];
                                if (chunk == null) continue;
                                if ((chunk.Position - _playerPosition).LengthSquared() <= chunkRadiusSquared)
                                    continue;
                                chunks[x, y, z] = null;
                                chunk.RemoveAllBlockFacesFromRender();
                            }
                        }
                    }

                    if (!region.DeleteIfEmpty()) continue;
                    if (region.World.ChunkRegions.TryRemove(region.Position, out _)) {
                        Console.WriteLine("Region " + region.Position + " deleted");
                    }
                }
            }

            _chunksToGenerate.RemoveIf(pos => (pos - _playerPosition).LengthSquared() > chunkRadiusSquared);
            for (var x = -chunkRadius; x <= chunkRadius; x++) {
                for (var y = -2; y <= 2; y++) {
                    for (var z = -chunkRadius; z <= chunkRadius; z++) {
                        var chunkPos = new Vector3i(x, y, z);
                        var wChunkPos = chunkPos + _playerPosition;
                        if (chunkPos.LengthSquared() > chunkRadiusSquared
                            || _world.GetChunk(wChunkPos) != null || _chunksToGenerate.Contains(wChunkPos))
                            continue;
                        _chunksToGenerate.Push(wChunkPos);
                    }
                }
            }
        }

        private int Comparision(Vector3i o1, Vector3i o2) {
            o1 -= _playerPosition;
            o2 -= _playerPosition;
            var o1d = o1.LengthSquared();
            var o2d = o2.LengthSquared();
            if (o1d < 9) {
                if (o2d < 9) {
                    return o1d - o2d;
                }

                return -1;
            }

            if (o2d < 9) {
                return 1;
            }

            return o1.Mul(1, 2, 1).LengthSquared() - o2.Mul(1, 2, 1).LengthSquared();
        }
    }
}