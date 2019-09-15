using System;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using OpenTK;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Sketch{
    public abstract class CrossBlock : Block{
        public CrossBlock(string id, Chunk chunk, Vector3i position, Color4 textureFilter, bool passable = false,
            bool lightSource = false, int blockLightPassReduction = 1, int sunlightPassReduction = 0)
            : base(id, BlockModelManager.GetModelOrNull(CrossBlockModel.Key), chunk, position, textureFilter,
                passable, 1, 0, lightSource, blockLightPassReduction, sunlightPassReduction) {
        }


        public override Vector3 CollisionBoxPosition => _position.ToFloat();
        public abstract Area2d TextureArea { get; }

        public override void OnPlace(Block oldBlock, Block[] neighbours, bool triggerWorldUpdates) {
            var render = _chunk.Region.Render;
            render.AddData(0, this, _blockLight.Light, _blockLight.Sunlight);
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