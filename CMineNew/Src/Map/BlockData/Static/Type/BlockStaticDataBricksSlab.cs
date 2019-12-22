namespace CMineNew.Map.BlockData.Static.Type{
    public class BlockStaticDataBricksSlab : BlockStaticDataTexturedSlab{
        public static readonly BlockStaticDataBricksSlab Instance = new BlockStaticDataBricksSlab();

        private BlockStaticDataBricksSlab()
            : base("default:bricks_slab", false, false, 0,
                1, Block.MaxBlockLight, "default:bricks") {
        }
    }
}