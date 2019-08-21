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

        public abstract Area2d TextureArea { get; }

        public override void OnPlace(Block oldBlock, Block[] neighbours) {
            var render = _chunk.Region.Render;
            render.AddData(0, this);
        }

        public override void OnRemove(Block newBlock) {
            if (_blockModel.Id == newBlock.BlockModel?.Id) return;
            if (_chunk.Region.Deleted) return;
            var render = _chunk.Region.Render;
            render.RemoveData(0, this);
        }

        public override void OnNeighbourBlockChange(Block from, Block to, BlockFace relative) {
        }

        public override bool Collides(Vector3 origin, Vector3 direction) {
            //TODO
            return true;
        }

        public override bool IsFaceOpaque(BlockFace face) {
            return false;
        }

        public override void RemoveFromRender() {
            if (_chunk.Region.Deleted) return;
            _chunk.Region.Render.RemoveData(0, this);
        }
    }
}