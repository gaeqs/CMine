namespace CMineNew.Map.BlockData.Static.Type{
    public class BlockStaticDataOakLeaves : BlockStaticDataTexturedCubic{
        
        public static readonly BlockStaticDataOakLeaves Instance = new BlockStaticDataOakLeaves();

        private BlockStaticDataOakLeaves() 
            : base("default:oak_leaves", false, false, 0, 
                1, 1, "default:oak_leaves") {
        }
    }
}