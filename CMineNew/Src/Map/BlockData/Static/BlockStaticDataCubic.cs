using CMineNew.Map.BlockData.Model;

namespace CMineNew.Map.BlockData.Static{
    public abstract class BlockStaticDataCubic : BlockStaticData{

        public BlockStaticDataCubic(string id, bool passable, bool lightSource, sbyte lightSourceLight,
            sbyte blockLightPassReduction, sbyte sunlightPassReduction ) : 
            base(id, BlockModelManager.GetModelOrNull(CubicBlockModel.Key), passable, 1, 0, 
                lightSource, lightSourceLight, blockLightPassReduction, sunlightPassReduction) {
        }
    }
}