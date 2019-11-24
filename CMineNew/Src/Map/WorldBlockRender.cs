using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using CMineNew.Map.BlockData;
using CMineNew.Map.BlockData.Model;
using CMineNew.Render.Mapper;

namespace CMineNew.Map{
    public class WorldBlockRender{
        private readonly ConcurrentDictionary<BlockModel, WorldVboMapper> _mappers;
        private readonly Collection<BlockModel> _notConfiguredModels;

        public WorldBlockRender() {
            _mappers = new ConcurrentDictionary<BlockModel, WorldVboMapper>();
            _notConfiguredModels = new Collection<BlockModel>();
        }


        public void AddBlock(Block block) {
            var model = block.BlockModel;
            if (!_mappers.TryGetValue(block.BlockModel, out var value)) {
                if (model != null) {
                    value = new WorldVboMapper(model.FloatsPerBlock, model, model.DefaultVboBlocks);
                    _mappers.TryAdd(model, value);
                    _notConfiguredModels.Add(model);
                }
            }

            foreach (var mapper in _mappers.Values.Where(mapper => mapper.Model != model)) {
                mapper.RemoveBlock(block);
            }

            if (model != null) value.AddBlock(block);
        }

        public void RemoveBlock(Block block) {
            if (!_mappers.TryGetValue(block.BlockModel, out var value)) return;
            value.RemoveBlock(block);
        }

        public void Draw() {
            foreach (var model in _mappers.Keys) {
                var mapper = _mappers[model];
                mapper.Update();
                var configured = !_notConfiguredModels.Contains(model);
                if (!configured && mapper.VboCreated) {
                    model.SetupVbo(mapper.VertexBufferObject);
                    configured = true;
                    _notConfiguredModels.Remove(model);
                }

                if (configured) {
                    model.Draw((int) mapper.MaxBlocks);
                }
            }
        }

        public void DrawAfterPostRender() {
            foreach (var model in _mappers.Keys) {
                if (!_notConfiguredModels.Contains(model)) {
                    model.DrawAfterPostRender((int) _mappers[model].MaxBlocks);
                }
            }
        }
    }
}