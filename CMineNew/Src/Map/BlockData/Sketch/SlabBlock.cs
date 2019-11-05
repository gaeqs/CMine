using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CMineNew.Color;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Static;
using OpenTK;

namespace CMineNew.Map.BlockData.Sketch{
    public abstract class SlabBlock : Block{
        public const float SlabHeight = 0.5f;

        protected bool _upside;
        private bool[] _visibleFaces;

        public SlabBlock(BlockStaticDataSlab staticData, Chunk chunk, Vector3i position, Rgba32I textureFilter,
            bool upside)
            : base(staticData, chunk, position, textureFilter) {
            _upside = upside;
            _visibleFaces = new bool[6];
        }

        public override Vector3 CollisionBoxPosition => _position.ToFloat() + new Vector3(0, _upside ? 0.5f : 0, 0);


        public bool Upside => _upside;

        public override float BlockHeight => base.BlockHeight + (_upside ? 0.5f : 0);

        public override float BlockYOffset => base.BlockYOffset + (_upside ? 0.5f : 0);

        public override void OnPlace(Block oldBlock, bool triggerWorldUpdates, bool addToRender) {
            var render = _chunk.Region.Render;
            for (var i = 0; i < _visibleFaces.Length; i++) {
                var block = GetNeighbour((BlockFace) i);
                var face = (BlockFace) i;

                var visibleBySlab = face == BlockFace.Up && !_upside || face == BlockFace.Down && _upside;

                _visibleFaces[i] = visibleBySlab || block == null ||
                                   !block.IsFaceOpaque(BlockFaceMethods.GetOpposite((BlockFace) i));
                if (!addToRender || !_visibleFaces[i]) continue;
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
            for (var i = 0; i < _visibleFaces.Length; i++) {
                if(!_visibleFaces[i]) continue;
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
            _visibleFaces[faceInt] = newVisible;
            if (newVisible) {
                _chunk.Region.Render.AddData(faceInt, this, to.BlockLight.Light, to.BlockLight.Sunlight);
            }
            else {
                _chunk.Region.Render.RemoveData(faceInt, this);
            }
        }

        public override bool Collides(Vector3 current, Vector3 origin, Vector3 direction) {
            var position = _upside ? _position.ToFloat() + new Vector3(0, 0.5f, 0) : _position.ToFloat();
            return BlockModel.BlockCollision.CollidesSegment(position, current, current + direction * 2);
        }

        public override bool IsFaceOpaque(BlockFace face) {
            return _upside ? face == BlockFace.Up : face == BlockFace.Down;
        }

        public abstract Area2d GetTextureArea(BlockFace face);

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
            _visibleFaces = (bool[]) formatter.Deserialize(stream);
            _upside = (bool) formatter.Deserialize(stream);
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
            return !IsFaceOpaque(face);
        }

        public override void OnNeighbourLightChange(BlockFace relative, Block block) {
            if (!_visibleFaces[(int) relative]) return;
            if (relative == BlockFace.Up && !_upside || relative == BlockFace.Down && _upside) return;
            _chunk.Region.Render.AddData((int) relative, this, block.BlockLight.Light, block.BlockLight.Sunlight);
        }

        public override void OnSelfLightChange() {
            _chunk.Region.Render.AddData((int) (_upside ? BlockFace.Down : BlockFace.Up),
                this, _blockLight.Light, _blockLight.Sunlight);
        }
    }
}