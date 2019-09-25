namespace CMineNew.Map.BlockData.Static.Type{
    public class BlockStaticDataDirt : BlockStaticDataTexturedCubic{
        
        public static BlockStaticDataDirt Instance = new BlockStaticDataDirt();

        private BlockStaticDataDirt() 
            : base("default:dirt", false, false, 0, 
                Block.MaxBlockLight, Block.MaxBlockLight, "string:dirt") {
        }
    }
}