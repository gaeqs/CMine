using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using OpenTK;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Sketch{
    public abstract class CrossBlock : Block{
        public CrossBlock(string id, Chunk chunk, Vector3i position, Color4 textureFilter, bool passable = false)
            : base(id, BlockModelManager.GetModelOrNull(CrossBlockModel.Key), chunk, position, textureFilter,
                passable) {
        }


        public override Vector3 CollisionBoxPosition => _position.ToFloat();
        public abstract Area2d TextureArea { get; }

        public override void OnPlace(Block oldBlock, Block[] neighbours, bool triggerWorldUpdates) {
            var render = _chunk.Region.Render;
            render.AddData(0, this, _blockLight);
        }

        public override void OnRemove(Block newBlock) {
            if (_blockModel.Id == newBlock.BlockModel?.Id) return;
            if (_chunk.Region.Deleted) return;
            var render = _chunk.Region.Render;
            render.RemoveData(0, this);
        }

        public override void OnNeighbourBlockChange(Block from, Block to, BlockFace relative) {
        }

        public override bool Collides(Vector3 current, Vector3 origin, Vector3 direction) {
            return _blockModel.BlockCollision.CollidesSegment(_position.ToFloat(), current, current + direction * 2);
        }

        public override bool IsFaceOpaque(BlockFace face) {
            return false;
        }

        public override void RemoveFromRender() {
            if (_chunk.Region.Deleted) return;
            _chunk.Region.Render.RemoveData(0, this);
        }

        public override void OnLightChange(BlockFace from, Block fromBlock, int light, Vector3i source) {
            if (light <= _blockLight) return;
            _blockLight = light;
            _blockLightSource = source;
            light -= _blockLightReduction;

            foreach (var blockFace in BlockFaceMethods.All) {
                _chunk.Region.Render.AddData((int) blockFace, this, _blockLight);
            }

            var blocks = _chunk.GetNeighbourBlocks(new Block[6], _position,
                _position - (_chunk.Position << Chunk.WorldPositionShift));
            for (var i = 0; i < blocks.Length; i++) {
                var face = (BlockFace) i;
                var opposite = BlockFaceMethods.GetOpposite(face);
                var block = blocks[i];
                block?.OnNeighbourLightChange(opposite, this, _blockLight, source);
                if (light > 0) {
                    block?.OnLightChange(opposite, this, light, source);
                }
            }
        }

        public override void OnNeighbourLightChange(BlockFace relative, Block block, int light, Vector3i source) {
        }
    }
}