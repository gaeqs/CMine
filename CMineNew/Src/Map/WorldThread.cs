using System;
using System.Diagnostics;
using System.Threading;

namespace CMineNew.Map{
    public class WorldThread{
        private World _world;

        private Thread _thread;
        private Stopwatch _stopwatch;
        private bool _alive;
        private float load;

        public WorldThread(World world) {
            _world = world;
            _stopwatch = new Stopwatch();
            _alive = false;
        }

        public float Load => load;

        public void StartThread() {
            if (_alive) return;
            _alive = true;
            _thread = new Thread(Run) {Name = "World thread", Priority = ThreadPriority.Highest};
            _thread.Start();
            _stopwatch.Restart();
        }

        public void Kill() {
            _alive = false;
        }

        private void Run() {
            var constDelay = CMine.TicksPerSecond / 70;
            while (_alive) {
                _stopwatch.Restart();
                _world.WorldTaskManager.Tick(constDelay);
                foreach (var worldEntity in _world.Entities) {
                    worldEntity.Tick(constDelay);
                }
                foreach (var region in _world.ChunkRegions.Values) {
                    region.Tick(constDelay);
                }

                _stopwatch.Stop();
                load = _stopwatch.ElapsedTicks * 1000 / (CMine.TicksPerSecondF * 14);
                Thread.Sleep(new TimeSpan(Math.Max(CMine.TicksPerSecond / 70 - _stopwatch.ElapsedTicks, 0)));
            }
        }
    }
}