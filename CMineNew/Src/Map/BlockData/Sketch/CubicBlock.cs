using System;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using CMineNew.Map.BlockData.Static;
using OpenTK;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Sketch{
    public abstract class CubicBlock : Block{
        private readonly bool[] _visibleFaces;

        public CubicBlock(BlockStaticDataCubic staticData, Chunk chunk, Vector3i position, Color4 textureFilter)
            : base(staticData, chunk, position, textureFilter) {
            _visibleFaces = new bool[6];
        }

        public override Vector3 CollisionBoxPosition => _position.ToFloat();

        public override void OnPlace(Block oldBlock, Block[] neighbours, bool triggerWorldUpdates, bool addToRender) {
            var render = _chunk.Region.Render;
            for (var i = 0; i < _visibleFaces.Length; i++) {
                var block = neighbours[i];
                var oppositeFace = BlockFaceMethods.GetOpposite((BlockFace) i);
                var vis = _visibleFaces[i] = block == null || !block.IsFaceOpaque(oppositeFace);
                if (!vis || !addToRender) continue;
                var light = block?.BlockLight;
                render.AddData(i, this, light?.Light ?? 0, light?.Sunlight ?? 0);
            }
        }

        public override void AddToRender() {
            var render = _chunk.Region.Render;
            for (var i = 0; i < _visibleFaces.Length; i++) {
                if (!_visibleFaces[i]) continue;
                var block = _neighbours[i];
                var light = block?.BlockLight;
                render.AddData(i, this, light?.Light ?? 0, light?.Sunlight ?? 0);
            }
        }

        public override void OnRemove(Block newBlock) {
            if (BlockModel.Id == newBlock.BlockModel?.Id) return;
            if (_chunk.Region.Deleted) return;
            var render = _chunk.Region.Render;
            ForEachVisibleFaceInt(face => render.RemoveData(face, this));
        }

        public override void OnNeighbourBlockChange(Block from, Block to, BlockFace relative) {
            var faceInt = (int) relative;
            var oldVisible = _visibleFaces[faceInt];
            var newVisible = !to.IsFaceOpaque(BlockFaceMethods.GetOpposite(relative));
            if (oldVisible == newVisible) return;
            _visibleFaces[faceInt] = newVisible;
            if (newVisible) {
                _chunk.Region.Render.AddData(faceInt, this, to.BlockLight.Light, to.BlockLight.Sunlight);
            }
            else {
                _chunk.Region.Render.RemoveData(faceInt, this);
            }
        }

        public override bool Collides(Vector3 current, Vector3 origin, Vector3 direction) {
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

        public override bool CanLightPassThrough(BlockFace face) {
            return _blockLightSource != null || !IsFaceOpaque(face);
        }

        public override bool CanLightBePassedFrom(BlockFace face, Block from) {
            return true;
        }

        public override void OnNeighbourLightChange(BlockFace relative, Block block) {
            if (_visibleFaces[(int) relative]) {
                _chunk.Region.Render.AddData((int) relative, 
                    this, block.BlockLight.Light, block.BlockLight.Sunlight);
            }
        }

        public override void OnSelfLightChange() {
        }
    }
}