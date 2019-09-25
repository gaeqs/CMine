namespace CMineNew.Map.BlockData.Static.Type{
    public class BlockStaticDataStone : BlockStaticDataTexturedCubic{
        public static readonly BlockStaticDataStone Instance = new BlockStaticDataStone();

        private BlockStaticDataStone()
            : base("default:stone", false, false, 0,
                Block.MaxBlockLight, Block.MaxBlockLight, "default:stone") {
        }
    }
}