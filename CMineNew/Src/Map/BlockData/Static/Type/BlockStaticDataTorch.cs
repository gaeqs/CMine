using CMineNew.Map.BlockData.Model;

namespace CMineNew.Map.BlockData.Static.Type{
    public class BlockStaticDataTorch : BlockStaticData{
        
        public static BlockStaticDataTorch Instance = new BlockStaticDataTorch();
        
        private BlockStaticDataTorch() : 
            base("default:torch", BlockModelManager.GetModelOrNull(TorchBlockModel.Key),
                true, 0.6f, 0, true, 
                Block.MaxBlockLight, 1, 0) {
        }
    }
}