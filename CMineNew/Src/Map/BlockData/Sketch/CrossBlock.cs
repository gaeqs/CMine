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
        public abstract int TextureIndex { get; }

        public override void OnPlace(Block oldBlock, bool triggerWorldUpdates, bool addToRender) {
            if (!addToRender) return;
            AddToRender();
        }

        public override void AddToRender() {
            var render = _chunk.Region.Render;
            render.AddData(0, this, _blockLight.Light, _blockLight.Sunlight);
        }

        public override void OnRemove(Block newBlock) {
            if (BlockModel.Id == newBlock.BlockModel?.Id) return;
            if (_chunk.Region.Deleted) return;
            var render = _chunk.Region.Render;
            render.RemoveData(0, this);
        }

        public override void OnNeighbourBlockChange(Block from, Block to, BlockFace relative) {
        }

        public override bool Collides(BlockFace fromFace, Vector3 current, Vector3 origin, Vector3 direction, 
            out BlockFace face, out Vector3 collision) {
            return BlockModel.BlockCollision.CollidesSegment(_position.ToFloat(), current, current + direction * 2,
                out collision, out face);
        }

        public override bool IsFaceOpaque(BlockFace face) {
            return false;
        }

        public override void RemoveFromRender() {
            if (_chunk.Region.Deleted) return;
            _chunk.Region.Render.RemoveData(0, this);
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
            _chunk.Region.Render.AddData(0, this, _blockLight.Light, _blockLight.Sunlight);
        }
    }
}