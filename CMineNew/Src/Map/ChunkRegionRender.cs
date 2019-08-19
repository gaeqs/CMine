using System;
using System.Collections.Generic;
using CMineNew.Map.BlockData.Model;
using CMineNew.Map.BlockData.Render;

namespace CMineNew.Map.BlockData{
    public class ChunkRegionRender{
        private readonly ChunkRegion _chunkRegion;
        private readonly Dictionary<string, BlockRender> _renders;


        public ChunkRegionRender(ChunkRegion chunkRegion) {
            _chunkRegion = chunkRegion;
            _renders = new Dictionary<string, BlockRender>();
        }

        public ChunkRegion ChunkRegion => _chunkRegion;

        public void AddData(int mapper, Block block) {
            GetOrCreateRender(block.BlockModel).AddData(mapper, block);
        }

        public void RemoveData(int mapper, Block block) {
            GetOrCreateRender(block.BlockModel).RemoveData(mapper, block);
        }
        
        public void Draw() {
            foreach (var render in _renders.Values) {
                render.Draw();
            }
        }

        public void DrawAfterPostRender() {
            foreach (var render in _renders.Values) {
                render.DrawAfterPostRender();
            }
        }

        public void FlushInBackground() {
            foreach (var render in _renders.Values) {
                render.FlushInBackground();
            }
        }

        public void CleanUp() {
            foreach (var render in _renders.Values) {
                render.CleanUp();
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