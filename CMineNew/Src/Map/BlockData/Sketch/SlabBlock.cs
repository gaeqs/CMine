using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CMineNew.Color;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Static;
using OpenTK;

namespace CMineNew.Map.BlockData.Sketch{
    public abstract class SlabBlock : Block{
        private const int Faces = 6;
        public const float SlabHeight = 0.5f;

        protected bool _upside;
        private byte _visibleFaces;

        public SlabBlock(BlockStaticDataSlab staticData, Chunk chunk, Vector3i position, Rgba32I textureFilter,
            bool upside)
            : base(staticData, chunk, position, textureFilter) {
            _upside = upside;
            _visibleFaces = 0;
        }

        public override Vector3 CollisionBoxPosition => _position.ToFloat() + new Vector3(0, _upside ? 0.5f : 0, 0);


        public bool Upside => _upside;

        public override float BlockHeight => base.BlockHeight + (_upside ? 0.5f : 0);

        public override float BlockYOffset => base.BlockYOffset + (_upside ? 0.5f : 0);


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
                var face = (BlockFace) i;

                var visibleBySlab = face == BlockFace.Up && !_upside || face == BlockFace.Down && _upside;

                SetFaceVisible(i, visibleBySlab || block == null ||
                                  !block.IsFaceOpaque(BlockFaceMethods.GetOpposite((BlockFace) i)));
                if (!addToRender || !IsFaceVisible(i)) continue;
                if (visibleBySlab) {
                    render.AddData(i, this, _blockLight.Light, _blockLight.Sunlight);
                }
                else {
                    render.AddData(i, this, block?.BlockLight.Light ?? 0,
                        block?.BlockLight.Sunlight ?? 0);
                }
            }
        }

        public override void AddToRender() {
            var render = _chunk.Region.Render;
            for (var i = 0; i < Faces; i++) {
                if (!IsFaceVisible(i)) continue;
                var face = (BlockFace) i;
                var visibleBySlab = face == BlockFace.Up && !_upside || face == BlockFace.Down && _upside;
                var block = GetNeighbour((BlockFace) i);
                if (visibleBySlab) {
                    render.AddData(i, this, _blockLight.Light, _blockLight.Sunlight);
                }
                else {
                    render.AddData(i, this, block?.BlockLight.Light ?? 0,
                        block?.BlockLight.Sunlight ?? 0);
                }
            }
        }

        public override void OnRemove(Block newBlock) {
            if (BlockModel.Id == newBlock.BlockModel?.Id) return;
            if (_chunk.Region.Deleted) return;
            var render = _chunk.Region.Render;
            ForEachVisibleFaceInt(face => render.RemoveData(face, this));
        }

        public override void OnNeighbourBlockChange(Block from, Block to, BlockFace relative) {
            var slabFace = relative == BlockFace.Up && !_upside || relative == BlockFace.Down && _upside;
            if (slabFace) return;
            var faceInt = (int) relative;
            var newVisible = !to.IsFaceOpaque(BlockFaceMethods.GetOpposite(relative));
            SetFaceVisible(faceInt, newVisible);
            if (newVisible) {
                _chunk.Region.Render.AddData(faceInt, this, to.BlockLight.Light, to.BlockLight.Sunlight);
            }
            else {
                _chunk.Region.Render.RemoveData(faceInt, this);
            }
        }

        public override bool Collides(BlockFace fromFace, Vector3 current, Vector3 origin, Vector3 direction,
            out BlockFace face, out Vector3 collision) {
            var position = _upside ? _position.ToFloat() + new Vector3(0, 0.5f, 0) : _position.ToFloat();

            return BlockModel.BlockCollision.CollidesSegment(position, current, current + direction * 2, 
                out collision, out face);
        }

        public override bool IsFaceOpaque(BlockFace face) {
            return _upside ? face == BlockFace.Up : face == BlockFace.Down;
        }

        public abstract int GetTextureIndex(BlockFace face);

        public override void RemoveFromRender() {
            if (_chunk.Region.Deleted) return;
            var render = _chunk.Region.Render;
            ForEachVisibleFaceInt(face => render.RemoveData(face, this));
        }

        public override void Save(Stream stream, BinaryFormatter formatter) {
            base.Save(stream, formatter);
            formatter.Serialize(stream, _visibleFaces);
            formatter.Serialize(stream, _upside);
        }

        public override void Load(Stream stream, BinaryFormatter formatter, uint version, World2dRegion region2d) {
            base.Load(stream, formatter, version, region2d);
            _visibleFaces = (byte) formatter.Deserialize(stream);
            _upside = (bool) formatter.Deserialize(stream);
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
            return !IsFaceOpaque(face);
        }

        public override bool CanLightBePassedFrom(BlockFace face, Block from) {
            return !IsFaceOpaque(face);
        }

        public override void OnNeighbourLightChange(BlockFace relative, Block block) {
            if (!IsFaceVisible(relative)) return;
            if (relative == BlockFace.Up && !_upside || relative == BlockFace.Down && _upside) return;
            _chunk.Region.Render.AddData((int) relative, this, block.BlockLight.Light, block.BlockLight.Sunlight);
        }

        public override void OnSelfLightChange() {
            _chunk.Region.Render.AddData((int) (_upside ? BlockFace.Down : BlockFace.Up),
                this, _blockLight.Light, _blockLight.Sunlight);
        }
    }
}