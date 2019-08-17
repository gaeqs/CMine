using CMineNew.Map;

namespace CMine.Collision{
    public class CollisionData{
        private BlockFace _blockFace;
        private float _distance;

        public CollisionData(BlockFace blockFace, float distance) {
            _blockFace = blockFace;
            _distance = distance;
        }

        public BlockFace BlockFace => _blockFace;

        public float Distance => _distance;
    }
}