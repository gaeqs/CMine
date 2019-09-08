using System;
using System.Collections.Generic;

namespace CMineNew.DataStructure{
    public class ArrayMapper<TKey>{
        private int _elementSize;
        private int _amount;
        private int _maximumAmount;
        private float[] _array;

        private readonly Dictionary<TKey, int> _offsets;
        private readonly List<TKey> _keys;

        public ArrayMapper(int elementSize, int initialMaximumAmount) {
            _elementSize = elementSize;
            _amount = 0;
            _maximumAmount = initialMaximumAmount;
            _array = new float[elementSize * initialMaximumAmount];
            _offsets = new Dictionary<TKey, int>();
            _keys = new List<TKey>();
        }

        public void Add(TKey key, float[] data) {
            if (_offsets.TryGetValue(key, out var point)) {
                Edit(key, data, point, 0);
                return;
            }

            if (_amount == _maximumAmount) {
                Resize();
            }

            var offset = _elementSize * _amount;
            foreach (var t in data) {
                _array[offset++] = t;
            }

            _offsets[key] = _amount;
            _keys.Add(key);
            _amount++;
        }

        public void Edit(TKey key, float[] data, int dataOffset) {
            if (!_offsets.TryGetValue(key, out var point)) return;
            Edit(key, data, point, dataOffset);
        }

        private void Edit(TKey key, float[] data, int point, int dataOffset) {
            var offset = _elementSize * point + dataOffset;
            foreach (var t in data) {
                _array[offset++] = t;
            }
        }

        public void Remove(TKey key) {
            if (!_offsets.TryGetValue(key, out var point)) return;
            if (point == _amount - 1) {
                _offsets.Remove(key);
                _keys.RemoveAt(_amount - 1);
                _amount--;
                return;
            }

            var pf = _elementSize * (_amount - 1);
            var pi = _elementSize * point;
            for (var i = 0; i < _elementSize; i++) {
                _array[pi++] = _array[pf++];
            }

            var lastKey = _keys[_amount - 1];
            _offsets[lastKey] = point;
            _keys[point] = lastKey;
            _keys.RemoveAt(_amount - 1);
            _offsets.Remove(key);
            _amount--;
        }

        private void Resize() {
            var array = new float[_array.Length << 1];
            for (var i = 0; i < _array.Length; i++) {
                array[i] = _array[i];
            }

            _array = array;
            _maximumAmount <<= 1;
        }

        public unsafe int Map(float* pointer, int amount, int offset, Func<TKey, bool> canMap) {
            if (offset >= _amount) return 0;
            var current = offset;
            var added = 0;
            while (added < amount && current < _amount) {
                var key = _keys[current];
                if (!canMap.Invoke(key)) {
                    current++;
                    continue;
                }

                var pi = _elementSize * current;
                for (var i = 0; i < _elementSize; i++) {
                    *pointer++ = _array[pi++];
                }

                current++;
                added++;
            }

            return added;
        }
    }
}