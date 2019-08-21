using System;
using System.Collections.Generic;
using System.Threading;
using CMine.DataStructure.Queue;
using CMineNew.Geometry;
using CMineNew.Map.Task;

namespace CMineNew.Map{
    public class AsyncChunkGenerator{
        private World _world;
        private EDynamicPriorityQueue<Vector3i> _chunksToGenerate;
        private Thread _thread;
        private bool _alive;
        private Vector3i _playerPosition;
        private volatile bool _generateChunkArea;

        private object _queueLock = new object();

        public AsyncChunkGenerator(World world) {
            _world = world;
            _chunksToGenerate = new EDynamicPriorityQueue<Vector3i>(Comparer<Vector3i>
                .Create((o1, o2) => (o1 - _playerPosition).Mul(1, 2, 1).LengthSquared() -
                                    (o2 - _playerPosition).Mul(1, 2, 1).LengthSquared()));
            _thread = null;
            _alive = false;
        }

        public bool GenerateChunkArea {
            get => _generateChunkArea;
            set => _generateChunkArea = value;
        }

        public int ChunksToGenerateSize {
            get {
                lock (_queueLock) {
                    return _chunksToGenerate.Size();
                }
            }
        }

        public void StartThread() {
            if (_thread != null) return;
            _thread = new Thread(Run) {
                Priority = ThreadPriority.BelowNormal, Name = _world.Name + "'s Async Chunk Generator"
            };
            _alive = true;
            _thread.Start();
        }

        public void AddToQueue(Vector3i position) {
            lock (_queueLock) {
                _chunksToGenerate.Push(position);
            }
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

                Vector3i position;
                lock (_queueLock) {
                    if (_chunksToGenerate.Size() == 0) continue;
                    if (_chunksToGenerate.Size() < 0) {
                        Console.WriteLine("OH NO");
                    }

                    position = _chunksToGenerate.Pop();
                }

                _world.CreateChunk(position);
            }

            _thread = null;
        }

        private void TriggerGenerateChunkArea() {
            const int chunkRadius = 8;
            const int chunkRadiusSquared = chunkRadius * chunkRadius;
            var regions = _world.ChunkRegions;
            var trashQueue = _world.AsyncChunkTrashCan.Queue;
            foreach (var region in regions.Values) {
                var chunks = region.Chunks;
                lock (region) {
                    for (var x = 0; x < 4; x++) {
                        for (var y = 0; y < 4; y++) {
                            for (var z = 0; z < 4; z++) {
                                var chunk = chunks[x, y, z];
                                if (chunk == null) continue;
                                if ((chunk.Position - _playerPosition).LengthSquared() <= chunkRadiusSquared) continue;
                                chunks[x, y, z] = null;
                                trashQueue.Push(chunk);
                            }
                        }
                    }

                    _world.WorldTaskManager.AddTask(new WorldTaskRegionDelete(region));
                }
            }

            lock (_queueLock) {
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
        }
    }
}