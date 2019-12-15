using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CMineNew.Geometry;
using CMineNew.Map;
using CMineNew.Map.BlockData;
using CMineNew.Render.Object;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Render.Mapper{
    public class ArrayBlockVboMapper : VboMapper<Block>{
        private readonly ChunkRegion _chunkRegion;
        private readonly Vector3i _regionWorldPosition;

        private VertexBufferObject _vbo;
        private VertexArrayObject _vao;
        private float[] _array;

        private readonly int _elementSize;
        private BufferTarget _bufferTarget;

        private int _amount;
        private int _maximumAmount;

        private int _updates;
        private volatile bool _onBackground, _requiresResize;


        private readonly ConcurrentQueue<VboMapperTask<Block>> _tasks;
        private readonly int[,,] _offsets;
        private readonly Dictionary<int, Vector3i> _keys;

        private volatile bool _updated;

        public ArrayBlockVboMapper(ChunkRegion chunkRegion, VertexBufferObject vbo, VertexArrayObject vao,
            int elementSize,
            int maximumAmount, BufferTarget bufferTarget = BufferTarget.ArrayBuffer) {
            _chunkRegion = chunkRegion;
            _regionWorldPosition = chunkRegion.Position << World2dRegion.WorldPositionShift;
            _vbo = vbo;
            _vao = vao;
            _array = new float[elementSize * maximumAmount];

            _elementSize = elementSize;

            _bufferTarget = bufferTarget;

            _amount = 0;
            _maximumAmount = maximumAmount;

            _updates = 0;
            _onBackground = false;
            _requiresResize = false;

            _tasks = new ConcurrentQueue<VboMapperTask<Block>>();
            _offsets = new int[ChunkRegion.RegionLength, ChunkRegion.RegionLength, ChunkRegion.RegionLength];
            _keys = new Dictionary<int, Vector3i>();
            for (var x = 0; x < ChunkRegion.RegionLength; x++) {
                for (var y = 0; y < ChunkRegion.RegionLength; y++) {
                    for (var z = 0; z < ChunkRegion.RegionLength; z++) {
                        _offsets[x, y, z] = -1;
                    }
                }
            }
        }

        public ChunkRegion ChunkRegion => _chunkRegion;

        public VertexBufferObject Vbo {
            get => _vbo;
            set => _vbo = value;
        }

        public VertexArrayObject Vao {
            get => _vao;
            set => _vao = value;
        }

        public int ElementSize => _elementSize;

        public int Amount => _amount;


        public int Updates => _updates;

        public bool OnBackground {
            get => _onBackground;
            set => _onBackground = value;
        }

        public int GetPointer(Block key) {
            var pos = key.Position - _regionWorldPosition;
            return _offsets[pos.X, pos.Y, pos.Z];
        }


        public void AddTask(VboMapperTask<Block> task) {
            ExecuteTask(task);
            _updated = true;
        }

        public void FlushQueue() {
            _updates = 0;
            if (!_updated) {
                return;
            }

            _updated = false;
            _vbo.Bind(_bufferTarget);
            _vbo.Orphan(_bufferTarget, BufferUsageHint.StreamDraw);
            _vbo.SetSubData(_bufferTarget, _array, (_elementSize * _amount) << 2, 0);
        }

        public bool ContainsKey(Block key) {
            var pos = key.Position - _regionWorldPosition;
            return _offsets[pos.X, pos.Y, pos.Z] != -1;
        }

        private void ExecuteTask(VboMapperTask<Block> task) {
            if (_requiresResize) {
                Console.WriteLine("Expanding buffer... " + _amount + " >= " + _maximumAmount);
                ResizeBuffer();
                _requiresResize = false;
            }

            switch (task.Type) {
                case VboMapperTaskType.Add:
                    AddToMap(task.Key, task.Data);
                    break;
                case VboMapperTaskType.Remove:
                    RemoveFromMap(task.Key);
                    break;
                case VboMapperTaskType.Edit:
                    EditMap(task.Key, task.Data, task.Offset);
                    break;
            }

            if (_amount < _maximumAmount) return;
            _requiresResize = true;
        }

        private void AddToMap(Block key, float[] data) {
            var pos = key.Position - _regionWorldPosition;
            var point = _offsets[pos.X, pos.Y, pos.Z];

            if (point != -1) {
                EditArrayFromIndex(point * _elementSize, data);
                return;
            }

            EditArrayFromIndex(_amount * _elementSize, data);

            _offsets[pos.X, pos.Y, pos.Z] = _amount;
            _keys[_amount] = pos;
            _amount++;
        }

        private void EditMap(Block key, float[] data, int offset) {
            var pos = key.Position - _regionWorldPosition;

            var point = _offsets[pos.X, pos.Y, pos.Z];
            if (point == -1) return;
            EditArrayFromIndex(point * _elementSize + offset, data);
        }

        private void EditArrayFromIndex(int arrayIndex, float[] data) {
            foreach (var f in data) {
                _array[arrayIndex++] = f;
            }
        }

        private void RemoveFromMap(Block key) {
            var pos = key.Position - _regionWorldPosition;

            var point = _offsets[pos.X, pos.Y, pos.Z];
            if (point == -1) return;
            if (point == _amount - 1) {
                _offsets[pos.X, pos.Y, pos.Z] = -1;
                _keys.Remove(_amount - 1);
                _amount--;
                return;
            }

            var removing = _elementSize * point;
            var last = _elementSize * (_amount - 1);
            for (var i = 0; i < _elementSize; i++) {
                _array[removing++] = _array[last++];
            }

            var lastKey = _keys[_amount - 1];

            _offsets[lastKey.X, lastKey.Y, lastKey.Z] = point;
            _offsets[pos.X, pos.Y, pos.Z] = -1;
            _keys[point] = lastKey;
            _keys.Remove(_amount - 1);
            _amount--;
        }

        private bool IsInsideArray(Vector3i vec) {
            return vec.X >= 0 && vec.Y >= 0 && vec.Z >= 0 &&
                   vec.X < ChunkRegion.RegionLength && vec.Y < ChunkRegion.RegionLength &&
                   vec.Z < ChunkRegion.RegionLength;
        }

        private void ResizeBuffer() {
            var newArray = new float[_array.Length << 1];
            for (var i = 0; i < _array.Length; i++) {
                newArray[i] = _array[i];
            }

            _array = newArray;
            _maximumAmount <<= 1;
            if(_vbo == null) return;
            _vbo.Bind(_bufferTarget);
            _vbo.SetData(_bufferTarget, _maximumAmount * _elementSize << 2, BufferUsageHint.StreamDraw);
        }
    }
}