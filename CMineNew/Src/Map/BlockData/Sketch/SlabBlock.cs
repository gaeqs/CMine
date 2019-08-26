using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using OpenTK;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Sketch{
    public abstract class SlabBlock : Block{
        public const float SlabHeight = 0.5f;

        protected bool _upside;
        private bool[] _visibleFaces;

        public SlabBlock(string id, Chunk chunk, Vector3i position, Color4 textureFilter, bool upside,
            bool passable = false)
            : base(id, BlockModelManager.GetModelOrNull(SlabBlockModel.Key), chunk, position, textureFilter,
                passable, upside ? 1 : SlabHeight, upside ? SlabHeight : 0) {
            _upside = upside;
            _visibleFaces = new bool[6];
        }

        public override Vector3 CollisionBoxPosition => _position.ToFloat() + new Vector3(0, _upside ? 0.5f : 0, 0);


        public bool Upside => _upside;

        public override void OnPlace(Block oldBlock, Block[] neighbours, bool triggerWorldUpdates) {
            var render = _chunk.Region.Render;
            for (var i = 0; i < _visibleFaces.Length; i++) {
                var block = neighbours[i];

                var visibleBySlab = i == (int) BlockFace.Up && !_upside || i == (int) BlockFace.Down && _upside;

                _visibleFaces[i] = visibleBySlab || block == null ||
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
            if (relative == BlockFace.Up && !_upside || relative == BlockFace.Down && _upside) return;
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
            var position = _upside ? _position.ToFloat() + new Vector3(0, 0.5f, 0) : _position.ToFloat();
            return _blockModel.BlockCollision.CollidesSegment(position, current, current + direction * 2);
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

        public override void Load(Stream stream, BinaryFormatter formatter, uint version) {
            base.Load(stream, formatter, version);
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
    }
}