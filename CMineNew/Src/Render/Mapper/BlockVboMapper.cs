using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CMineNew.Geometry;
using CMineNew.Map;
using CMineNew.Map.BlockData;
using CMineNew.Render.Object;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Render.Mapper{
    public class BlockVboMapper : VboMapper<Block>{
        private readonly ChunkRegion _chunkRegion;
        private VertexBufferObject _vbo;
        private VertexArrayObject _vao;

        private readonly int _elementSize;

        private OnResize _onResize;

        private BufferTarget _bufferTarget;

        private int _amount;
        private int _maximumAmount;

        private int _updates;
        private volatile bool _onBackground, _requiresResize;


        private readonly ConcurrentQueue<VboMapperTask<Block>> _tasks;
        private readonly Dictionary<Vector3i, int> _offsets;

        public BlockVboMapper(ChunkRegion chunkRegion, VertexBufferObject vbo, VertexArrayObject vao, int elementSize,
            int maximumAmount,
            OnResize onResize, BufferTarget bufferTarget = BufferTarget.ArrayBuffer) {
            _chunkRegion = chunkRegion;
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

            _tasks = new ConcurrentQueue<VboMapperTask<Block>>();
            _offsets = new Dictionary<Vector3i, int>();
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
            return _offsets.TryGetValue(key.Position, out var pointer) ? pointer : -1;
        }


        public void AddTask(VboMapperTask<Block> task) {
            _tasks.Enqueue(task);
        }

        public void FlushQueue() {
            _updates = 0;
            if (_tasks.IsEmpty) {
                return;
            }

            _vbo.StartMapping(_bufferTarget);

            while (_vbo.Mapping && _tasks.TryDequeue(out var current)) {
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

                    if (current == null) continue;
                    ExecuteTask(current, true);
                    _updates++;
                }
            }

            _vbo.FinishMapping(_bufferTarget);
        }

        public bool ContainsKey(Block key) {
            return _offsets.ContainsKey(key.Position);
        }

        private void ExecuteTask(VboMapperTask<Block> task, bool fromRender) {
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

        private void AddToMap(Block key, float[] data) {
            if (_offsets.TryGetValue(key.Position, out var point)) {
                try {
                    _vbo.AddToMap(data, _elementSize * point);
                }
                catch (AccessViolationException ex) {
                    Console.WriteLine("Point: "+point);
                    throw ex;
                }

                return;
            }

            _vbo.AddToMap(data, _elementSize * _amount);

            _offsets[key.Position] = _amount;
            _amount++;
        }

        private void EditMap(Block key, float[] data, int offset) {
            if (!_offsets.TryGetValue(key.Position, out var point)) return;
            _vbo.AddToMap(data, _elementSize * point + offset);
        }

        private void RemoveFromMap(Block key) {
            if (!_offsets.TryGetValue(key.Position, out var point)) return;

            if (point == _amount - 1) {
                _offsets[key.Position] = -1;
                _amount--;
                return;
            }

            _vbo.MoveMapDataFloat(_elementSize * (_amount - 1), _elementSize * point, _elementSize);
            var lastKey = new Vector3i(_vbo.GetDataFromMap(_elementSize * (_amount - 1), 3), true);
            _offsets[lastKey] = point;


            _offsets.Remove(key.Position);
            _amount--;
        }

        private void ResizeBuffer() {
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