using System.Collections.Concurrent;
using System.Threading;

namespace CMineNew.Map{
    public class AsyncChunkTrashCan{
        private readonly World _world;
        private readonly ConcurrentQueue<Chunk> _queue;
        private Thread _thread;
        private bool _alive;

        public AsyncChunkTrashCan(World world) {
            _world = world;
            _queue = new ConcurrentQueue<Chunk>();
            _thread = null;
            _alive = false;
        }

        public ConcurrentQueue<Chunk> Queue => _queue;

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
                if (!_queue.TryDequeue(out var chunk)) continue;
                chunk?.RemoveAllBlockFacesFromRender();
            }

            _thread = null;
        }
    }
}