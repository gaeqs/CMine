using CMineNew.Map.BlockData.Model;

namespace CMineNew.Map.BlockData.Static{
    public class BlockStaticDataSlab : BlockStaticData{

        public BlockStaticDataSlab(string id, bool passable, bool lightSource, int lightSourceLight,
            int blockLightPassReduction, int sunlightPassReduction ) : 
            base(id, BlockModelManager.GetModelOrNull(CubicBlockModel.Key), passable, 0.5f, 0, 
                lightSource, lightSourceLight, blockLightPassReduction, sunlightPassReduction) {
        }
    }
}