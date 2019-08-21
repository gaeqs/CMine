using System.Collections.Generic;
using CMineNew.Map.BlockData;
using CMineNew.Map.BlockData.Model;
using CMineNew.Map.BlockData.Render;

namespace CMineNew.Map{
    public class ChunkRegionRender{
        private readonly ChunkRegion _chunkRegion;
        private readonly Dictionary<string, BlockRender> _renders;

        private readonly object _lock = new object();


        public ChunkRegionRender(ChunkRegion chunkRegion) {
            _chunkRegion = chunkRegion;
            lock (_lock) {
                _renders = new Dictionary<string, BlockRender>();
            }
        }

        public ChunkRegion ChunkRegion => _chunkRegion;

        public void AddData(int mapper, Block block) {
            GetOrCreateRender(block.BlockModel).AddData(mapper, block);
        }

        public void RemoveData(int mapper, Block block) {
            GetOrCreateRender(block.BlockModel).RemoveData(mapper, block);
        }

        public void Draw() {
            lock (_lock) {
                foreach (var render in _renders.Values) {
                    render.Draw();
                }
            }
        }

        public void DrawAfterPostRender() {
            lock (_lock) {
                foreach (var render in _renders.Values) {
                    render.DrawAfterPostRender();
                }
            }
        }

        public void FlushInBackground() {
            lock (_lock) {
                foreach (var render in _renders.Values) {
                    render.FlushInBackground();
                }
            }
        }

        public void CleanUp() {
            lock (_lock) {
                foreach (var render in _renders.Values) {
                    render.CleanUp();
                }
            }

            _renders.Clear();
        }

        private BlockRender GetOrCreateRender(BlockModel model) {
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