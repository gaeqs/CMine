using CMineNew.Map.BlockData.Model;

namespace CMineNew.Map.BlockData.Static.Type{
    public class BlockStaticDataTallGrass : BlockStaticDataTexturedCross{
        public static readonly BlockStaticDataTallGrass Instance = new BlockStaticDataTallGrass();

        private BlockStaticDataTallGrass()
            : base("default:tall_grass", true, false, 0,
                1, 0, "default:tall_grass") {
            BlockModel = BlockModelManager.GetModelOrNull(TallGrassBlockModel.Key);
        }
    }
}