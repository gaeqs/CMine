using System;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using OpenTK;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Sketch{
    public abstract class CubicBlock : Block{
        private bool[] _visibleFaces;

        public CubicBlock(string id, Chunk chunk, Vector3i position, Color4 textureFilter, bool passable = false)
            : base(id, BlockModelManager.GetModelOrNull(CubicBlockModel.Key), chunk, position, textureFilter, passable) {
            _visibleFaces = new bool[6];
        }

        public override void OnPlace(Block oldBlock, Block[] neighbours, bool triggerWorldUpdates) {
            var render = _chunk.Region.Render;
            for (var i = 0; i < _visibleFaces.Length; i++) {
                var block = neighbours[i];
                _visibleFaces[i] = block == null || !block.IsFaceOpaque(BlockFaceMethods.GetOpposite((BlockFace) i));
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

        public override bool Collides(Vector3 origin, Vector3 direction) {
            return true;
        }

        public override bool IsFaceOpaque(BlockFace face) {
            return true;
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