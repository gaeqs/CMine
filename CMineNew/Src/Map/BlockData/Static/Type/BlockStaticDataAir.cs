namespace CMineNew.Map.BlockData.Static.Type{
    public class BlockStaticDataAir : BlockStaticData{
        public static BlockStaticDataAir Instance = new BlockStaticDataAir();

        private BlockStaticDataAir() :
            base("default:air", null, true, 1, 0,
                false, 0, 1, 0) {
        }
    }
}