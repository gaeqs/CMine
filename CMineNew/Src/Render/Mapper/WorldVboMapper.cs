using System.Collections.Generic;
using CMineNew.Geometry;
using CMineNew.Map.BlockData;
using CMineNew.Map.BlockData.Model;
using CMineNew.Render.Object;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Render.Mapper{
    public class WorldVboMapper{
        private readonly BlockModel _model;
        private VertexBufferObject _vertexBufferObject;

        private uint _maxBlocks;
        private readonly uint _floatsPerBlock;
        private readonly Dictionary<Vector3i, uint> _blockIndices;
        private readonly Queue<uint> _removedIndices;
        private float[] _array;

        private volatile bool _updated, _expandVbo, _vboCreated;

        //Min inclusive, max exclusive
        private uint minUpdatedIndex, maxUpdatedIndex;

        public WorldVboMapper(uint floatsPerBlock, BlockModel model, uint initialMaxBlocks) {
            _model = model;
            _maxBlocks = initialMaxBlocks;
            _floatsPerBlock = floatsPerBlock;
            _blockIndices = new Dictionary<Vector3i, uint>((int) initialMaxBlocks);
            _removedIndices = new Queue<uint>();

            _vboCreated = false;
            _expandVbo = false;
            _updated = false;

            _array = new float[_maxBlocks * _floatsPerBlock];
            for (var i = 0; i < _array.Length; i++)
                _array[i] = -1;
            _updated = false;
            minUpdatedIndex = _maxBlocks;
            maxUpdatedIndex = 0;
        }

        public BlockModel Model => _model;

        public VertexBufferObject VertexBufferObject => _vertexBufferObject;

        public bool VboCreated => _vboCreated;

        public uint MaxBlocks => _maxBlocks;

        public bool AddBlock(Block block) {
            if (block.BlockModel != _model) return false;
            if (!_blockIndices.TryGetValue(block.Position, out var start)) {
                start = GetFirstAvailableIndex(out var expand);
                _blockIndices.Add(block.Position, start);
                if (expand) Expand();
            }

            start *= _floatsPerBlock;

            var data = _model.GetData(block);
            var end = start + _floatsPerBlock;
            for (var i = start; i < end; i++) {
                _array[i] = data[i - start];
            }

            if (minUpdatedIndex > start) minUpdatedIndex = start;
            if (maxUpdatedIndex < end) maxUpdatedIndex = end;
            _updated = true;
            return true;
        }

        public bool RemoveBlock(Block block) {
            if (block.BlockModel != _model) return false;
            if (!_blockIndices.TryGetValue(block.Position, out var start)) return false;
            _removedIndices.Enqueue(start);
            start *= _floatsPerBlock;
            var end = start + _floatsPerBlock;
            for (var i = start; i < end; i++) {
                _array[start] = -1;
            }

            if (minUpdatedIndex > start) minUpdatedIndex = start;
            if (maxUpdatedIndex < end) maxUpdatedIndex = end;
            _updated = true;
            return true;
        }

        public void Update() {
            if (!_updated) return;

            if (!_vboCreated) {
                _expandVbo = false;
                _vertexBufferObject = new VertexBufferObject();
                _vertexBufferObject.Bind(BufferTarget.ArrayBuffer);
                _vertexBufferObject.SetData(BufferTarget.ArrayBuffer, _array, BufferUsageHint.StreamDraw);
                _vboCreated = true;
            }
            else {
                _vertexBufferObject.Bind(BufferTarget.ArrayBuffer);

                if (_expandVbo) {
                    _vertexBufferObject.SetData(BufferTarget.ArrayBuffer, _array, BufferUsageHint.StreamDraw);
                    _expandVbo = false;
                }
                else {
                    _vertexBufferObject.SetSubData(BufferTarget.ArrayBuffer, _array,
                        (int) (maxUpdatedIndex - minUpdatedIndex) << 2, (int) minUpdatedIndex << 2);
                }
            }

            minUpdatedIndex = _maxBlocks;
            maxUpdatedIndex = 0;
        }

        private uint GetFirstAvailableIndex(out bool requiresExpansion) {
            if (_removedIndices.Count == 0) {
                var count = (uint) _blockIndices.Count;
                requiresExpansion = count == _maxBlocks;
                return count;
            }

            requiresExpansion = false;
            return _removedIndices.Dequeue();
        }

        private void Expand() {
            var newArray = new float[_array.Length << 1];
            for (var i = 0; i < _array.Length; i++) {
                newArray[i] = _array[i];
            }

            for (var i = _array.Length; i < newArray.Length; i++) {
                newArray[i] = -1;
            }

            _array = newArray;
            _expandVbo = true;
            _maxBlocks <<= 1;
        }
    }
}