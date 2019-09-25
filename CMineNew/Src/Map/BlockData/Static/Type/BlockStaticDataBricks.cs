namespace CMineNew.Map.BlockData.Static.Type{
    public class BlockStaticDataBricks : BlockStaticDataTexturedCubic{
        
        public static readonly BlockStaticDataBricks Instance = new BlockStaticDataBricks();

        private BlockStaticDataBricks() 
            : base("default:bricks", false, false, 0, 
                Block.MaxBlockLight, Block.MaxBlockLight, "default:bricks") {
        }
    }
}