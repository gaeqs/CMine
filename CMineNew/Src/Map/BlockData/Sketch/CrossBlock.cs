using CMineNew.Color;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Static;
using OpenTK;

namespace CMineNew.Map.BlockData.Sketch{
    public abstract class CrossBlock : Block{
        public CrossBlock(BlockStaticDataCross staticData, Chunk chunk, Vector3i position, Rgba32I textureFilter)
            : base(staticData, chunk, position, textureFilter) {
        }


        public override Vector3 CollisionBoxPosition => _position.ToFloat();
        public abstract Area2d TextureArea { get; }

        public override void OnPlace(Block oldBlock, bool triggerWorldUpdates, bool addToRender) {
            if (!addToRender) return;
            AddToRender();
        }

        public override void AddToRender() {
            World.BlockRender.AddBlock(this);
        }

        public override void OnRemove(Block newBlock) {
        }

        public override void OnNeighbourBlockChange(Block from, Block to, BlockFace relative) {
        }

        public override bool Collides(Vector3 current, Vector3 origin, Vector3 direction) {
            return BlockModel.BlockCollision.CollidesSegment(_position.ToFloat(), current, current + direction * 2);
        }

        public override bool IsFaceOpaque(BlockFace face) {
            return false;
        }

        public override void RemoveFromRender() {
            World.BlockRender.RemoveBlock(this);
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
            World.BlockRender.AddBlock(this);
        }
    }
}