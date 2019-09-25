using System.Collections.Concurrent;
using System.Collections.Generic;
using CMineNew.Map.BlockData;
using CMineNew.Map.BlockData.Model;
using CMineNew.Map.BlockData.Render;

namespace CMineNew.Map{
    public class ChunkRegionRender{
        private readonly ChunkRegion _chunkRegion;
        private readonly ConcurrentDictionary<string, BlockRender> _renders;

        private bool _deleted;


        public ChunkRegionRender(ChunkRegion chunkRegion) {
            _chunkRegion = chunkRegion;

            _renders = new ConcurrentDictionary<string, BlockRender>();


            _deleted = false;
        }

        public ChunkRegion ChunkRegion => _chunkRegion;

        public void AddData(int mapper, Block block, int blockLight, int sunlight) {
            _deleted = false;

            GetOrCreateRender(block.BlockModel)?.AddData(mapper, block, blockLight, sunlight);
        }

        public void RemoveData(int mapper, Block block) {
            _deleted = false;
            GetOrCreateRender(block.BlockModel)?.RemoveData(mapper, block);
        }

        public void Draw() {
            if (_deleted) return;

            foreach (var render in _renders.Values) {
                render.Draw();
            }
        }

        public void DrawAfterPostRender() {
            if (_deleted) return;

            foreach (var render in _renders.Values) {
                render.DrawAfterPostRender();
            }
        }

        public void FlushInBackground() {
            if (_deleted) return;
            foreach (var render in _renders.Values) {
                render.FlushInBackground();
            }
        }

        public void CleanUp() {
            if (_deleted) return;
            _deleted = true;
            foreach (var render in _renders.Values) {
                render.CleanUp();
            }

            _renders.Clear();
        }

        private BlockRender GetOrCreateRender(BlockModel model) {
            if (_deleted) return null;
            var modelId = model.Id;
            if (_renders.TryGetValue(modelId, out var render)) {
                return render;
            }

            render = model.CreateBlockRender(_chunkRegion);
            _renders.TryAdd(modelId, render);
            return render;
        }
    }
}