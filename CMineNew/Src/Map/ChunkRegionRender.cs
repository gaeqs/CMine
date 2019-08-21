using System.Collections.Generic;
using CMineNew.Map.BlockData;
using CMineNew.Map.BlockData.Model;
using CMineNew.Map.BlockData.Render;

namespace CMineNew.Map{
    public class ChunkRegionRender{
        private readonly ChunkRegion _chunkRegion;
        private readonly Dictionary<string, BlockRender> _renders;

        private readonly object _lock = new object();
        private bool _deleted;


        public ChunkRegionRender(ChunkRegion chunkRegion) {
            _chunkRegion = chunkRegion;
            lock (_lock) {
                _renders = new Dictionary<string, BlockRender>();
            }

            _deleted = false;
        }

        public ChunkRegion ChunkRegion => _chunkRegion;

        public void AddData(int mapper, Block block) {
            lock (_lock) {
                _deleted = false;
            }
            GetOrCreateRender(block.BlockModel).AddData(mapper, block);
        }

        public void RemoveData(int mapper, Block block) {
            lock (_lock) {
                _deleted = false;
            }
            GetOrCreateRender(block.BlockModel).RemoveData(mapper, block);
        }

        public void Draw() {
            if(_deleted) return;
            lock (_lock) {
                foreach (var render in _renders.Values) {
                    render.Draw();
                }
            }
        }

        public void DrawAfterPostRender() {
            if(_deleted) return;
            lock (_lock) {
                foreach (var render in _renders.Values) {
                    render.DrawAfterPostRender();
                }
            }
        }

        public void FlushInBackground() {
            if(_deleted) return;
            lock (_lock) {
                foreach (var render in _renders.Values) {
                    render.FlushInBackground();
                }
            }
        }

        public void CleanUp() {
            if(_deleted) return;
            _deleted = true;
            lock (_lock) {
                foreach (var render in _renders.Values) {
                    render.CleanUp();
                }
                _renders.Clear();
            }
        }

        private BlockRender GetOrCreateRender(BlockModel model) {
            lock (_lock) {
                if(_deleted) return null;
                var modelId = model.Id;
                if (_renders.TryGetValue(modelId, out var render)) {
                    return render;
                }

                render = model.CreateBlockRender(_chunkRegion);
                _renders.Add(modelId, render);
                return render;
            }
        }
    }
}