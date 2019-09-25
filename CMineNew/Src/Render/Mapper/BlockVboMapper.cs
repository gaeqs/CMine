using System;
using CMineNew.DataStructure.Queue;
using CMineNew.Geometry;
using CMineNew.Map;
using CMineNew.Map.BlockData;
using CMineNew.Render.Object;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Render.Mapper{
    public class BlockVboMapper : VboMapper<Block>{
        private readonly ChunkRegion _chunkRegion;
        private readonly Vector3i _regionWorldPosition;
        private VertexBufferObject _vbo;
        private VertexArrayObject _vao;

        private readonly int _elementSize;

        private OnResize _onResize;

        private BufferTarget _bufferTarget;

        private int _amount;
        private int _maximumAmount;

        private int _updates;
        private volatile bool _onBackground, _requiresResize;


        private readonly EConcurrentLinkedQueue<VboMapperTask<Block>> _tasks;
        private readonly int[,,] _offsets;

        public BlockVboMapper(ChunkRegion chunkRegion, VertexBufferObject vbo, VertexArrayObject vao, int elementSize,
            int maximumAmount,
            OnResize onResize, BufferTarget bufferTarget = BufferTarget.ArrayBuffer) {
            _chunkRegion = chunkRegion;
            _regionWorldPosition = chunkRegion.Position << World2dRegion.WorldPositionShift;
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

            _tasks = new EConcurrentLinkedQueue<VboMapperTask<Block>>();
            _offsets = new int[ChunkRegion.RegionLength, ChunkRegion.RegionLength, ChunkRegion.RegionLength];
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
            set {
                if (_onBackground == value) return;
                _onBackground = value;
                if (value) {
                    _vbo.StartMapping(_bufferTarget);
                }
            }
        }

        public int GetPointer(Block key) {
            var pos = key.Position - _regionWorldPosition;
            return _offsets[pos.X, pos.Y, pos.Z];
        }


        public void AddTask(VboMapperTask<Block> task) {
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
                    _vbo.FinishMapping(_bufferTarget);
                }

                return;
            }

            if (!_onBackground) {
                _vbo.StartMapping(_bufferTarget);
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
                _vbo.FinishMapping(_bufferTarget);
            }
        }

        public bool ContainsKey(Block key) {
            var pos = key.Position - _regionWorldPosition;
            return _offsets[pos.X, pos.Y, pos.Z] != -1;
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
            var pos = key.Position - _regionWorldPosition;
            var point = _offsets[pos.X, pos.Y, pos.Z];
            if (point != -1) {
                _vbo.AddToMap(data, _elementSize * point);
                return;
            }

            _vbo.AddToMap(data, _elementSize * _amount);

            _offsets[pos.X, pos.Y, pos.Z] = _amount;
            _amount++;
        }

        private void EditMap(Block key, float[] data, int offset) {
            var pos = key.Position - _regionWorldPosition;
            var point = _offsets[pos.X, pos.Y, pos.Z];
            if (point == -1) return;
            _vbo.AddToMap(data, _elementSize * point + offset);
        }

        private void RemoveFromMap(Block key) {
            var pos = key.Position - _regionWorldPosition;
            var point = _offsets[pos.X, pos.Y, pos.Z];
            if (point == -1) return;

            if (point == _amount - 1) {
                _offsets[pos.X, pos.Y, pos.Z] = -1;
                _amount--;
                return;
            }

            _vbo.MoveMapDataFloat(_elementSize * (_amount - 1), _elementSize * point, _elementSize);
            var lastKey = new Vector3i(_vbo.GetDataFromMap(_elementSize * (_amount - 1), 3), true);
            lastKey -= _regionWorldPosition;
            _offsets[lastKey.X, lastKey.Y, lastKey.Z] = point;
            _offsets[pos.X, pos.Y, pos.Z] = -1;
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