using System;
using System.Collections;
using System.Collections.Generic;

namespace CMine.DataStructure.List{
    public class EArrayList<TE> : IEList<TE>{
        private const int DefaultSize = 10;

        protected TE[] _array;
        protected int _size;

        public EArrayList() {
            _array = new TE[DefaultSize];
            _size = 0;
        }

        public EArrayList(int defaultSize) {
            _array = new TE[defaultSize];
            _size = 0;
        }

        public bool Add(TE elem) {
            var expanded = false;
            if (_array.Length == _size) {
                Expand();
                expanded = true;
            }

            _array[_size] = elem;
            _size++;
            return expanded;
        }

        public TE Get(int index) {
            if (index < 0 || index >= _size)
                throw new IndexOutOfRangeException(
                    "Maximum range is " + (_size - 1) + ". Range " + index + " is not allowed.");
            return _array[index];
        }

        public TE Set(TE elem, int index) {
            if (index < 0 || index >= _size)
                throw new IndexOutOfRangeException(
                    "Maximum range is " + (_size - 1) + ". Range " + index + " is not allowed.");
            var previous = _array[index];
            _array[index] = elem;
            return previous;
        }

        public bool Add(TE elem, int index) {
            if (index < 0 || index > _size)
                throw new IndexOutOfRangeException(
                    "Maximum range is " + _size + ". Range " + index + " is not allowed.");
            if (index == _size)
                return Add(elem);
            var expanded = false;
            if (_array.Length == _size) {
                Expand();
                expanded = true;
            }

            for (var i = _size - 1; i >= index; i--) {
                _array[i + 1] = _array[i];
            }

            _array[index] = elem;
            _size++;
            return expanded;
        }

        public bool Remove(TE elem) {
            var found = false;
            var index = 0;
            while (!found && index < _size) {
                if (_array[index].Equals(elem)) {
                    for (var i = index; i < _size - 1; i++) {
                        _array[i] = _array[i + 1];
                    }

                    _array[_size - 1] = default;
                    _size--;
                    found = true;
                }
                else index++;
            }

            return false;
        }

        public TE RemoveFromIndex(int index) {
            if (index < 0 || index >= _size)
                throw new IndexOutOfRangeException(
                    "Maximum range is " + (_size - 1) + ". Range " + index + " is not allowed.");
            var elem = _array[index];
            for (var i = 0; i < _size - 1; i++) {
                _array[i] = _array[i + 1];
            }

            _array[_size - 1] = default;
            _size--;
            return elem;
        }

        public TE RemoveLast() {
            if (_size == 0)
                throw new IndexOutOfRangeException("Maximum range is " + (_size - 1) + ". Range 0 is not allowed.");
            var elem = _array[_size - 1];
            _array[_size - 1] = default;
            _size--;
            return elem;
        }

        public bool Contains(TE elem) {
            var found = false;
            var index = 0;
            while (!found && index < _size) {
                if (_array[index].Equals(elem)) found = true;
                else index++;
            }

            return found;
        }

        public bool IsEmpty() {
            return _size == 0;
        }

        public int Size() {
            return _size;
        }

        public void Clear() {
            for (var i = 0; i < _size; i++) {
                _array[i] = default;
            }

            _size = 0;
        }

        public TE[] ToArray() {
            var newArray = new TE[_size];
            for (var i = 0; i < _size; i++) {
                newArray[i] = _array[i];
            }

            return newArray;
        }

        public void ForEach(Action<TE> action) {
            for (var i = 0; i < _size; i++) {
                action.Invoke(_array[i]);
            }
        }


        private void Expand() {
            var newArray = new TE[_array.Length * 2];
            for (var i = 0; i < _array.Length; i++) {
                newArray[i] = _array[i];
            }

            _array = newArray;
        }


        public IEnumerator<TE> GetEnumerator() {
            return new EArrayListEnumerator<TE>(this);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public override string ToString() {
            if (_size == 0) return "[]";
            var s = "[";
            for (var i = 0; i < _size - 1; i++) {
                s += _array[i] + ", ";
            }

            return s + _array[_size - 1] + "]";
        }

        public TE this[int index] {
            get => Get(index);
            set => Set(value, index);
        }

        private class EArrayListEnumerator<TI> : IEnumerator<TI>{
            private readonly EArrayList<TI> _parent;
            private int _index;

            public EArrayListEnumerator(EArrayList<TI> parent) {
                _parent = parent;
                _index = -1;
            }

            public void Dispose() {
            }

            public bool MoveNext() {
                _index++;
                return _index < _parent._size;
            }

            public void Reset() {
                _index = -1;
            }

            public TI Current => _parent._array[_index];

            object IEnumerator.Current => Current;
        }
    }
}