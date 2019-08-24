using System;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using OpenTK;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Sketch{
    public abstract class SlabBlock : Block{
        public const float SlabHeight = 0.5f;

        private bool[] _visibleFaces;

        public SlabBlock(string id, Chunk chunk, Vector3i position, Color4 textureFilter, bool passable = false)
            : base(id, BlockModelManager.GetModelOrNull(SlabBlockModel.Key), chunk, position, textureFilter,
                passable, SlabHeight) {
            _visibleFaces = new bool[6];
        }

        public override void OnPlace(Block oldBlock, Block[] neighbours, bool triggerWorldUpdates) {
            var render = _chunk.Region.Render;
            for (var i = 0; i < _visibleFaces.Length; i++) {
                var block = neighbours[i];
                _visibleFaces[i] = i == (int) BlockFace.Up || block == null ||
                                   !block.IsFaceOpaque(BlockFaceMethods.GetOpposite((BlockFace) i));
                if (_visibleFaces[i]) {
                    render.AddData(i, this);
                }
                else {
                    render.RemoveData(i, this);
                }
            }
        }

        public override void OnRemove(Block newBlock) {
            if (_blockModel.Id == newBlock.BlockModel?.Id) return;
            if (_chunk.Region.Deleted) return;
            var render = _chunk.Region.Render;
            ForEachVisibleFaceInt(face => render.RemoveData(face, this));
        }

        public override void OnNeighbourBlockChange(Block from, Block to, BlockFace relative) {
            if (relative == BlockFace.Up) return;
            var faceInt = (int) relative;
            var oldVisible = _visibleFaces[faceInt];
            var newVisible = to == null || !to.IsFaceOpaque(BlockFaceMethods.GetOpposite(relative));
            if (oldVisible == newVisible) return;
            _visibleFaces[faceInt] = newVisible;
            if (newVisible) {
                _chunk.Region.Render.AddData(faceInt, this);
            }
            else {
                _chunk.Region.Render.RemoveData(faceInt, this);
            }
        }

        public override bool Collides(Vector3 current, Vector3 origin, Vector3 direction) {
            return _blockModel.BlockCollision.CollidesSegment(_position.ToFloat(), current, current + direction * 2);
        }

        public override bool IsFaceOpaque(BlockFace face) {
            return face == BlockFace.Down;
        }

        public abstract Area2d GetTextureArea(BlockFace face);

        public override void RemoveFromRender() {
            if (_chunk.Region.Deleted) return;
            var render = _chunk.Region.Render;
            ForEachVisibleFaceInt(face => render.RemoveData(face, this));
        }

        public void ForEachVisibleFace(Action<BlockFace> action) {
            for (var i = 0; i < _visibleFaces.Length; i++) {
                if (_visibleFaces[i]) {
                    action.Invoke((BlockFace) i);
                }
            }
        }

        public void ForEachVisibleFaceInt(Action<int> action) {
            for (var i = 0; i < _visibleFaces.Length; i++) {
                if (_visibleFaces[i]) {
                    action.Invoke(i);
                }
            }
        }
    }
}