using CMineNew.Geometry;

namespace CMineNew.Map.BlockData.Static.Type {
    public class BlockStaticDataAir : BlockStaticData {
        public static readonly BlockStaticDataAir Instance = new BlockStaticDataAir();

        private BlockStaticDataAir() :
            base("default:air", null, true, 1, 0,
                false, 0, 1, 0) {
        }

        public override int GetTextureIndex(BlockFace face) {
            return 0;
        }
    }
}