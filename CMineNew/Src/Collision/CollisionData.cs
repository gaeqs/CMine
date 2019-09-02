using CMineNew.Map;

namespace CMineNew.Collision{
    /// <summary>
    /// Represents the result of a collision test if a collision was detected.
    /// </summary>
    public class CollisionData{
        
        private BlockFace _blockFace;
        private float _distance;

        /// <summary>
        /// Creates a collision data using a penetrated block face and the penetration distance.
        /// </summary>
        /// <param name="blockFace">the block face</param>
        /// <param name="distance">the distance</param>
        public CollisionData(BlockFace blockFace, float distance) {
            _blockFace = blockFace;
            _distance = distance;
        }

        /// <summary>
        /// The penetrated block face.
        /// </summary>
        public BlockFace BlockFace => _blockFace;

        /// <summary>
        /// The penetration distance.
        /// </summary>
        public float Distance => _distance;
    }
}