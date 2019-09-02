using System.Collections.Generic;
using System.Threading;
using CMineNew.DataStructure.Queue;
using CMineNew.Geometry;

namespace CMineNew.Map{
    public class AsyncChunkTrashCan{
        private readonly World _world;
        private readonly EConcurrentLinkedQueue<Chunk> _queue;
        private Thread _thread;
        private bool _alive;

        public AsyncChunkTrashCan(World world) {
            _world = world;
            _queue = new EConcurrentLinkedQueue<Chunk>();
            _thread = null;
            _alive = false;
        }

        public EConcurrentLinkedQueue<Chunk> Queue {
            get {
                lock (_queue) {
                    return _queue;
                }
            }
        }

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
                    if (_queue.IsEmpty()) continue;
                    chunk = _queue.Pop();
                }

                chunk?.RemoveAllBlockFacesFromRender();
            }

            _thread = null;
        }
    }
}