using CMineNew.Map.BlockData.Model;

namespace CMineNew.Map.BlockData.Static{
    public class BlockStaticDataCross : BlockStaticData{
        public BlockStaticDataCross(string id, bool passable, bool lightSource, sbyte lightSourceLight,
            sbyte blockLightPassReduction, sbyte sunlightPassReduction) :
            base(id, BlockModelManager.GetModelOrNull(CrossBlockModel.Key), passable, 1, 0,
                lightSource, lightSourceLight, blockLightPassReduction, sunlightPassReduction) {
        }
    }
}