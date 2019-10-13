using System.Collections.Generic;

namespace CMineNew.Map.BlockData.Model{
    public class BlockModelManager{
        private static Dictionary<string, BlockModel> _models = new Dictionary<string, BlockModel>();
        private static bool _loaded;


        public static void Load() {
            if (_loaded) return;
            _models.Add(CubicBlockModel.Key, new CubicBlockModel());
            _models.Add(CrossBlockModel.Key, new CrossBlockModel());
            _models.Add(WaterBlockModel.Key, new WaterBlockModel());
            _models.Add(SlabBlockModel.Key, new SlabBlockModel());
            _models.Add(TorchBlockModel.Key, new TorchBlockModel());
            _loaded = true;
        }

        public static Dictionary<string, BlockModel> Models => _models;

        public static void AddModel(string key, BlockModel model) {
            if (!_loaded) {
                Load();
            }

            _models.Add(key, model);
        }

        public static bool TryGetModel(string key, out BlockModel model) {
            if (!_loaded) {
                Load();
            }

            return _models.TryGetValue(key, out model);
        }

        public static BlockModel GetModelOrNull(string key) {
            return TryGetModel(key, out var model) ? model : null;
        }
    }
}