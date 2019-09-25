using CMineNew.Map.BlockData.Model;

namespace CMineNew.Map.BlockData.Static{
    public class BlockStaticDataCubic : BlockStaticData{

        public BlockStaticDataCubic(string id, bool passable, bool lightSource, int lightSourceLight,
            int blockLightPassReduction, int sunlightPassReduction ) : 
            base(id, BlockModelManager.GetModelOrNull(CubicBlockModel.Key), passable, 1, 0, 
                lightSource, lightSourceLight, blockLightPassReduction, sunlightPassReduction) {
        }
    }
}