namespace CMineNew.Map.Task{
    public abstract class WorldTask{
        private long _ticksToExecution;

        public WorldTask(long ticks) {
            _ticksToExecution = ticks;
        }

        public bool TryToExecute(long delay) {
            _ticksToExecution -= delay;
            if (_ticksToExecution >= 0) return false;
            _ticksToExecution = 0;
            Run();
            return true;
        }


        public abstract void Run();
    }
}