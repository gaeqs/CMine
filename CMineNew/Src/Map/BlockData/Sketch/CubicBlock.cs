using System;
using System.Linq;
using CMineNew.Color;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Static;
using OpenTK;

namespace CMineNew.Map.BlockData.Sketch{
    public abstract class CubicBlock : Block{
        private readonly bool[] _visibleFaces;

        public CubicBlock(BlockStaticDataCubic staticData, Chunk chunk, Vector3i position, Rgba32I textureFilter)
            : base(staticData, chunk, position, textureFilter) {
            _visibleFaces = new bool[6];
        }

        public override Vector3 CollisionBoxPosition => _position.ToFloat();

        public override void OnPlace(Block oldBlock, bool triggerWorldUpdates, bool addToRender) {
            //Calculate visible faces
            for (var i = 0; i < _visibleFaces.Length; i++) {
                var block = GetNeighbour((BlockFace) i);
                var oppositeFace = BlockFaceMethods.GetOpposite((BlockFace) i);
                _visibleFaces[i] = block == null || !block.IsFaceOpaque(oppositeFace);
            }

            if (!addToRender) return;
            World.BlockRender.AddBlock(this);
        }

        public override void AddToRender() {
            World.BlockRender.AddBlock(this);
        }

        public override void OnRemove(Block newBlock) {
        }

        public override void OnNeighbourBlockChange(Block from, Block to, BlockFace relative) {
            var faceInt = (int) relative;
            var oldVisible = _visibleFaces[faceInt];
            var newVisible = !to.IsFaceOpaque(BlockFaceMethods.GetOpposite(relative));
            if (oldVisible == newVisible) return;
            _visibleFaces[faceInt] = newVisible;

            World.BlockRender.AddBlock(this);
        }

        public override bool Collides(Vector3 current, Vector3 origin, Vector3 direction) {
            return true;
        }

        public override bool IsFaceOpaque(BlockFace face) {
            return true;
        }

        public abstract Area2d GetTextureArea(BlockFace face);

        public override void RemoveFromRender() {
            World.BlockRender.RemoveBlock(this);
        }

        public bool IsFaceVisible(BlockFace face) {
            return _visibleFaces[(int) face];
        }

        public bool IsFaceVisible(int face) {
            return _visibleFaces[face];
        }

        public int GetVisibleFacesCount() {
            return _visibleFaces.Count(b => b);
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
                World.BlockRender.AddBlock(this);
            }
        }

        public override void OnSelfLightChange() {
        }
    }
}