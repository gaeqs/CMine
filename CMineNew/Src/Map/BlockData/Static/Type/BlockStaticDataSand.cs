namespace CMineNew.Map.BlockData.Static.Type{
    public class BlockStaticDataSand : BlockStaticDataTexturedCubic{
        
        public static BlockStaticDataSand Instance = new BlockStaticDataSand();

        private BlockStaticDataSand() 
            : base("default:sand", false, false, 0, 
                Block.MaxBlockLight, Block.MaxBlockLight, "default:sand") {
        }
    }
}