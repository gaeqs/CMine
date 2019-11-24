using CMineNew.Color;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Static.Type;
using OpenTK;

namespace CMineNew.Map.BlockData.Type{
    public class BlockTorch : Block{
        private Area2d _textureArea;

        public BlockTorch(Chunk chunk, Vector3i position)
            : base(BlockStaticDataTorch.Instance, chunk, position, new Rgba32I(0, 0, 0, 0)) {
            _textureArea = CMine.TextureMap.Areas["default:torch"];
        }

        public override Vector3 CollisionBoxPosition => _position.ToFloat();

        public Area2d TextureArea => _textureArea;

        public override void OnPlace(Block oldBlock, bool triggerWorldUpdates, bool addToRender) {
            if(!addToRender) return;
            AddToRender();
        }

        public override void OnRemove(Block newBlock) {
        }

        public override void OnNeighbourBlockChange(Block @from, Block to, BlockFace relative) {
        }

        public override void AddToRender() {
            World.BlockRender.AddBlock(this);
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockGrass(chunk, position);
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

        public override bool CanLightBePassedFrom(BlockFace face, Block @from) {
            return true;
        }

        public override void OnNeighbourLightChange(BlockFace relative, Block block) {
        }

        public override void OnSelfLightChange() {
            World.BlockRender.AddBlock(this);
        }
    }
}