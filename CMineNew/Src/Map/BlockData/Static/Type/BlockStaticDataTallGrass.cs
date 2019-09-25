namespace CMineNew.Map.BlockData.Static.Type{
    public class BlockStaticDataTallGrass : BlockStaticDataTexturedCross{
        public static readonly BlockStaticDataTallGrass Instance = new BlockStaticDataTallGrass();

        private BlockStaticDataTallGrass()
            : base("default:tall_grass", true, false, 0,
                Block.MaxBlockLight, Block.MaxBlockLight, "default:tall_grass") {
        }
    }
}