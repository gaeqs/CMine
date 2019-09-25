using CMineNew.Map.BlockData.Model;

namespace CMineNew.Map.BlockData.Static{
    public class BlockStaticDataCross : BlockStaticData{
        public BlockStaticDataCross(string id, bool passable, bool lightSource, int lightSourceLight,
            int blockLightPassReduction, int sunlightPassReduction) :
            base(id, BlockModelManager.GetModelOrNull(CrossBlockModel.Key), passable, 1, 0,
                lightSource, lightSourceLight, blockLightPassReduction, sunlightPassReduction) {
        }
    }
}