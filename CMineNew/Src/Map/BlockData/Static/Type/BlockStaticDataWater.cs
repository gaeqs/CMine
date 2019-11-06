using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using CMineNew.Map.BlockData.Render;
using CMineNew.Map.BlockData.Type;

namespace CMineNew.Map.BlockData.Static.Type{
    public class BlockStaticDataWater : BlockStaticData{
        
        public static readonly BlockStaticDataWater Instance = new BlockStaticDataWater();
        
        private BlockStaticDataWater() : 
            base("default:water", BlockModelManager.GetModelOrNull(WaterBlockModel.Key),
                true, 1, 0, false, 
                0, 2, 2) {
        }

        public override Area2d GetTextureArea(BlockFace face) {
            return BlockWater.TextureArea;
        }
    }
}