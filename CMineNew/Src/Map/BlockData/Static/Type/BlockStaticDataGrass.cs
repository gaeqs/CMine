namespace CMineNew.Map.BlockData.Static.Type{
    public class BlockStaticDataGrass : BlockStaticDataMultiTexturedCubic{
        
        public static BlockStaticDataGrass Instance = new BlockStaticDataGrass();

        private BlockStaticDataGrass() 
            : base("default:grass", false, false, 0, 
                Block.MaxBlockLight, Block.MaxBlockLight, 
                new [] {"default:sand"}) {
        }
    }
}