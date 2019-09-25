namespace CMineNew.Map.BlockData.Static.Type{
    public class BlockStaticDataOakLog : BlockStaticDataMultiTexturedCubic{
        
        public static readonly BlockStaticDataOakLog Instance = new BlockStaticDataOakLog();

        private BlockStaticDataOakLog() 
            : base("default:oak_log", false, false, 0, 
                Block.MaxBlockLight, Block.MaxBlockLight, 
                new [] {"default:oak_log_top", "default:oak_log_top", "default:oak_log_side",
                    "default:oak_log_side", "default:oak_log_side", "default:oak_log_side"}) {
        }
    }
}