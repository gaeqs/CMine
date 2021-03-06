using CMineNew.Map.BlockData.Model;

namespace CMineNew.Map.BlockData.Static{
    public abstract class BlockStaticDataSlab : BlockStaticData{

        public BlockStaticDataSlab(string id, bool passable, bool lightSource, sbyte lightSourceLight,
            sbyte blockLightPassReduction, sbyte sunlightPassReduction ) : 
            base(id, BlockModelManager.GetModelOrNull(SlabBlockModel.Key), passable, 0.5f, 0, 
                lightSource, lightSourceLight, blockLightPassReduction, sunlightPassReduction) {
        }
    }
}