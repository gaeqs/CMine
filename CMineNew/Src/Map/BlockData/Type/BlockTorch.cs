using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using OpenTK;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Type{
    public class BlockTorch : Block{
        private Area2d _textureArea;

        public BlockTorch(Chunk chunk, Vector3i position)
            : base("default:torch", BlockModelManager.GetModelOrNull(TorchBlockModel.Key),
                chunk, position, Color4.Transparent, true, 0.6f, 0, true,
                MaxBlockLight) {
            _textureArea = CMine.Textures.Areas["default:torch"];
        }

        public override Vector3 CollisionBoxPosition => _position.ToFloat();

        public Area2d TextureArea => _textureArea;

        public override void OnPlace(Block oldBlock, Block[] neighbours, bool triggerWorldUpdates) {
            var render = _chunk.Region.Render;
            render.AddData(0, this, _blockLight.Light);
        }

        public override void OnRemove(Block newBlock) {
            if (_blockModel.Id == newBlock.BlockModel?.Id) return;
            if (_chunk.Region.Deleted) return;
            var render = _chunk.Region.Render;
            render.RemoveData(0, this);
        }

        public override void OnNeighbourBlockChange(Block @from, Block to, BlockFace relative) {
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockGrass(chunk, position);
        }

        public override bool Collides(Vector3 current, Vector3 origin, Vector3 direction) {
            return _blockModel.BlockCollision.CollidesSegment(_position.ToFloat(), current, current + direction * 2);
        }

        public override bool IsFaceOpaque(BlockFace face) {
            return false;
        }

        public override void RemoveFromRender() {
            if (_chunk.Region.Deleted) return;
            var render = _chunk.Region.Render;
            render.RemoveData(0, this);
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
            _chunk.Region.Render.AddData(0, this, _blockLight.Light);
        }
    }
}