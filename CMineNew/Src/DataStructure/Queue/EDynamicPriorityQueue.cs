using System.Collections.Generic;
using CMineNew.DataStructure.List;

namespace CMineNew.DataStructure.Queue{
    public class EDynamicPriorityQueue<T> : ELinkedList<T>, IEQueue<T>{
        private Comparer<T> _comparer;

        public EDynamicPriorityQueue(Comparer<T> comparer) {
            _comparer = comparer;
        }

        public Comparer<T> Comparer => _comparer;

        public void Push(T elem) {
            Add(elem);
        }

        public T Pop() {
            var current = GetBestNode(out var previous);
            if(current == null) throw new System.Exception("Queue is empty.");
            if (previous == null)
                return RemoveFirst();
            
            var elem = current._value;
            previous._next = current._next;
            if (current._next == null)
                _last = previous;
            _size--;
            return elem;
        }

        public T Peek() {
            return _size == 0 ? default : GetBestNode(out _)._value;
        }

        public T Element() {
            return GetBestNode(out _)._value;
        }

        private Node<T> GetBestNode(out Node<T> bestPrevious) {
            var current = _first;
            bestPrevious = null;
            var best = _first;

            while (current._next != null) {
                var previous = current;
                current = current._next;
                if (_comparer.Compare(best._value, current._value) < 0) continue;
                best = current;
                bestPrevious = previous;
            }

            return best;
        }
    }
}