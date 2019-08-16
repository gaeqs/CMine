namespace CMine.DataStructure.Queue{
    public class EConcurrentLinkedQueue<T>{
        private volatile Node<T> _first, _last;
        private volatile object _firstLock = new object(), _lastLock = new object();

        public EConcurrentLinkedQueue() {
        }

        public void Push(T value) {
            lock (_lastLock) {
                if (_last == null) {
                    lock (_firstLock) {
                        _first = _last = new Node<T>(value);
                    }
                    return;
                }

                var node = new Node<T>(value);
                _last._next = node;
                _last = node;
            }
        }

        public T Pop() {
            lock (_firstLock) {
                var elem = _first._value;
                _first = _first._next;
                if (_first != null) return elem;
                lock (_lastLock) {
                    _last = null;
                }
                return elem;
            }
        }

        public bool IsEmpty() {
            lock (_firstLock) {
                return _first == null;
            }
        }


        private class Node<T>{
            public T _value;
            public Node<T> _next;

            public Node(T value) {
                _value = value;
            }
        }
    }
}