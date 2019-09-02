using System;
using System.Collections;
using System.Collections.Generic;

namespace CMineNew.DataStructure.List{
    public class ELinkedList<TE> : IEList<TE>{
        protected Node<TE> _first, _last;
        protected int _size;

        public ELinkedList() {
            _size = 0;
        }

        public bool Add(TE elem) {
            var node = new Node<TE>(elem);
            if (_size == 0) {
                _first = _last = node;
            }
            else {
                _last._next = node;
                _last = node;
            }

            _size++;

            return true;
        }

        public bool Add(TE elem, int index) {
            if (index == _size) return Add(elem);
            var node = new Node<TE>(elem);
            if (index == 0) {
                node._next = _first;
                _first = node;
            }
            else {
                var previous = GetNode(index - 1);
                node._next = previous._next;
                previous._next = node;
            }

            _size++;

            return true;
        }

        public TE Get(int index) {
            if (index < 0 || index >= _size)
                throw new IndexOutOfRangeException(
                    "Maximum range is " + (_size - 1) + ". Range " + index + " is not allowed.");

            return GetNode(index)._value;
        }

        public bool Remove(TE elem) {
            var found = false;
            Node<TE> previous = null;
            var current = _first;
            while (!found && current != null) {
                if (current._value.Equals(elem)) {
                    found = true;
                    if (previous == null) {
                        _first = _first._next;
                        if (_size == 1)
                            _last = null;
                    }
                    else {
                        previous._next = current._next;
                        if (current._next == null)
                            _last = previous;
                    }

                    _size--;
                }
                else {
                    previous = current;
                    current = current._next;
                }
            }

            return found;
        }

        public TE RemoveFromIndex(int index) {
            if (index < 0 || index >= _size)
                throw new IndexOutOfRangeException(
                    "Maximum range is " + (_size - 1) + ". Range " + index + " is not allowed.");
            TE elem;
            if (index == 0) {
                elem = _first._value;
                _first = _first._next;
                if (_size == 1)
                    _last = null;
            }
            else {
                var previous = GetNode(index - 1);
                var current = previous._next;
                elem = current._value;
                previous._next = current._next;
                if (current._next == null)
                    _last = previous;
            }

            _size--;

            return elem;
        }

        public TE RemoveFirst() {
            if (_size == 0)
                throw new IndexOutOfRangeException("Maximum range is " + (_size - 1) + ". Range 0 is not allowed.");
            var elem = _first._value;
            _first = _first._next;
            if (_size == 1)
                _last = null;
            _size--;
            return elem;
        }

        public TE RemoveLast() {
            if (_size == 0)
                throw new IndexOutOfRangeException("Maximum range is " + (_size - 1) + ". Range 0 is not allowed.");
            if (_size == 1) {
                return RemoveFirst();
            }

            var previous = GetNode(_size - 2);
            var elem = previous._next._value;
            previous._next = null;
            _last = previous;
            _size--;

            return elem;
        }

        public TE Set(TE elem, int index) {
            if (index < 0 || index >= _size)
                throw new IndexOutOfRangeException(
                    "Maximum range is " + (_size - 1) + ". Range " + index + " is not allowed.");
            var node = GetNode(index);
            var previous = node._value;
            node._value = elem;

            return previous;
        }

        public bool Contains(TE elem) {
            var found = false;
            var current = _first;
            while (!found && current != null) {
                if (current._value.Equals(elem))
                    found = true;
                else current = current._next;
            }


            return found;
        }

        public bool IsEmpty() {
            return _size == 0;
        }

        public int Size() {
            return _size;
        }

        public int CalculateRealSize() {
            var i = 0;
            var current = _first;
            while (current != null) {
                i++;
                current = current._next;
            }

            return i;
        }

        public void Clear() {
            _first = _last = null;
            _size = 0;
        }

        public TE[] ToArray() {
            var array = new TE[_size];
            var index = 0;
            var current = _first;
            while (current != null) {
                array[index] = current._value;
                index++;
                current = current._next;
            }

            return array;
        }

        public void ForEach(Action<TE> action) {
            var current = _first;
            while (current != null) {
                action.Invoke(current._value);
                current = current._next;
            }
        }

        public bool TryGetNodeIf(Func<TE, bool> func, out Node<TE> result) {
            result = null;
            var node = _first;
            var found = false;
            while (!found && node != null) {
                if (func.Invoke(node._value)) {
                    result = node;
                    found = true;
                }
                else {
                    node = node._next;
                }
            }

            return found;
        }

        public int RemoveIf(Func<TE, bool> func) {
            var enumerator = new ELinkedListEnumerator<TE>(this);
            var amount = 0;
            while (enumerator.MoveNext()) {
                if (!func.Invoke(enumerator.Current)) continue;
                enumerator.Remove();
                amount++;
            }

            enumerator.Dispose();

            return amount;
        }


        private Node<TE> GetNode(int index) {
            var current = _first;
            for (var i = 0; i < index; i++)
                current = current._next;
            return current;
        }

        public IEnumerator<TE> GetEnumerator() {
            return new ELinkedListEnumerator<TE>(this);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public override string ToString() {
            if (_size == 0) return "[]";
            var s = "[";
            var current = _first;
            while (current._next != null) {
                s += current._value + ", ";
                current = current._next;
            }

            return s + current._value + "]";
        }


        public TE this[int index] {
            get => Get(index);
            set => Set(value, index);
        }

        public TE Last => _last._value;

        public TE First => _first._value;

        public class Node<TI>{
            public TI _value;
            public Node<TE> _next;

            public Node(TI value) {
                _value = value;
                _next = null;
            }
        }

        public class ELinkedListEnumerator<TI> : IEnumerator<TI>{
            private readonly ELinkedList<TI> _parent;
            private ELinkedList<TI>.Node<TI> _previous;
            private ELinkedList<TI>.Node<TI> _current;
            private bool _started;

            public ELinkedListEnumerator(ELinkedList<TI> parent) {
                _parent = parent;
                _previous = _current;
                _current = null;
                _started = false;
            }

            public void Dispose() {
            }

            public bool MoveNext() {
                if (_started) {
                    _previous = _current;
                    _current = _current._next;
                }
                else {
                    _current = _parent._first;
                    _started = true;
                }

                return _current != null;
            }

            public void Reset() {
                _current = null;
                _previous = null;
                _started = false;
            }

            public void Remove() {
                if (!_started) return;
                if (_previous == null) {
                    _parent._first = _parent._first._next;
                    _current = null;
                    _started = false;
                }
                else if (_current._next == null) {
                    _previous._next = null;
                    _parent._last = _previous;
                    _current = _previous;
                }
                else {
                    _previous._next = _current._next;
                    _current = _previous;
                }

                _parent._size--;
            }

            public TI Current => _current._value;

            object IEnumerator.Current => Current;
        }
    }
}