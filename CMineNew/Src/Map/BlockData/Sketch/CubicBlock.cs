using System;
using CMineNew.Color;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Static;
using OpenTK;

namespace CMineNew.Map.BlockData.Sketch{
    public abstract class CubicBlock : Block{
        private const int Faces = 6;

        private byte _visibleFaces;

        public CubicBlock(BlockStaticDataCubic staticData, Chunk chunk, Vector3i position, Rgba32I textureFilter)
            : base(staticData, chunk, position, textureFilter) {
            _visibleFaces = 0;
        }

        public override Vector3 CollisionBoxPosition => _position.ToFloat();

        public bool IsFaceVisible(BlockFace face) {
            return IsFaceVisible((int) face);
        }

        public bool IsFaceVisible(int face) {
            var value = _visibleFaces;
            value >>= face;
            value &= 0x1;
            return value != 0;
        }

        private bool SetFaceVisible(BlockFace face, bool visible) {
            return SetFaceVisible((int) face, visible);
        }

        private bool SetFaceVisible(int face, bool visible) {
            byte del = (byte) ~(0x1 << face);
            _visibleFaces &= del;
            var value = (byte) (visible ? 1 : 0);
            value <<= face;
            _visibleFaces |= value;
            return visible;
        }


        public override void OnPlace(Block oldBlock, bool triggerWorldUpdates, bool addToRender) {
            var render = _chunk.Region.Render;
            for (var i = 0; i < Faces; i++) {
                var block = GetNeighbour((BlockFace) i);
                var oppositeFace = BlockFaceMethods.GetOpposite((BlockFace) i);
                var vis = SetFaceVisible(i, block == null || !block.IsFaceOpaque(oppositeFace));
                if (!vis || !addToRender) continue;
                var light = block?.BlockLight;
                render.AddData(i, this, light?.Light ?? 0, light?.Sunlight ?? 0);
            }
        }

        public override void AddToRender() {
            var render = _chunk.Region.Render;
            for (var i = 0; i < Faces; i++) {
                if (!IsFaceVisible(i)) continue;
                var block = GetNeighbour((BlockFace) i);
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
            var oldVisible = IsFaceVisible(faceInt);
            var newVisible = !to.IsFaceOpaque(BlockFaceMethods.GetOpposite(relative));
            if (oldVisible == newVisible) return;
            SetFaceVisible(faceInt, newVisible);
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

        public abstract int GetTextureIndex(BlockFace face);

        public override void RemoveFromRender() {
            if (_chunk.Region.Deleted) return;
            var render = _chunk.Region.Render;
            ForEachVisibleFaceInt(face => render.RemoveData(face, this));
        }

        public void ForEachVisibleFace(Action<BlockFace> action) {
            for (var i = 0; i < Faces; i++) {
                if (IsFaceVisible(i)) {
                    action.Invoke((BlockFace) i);
                }
            }
        }

        public void ForEachVisibleFaceInt(Action<int> action) {
            for (var i = 0; i < Faces; i++) {
                if (IsFaceVisible(i)) {
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
            if (IsFaceVisible(relative)) {
                _chunk.Region.Render.AddData((int) relative,
                    this, block.BlockLight.Light, block.BlockLight.Sunlight);
            }
        }

        public override void OnSelfLightChange() {
        }
    }
}