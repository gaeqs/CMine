using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using CMineNew.Map.BlockData.Type;

namespace CMineNew.Map.BlockData.Static.Type{
    public class BlockStaticDataTorch : BlockStaticData{
        
        public static readonly BlockStaticDataTorch Instance = new BlockStaticDataTorch();
        
        private BlockStaticDataTorch() : 
            base("default:torch", BlockModelManager.GetModelOrNull(TorchBlockModel.Key),
                true, 0.6f, 0, true, 
                Block.MaxBlockLight, 1, 0) {
        }

        public override Area2d GetTextureArea(BlockFace face) {
            return CMine.TextureMap.Areas["default:torch"];
        }
    }
}