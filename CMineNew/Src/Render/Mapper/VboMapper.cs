using System;
using System.Collections.Generic;
using CMineNew.DataStructure.Queue;
using CMineNew.Render.Object;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Render.Mapper{
    public class VboMapper<TKey>{
        private VertexBufferObject _vbo;
        private VertexArrayObject _vao;

        private readonly int _elementSize;

        private OnResize _onResize;

        private BufferTarget _bufferTarget;

        private int _amount;
        private int _maximumAmount;

        private int _updates;
        private volatile bool _onBackground, _requiresResize;


        private readonly EConcurrentLinkedQueue<VboMapperTask<TKey>> _tasks;
        private readonly Dictionary<TKey, int> _offsets;
        private readonly Dictionary<int, TKey> _keys;
        private readonly object _backgroundLock = new object();

        public VboMapper(VertexBufferObject vbo, VertexArrayObject vao, int elementSize, int maximumAmount,
            OnResize onResize, BufferTarget bufferTarget = BufferTarget.ArrayBuffer) {
            _vbo = vbo;
            _vao = vao;

            _elementSize = elementSize;

            _onResize = onResize;

            _bufferTarget = bufferTarget;

            _amount = 0;
            _maximumAmount = maximumAmount;

            _updates = 0;
            _onBackground = false;
            _requiresResize = false;

            _tasks = new EConcurrentLinkedQueue<VboMapperTask<TKey>>();
            _offsets = new Dictionary<TKey, int>();
            _keys = new Dictionary<int, TKey>();
        }

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
            set {
                if (_onBackground == value) return;
                _onBackground = value;
                if (value) {
                    _vbo.StartMapping();
                }
            }
        }

        public int getPointer(TKey key) {
            if (!_offsets.TryGetValue(key, out var point)) {
                return -1;
            }

            return point;
        }


        public void AddTask(VboMapperTask<TKey> task) {
            if (_requiresResize || !_onBackground || _vbo == null || !_vbo.Mapping) {
                _tasks.Push(task);
                return;
            }

            ExecuteTask(task, false);
            _updates++;
        }

        public void FlushQueue() {
            _updates = 0;
            if (_tasks.IsEmpty()) {
                if (!_onBackground) {
                    _vbo.FinishMapping();
                }

                return;
            }

            if (!_onBackground) {
                _vbo.StartMapping();
            }

            while (!_tasks.IsEmpty()) {
                if (_requiresResize) {
                    Console.WriteLine("Expanding buffer... " + _amount + " >= " + _maximumAmount);
                    ResizeBuffer();
                    _requiresResize = false;
                }

                unsafe {
                    if (_vbo.Pointer == null) {
                        Console.WriteLine("Warning! Pointer is null!");
                        break;
                    }

                    var current = _tasks.Pop();
                    if (current == null) continue;
                    ExecuteTask(current, true);
                    _updates++;
                }
            }

            if (!_onBackground) {
                _vbo.FinishMapping();
            }
        }

        public bool ContainsKey(TKey key) {
            lock (_backgroundLock) {
                return _offsets.ContainsKey(key);
            }
        }

        private void ExecuteTask(VboMapperTask<TKey> task, bool fromRender) {
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

            if (_requiresResize) {
                Console.WriteLine("??? " + fromRender);
            }

            if (_amount < _maximumAmount) return;
            Console.WriteLine("REQUIRES RESIZE: " + _amount);
            _requiresResize = true;
        }

        private void AddToMap(TKey key, float[] data) {
            if (_offsets.TryGetValue(key, out var point)) {
                _vbo.AddToMap(data, _elementSize * point);
                return;
            }

            _vbo.AddToMap(data, _elementSize * _amount);

            _offsets[key] = _amount;
            _keys[_amount] = key;
            _amount++;
        }

        private void EditMap(TKey key, float[] data, int offset) {
            if (!_offsets.TryGetValue(key, out var point)) return;
            _vbo.AddToMap(data, _elementSize * point + offset);
        }

        private void RemoveFromMap(TKey key) {
            if (!_offsets.TryGetValue(key, out var point)) return;
            if (point == -1) return;

            if (point == _amount - 1) {
                _offsets.Remove(key);
                _keys.Remove(_amount - 1);
                _amount--;
                return;
            }

            _vbo.MoveMapDataFloat(_elementSize * (_amount - 1), _elementSize * point, _elementSize);

            var lastKey = _keys[_amount - 1];
            _offsets[lastKey] = point;
            _keys[point] = lastKey;
            _keys.Remove(_amount - 1);
            _offsets.Remove(key);
            _amount--;
        }

        private void ResizeBuffer() {
            lock (_backgroundLock) {
                var oldBkg = _onBackground;
                _onBackground = false;
                _vbo.FinishMapping();
                var newVbo = new VertexBufferObject();
                _vbo.Bind(BufferTarget.CopyReadBuffer);
                newVbo.Bind(BufferTarget.CopyWriteBuffer);

                newVbo.SetData(BufferTarget.CopyWriteBuffer, _maximumAmount * _elementSize * sizeof(float) * 3 / 2,
                    BufferUsageHint.StreamDraw);
                GL.CopyBufferSubData(BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer,
                    IntPtr.Zero, IntPtr.Zero, _amount * _elementSize * sizeof(float));

                _onResize?.Invoke(_vao, _vbo, newVbo);
                _vbo.CleanUp();
                _vbo = newVbo;
                _maximumAmount = _maximumAmount * 3 / 2;
                newVbo.StartMapping();
                _onBackground = oldBkg;
            }
        }
    }

    public delegate void OnResize(VertexArrayObject obj, VertexBufferObject oldBuffer, VertexBufferObject newBuffer);
}