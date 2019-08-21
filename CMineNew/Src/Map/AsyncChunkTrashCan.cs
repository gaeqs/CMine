using System.Collections.Generic;
using System.Threading;
using CMine.DataStructure.Queue;
using CMineNew.Geometry;

namespace CMineNew.Map{
    public class AsyncChunkTrashCan{
        private readonly World _world;
        private readonly EDynamicPriorityQueue<Chunk> _queue;
        private Thread _thread;
        private bool _alive;
        private Vector3i _playerPosition;

        public AsyncChunkTrashCan(World world) {
            _world = world;
            _queue = new EDynamicPriorityQueue<Chunk>(Comparer<Chunk>
                .Create((o1, o2) => (o2.Position - _playerPosition).Mul(1, 2, 1).LengthSquared() -
                                    (o1.Position - _playerPosition).Mul(1, 2, 1).LengthSquared()));
            _thread = null;
            _alive = false;
        }

        public EDynamicPriorityQueue<Chunk> Queue => _queue;

        public void StartThread() {
            if (_thread != null) return;
            _thread = new Thread(Run) {
                Priority = ThreadPriority.Lowest, Name = _world.Name + "'s Async Chunk Trash Can"
            };
            _alive = true;
            _thread.Start();
        }

        public void Kill() {
            _alive = false;
        }

        private void Run() {
            while (_alive) {
                Chunk chunk;
                lock (_queue) {
                    if (_queue.Size() < 500) continue;
                    _playerPosition = new Vector3i(_world.Player.Position) >> 4;
                    chunk = _queue.Pop();
                }

                chunk?.RemoveAllBlockFacesFromRender();
            }

            _thread = null;
        }
    }
}