using CMineNew.Color;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Static.Type;
using OpenTK;

namespace CMineNew.Map.BlockData.Type{
    public class BlockAir : Block{
        public BlockAir(Chunk chunk, Vector3i position)
            : base(BlockStaticDataAir.Instance, chunk, position, new Rgba32I(0, 0, 0, 0)) {
        }

        public override Vector3 CollisionBoxPosition => _position.ToFloat();

        public override void OnPlace(Block oldBlock, bool triggerWorldUpdates, bool addToRender) {
        }

        public override void OnRemove(Block newBlock) {
        }

        public override void OnNeighbourBlockChange(Block from, Block to, BlockFace relative) {
        }

        public override void AddToRender() {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockAir(chunk, position);
        }

        public override bool Collides(BlockFace fromFace, Vector3 current, Vector3 origin, Vector3 direction,
            out BlockFace face, out Vector3 collision) {
            face = fromFace;
            collision = origin;
            return false;
        }

        public override bool IsFaceOpaque(BlockFace face) {
            return false;
        }

        public override void RemoveFromRender() {
        }

        public override bool CanLightPassThrough(BlockFace face) {
            return true;
        }

        public override bool CanLightBePassedFrom(BlockFace face, Block from) {
            return true;
        }

        public override void OnNeighbourLightChange(BlockFace relative, Block block) {
        }

        public override void OnSelfLightChange() {
        }
    }
}