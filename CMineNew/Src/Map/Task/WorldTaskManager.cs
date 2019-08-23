using System;
using CMine.DataStructure.List;

namespace CMineNew.Map.Task{
    public class WorldTaskManager{
        private readonly ELinkedList<WorldTask> _tasks;

        public WorldTaskManager() {
            _tasks = new ELinkedList<WorldTask>();
        }

        public void AddTask(WorldTask task) {
            if (task == null) throw new System.Exception("Task cannot be null!");
            _tasks.Add(task);
        }

        public void Tick(long dif) {
            var enumerator = (ELinkedList<WorldTask>.ELinkedListEnumerator<WorldTask>) _tasks.GetEnumerator();
            if (_tasks.Size() != 0) {
                Console.WriteLine(_tasks.Size());
            }

            while (enumerator.MoveNext()) {
                var current = enumerator.Current;
                if (current == null || !current.TryToExecute(dif)) continue;
                enumerator.Remove();
            }

            enumerator.Dispose();
        }
    }
}