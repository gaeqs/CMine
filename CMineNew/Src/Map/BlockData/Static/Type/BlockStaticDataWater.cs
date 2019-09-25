using CMineNew.Map.BlockData.Model;

namespace CMineNew.Map.BlockData.Static.Type{
    public class BlockStaticDataWater : BlockStaticData{
        
        public static readonly BlockStaticDataWater Instance = new BlockStaticDataWater();
        
        private BlockStaticDataWater() : 
            base("default:water", BlockModelManager.GetModelOrNull(WaterBlockModel.Key),
                true, 1, 0, false, 
                0, 2, 2) {
        }
    }
}