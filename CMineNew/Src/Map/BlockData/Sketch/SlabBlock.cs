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
                var face = (BlockFace) i;

                var visibleBySlab = face == BlockFace.Up && !_upside || face == BlockFace.Down && _upside;

                _visibleFaces[i] = visibleBySlab || block == null ||
                                   !block.IsFaceOpaque(BlockFaceMethods.GetOpposite((BlockFace) i));
                if (_visibleFaces[i]) {
                    var light = visibleBySlab || block == null ? _blockLight : block.BlockLight;
                    render.AddData(i, this, light);
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
            var slabFace = relative == BlockFace.Up && !_upside || relative == BlockFace.Down && _upside;
            if (slabFace) return;
            var faceInt = (int) relative;
            var newVisible = !to.IsFaceOpaque(BlockFaceMethods.GetOpposite(relative));
            _visibleFaces[faceInt] = newVisible;
            if (newVisible) {
                _chunk.Region.Render.AddData(faceInt, this, to.BlockLight);
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

        public override void OnLightChange(BlockFace from, Block fromBlock, int light, Vector3i source) {
            if (IsFaceOpaque(from)) return;
            if (BlockFaceMethods.IsSide(from) && fromBlock is SlabBlock slab && slab._upside != _upside) return;
            if (light <= _blockLight) return;
            _blockLight = light;
            _blockLightSource = source;
            light -= _blockLightReduction;

            if (_upside && from == BlockFace.Down || !_upside && from == BlockFace.Up) {
                _chunk.Region.Render.AddData((int) from, this, _blockLight);
            }

            var blocks = _chunk.GetNeighbourBlocks(new Block[6], _position,
                _position - (_chunk.Position << Chunk.WorldPositionShift));
            for (var i = 0; i < blocks.Length; i++) {
                var face = (BlockFace) i;
                var opposite = BlockFaceMethods.GetOpposite(face);
                if (IsFaceOpaque(face)) continue;
                var block = blocks[i];
                block?.OnNeighbourLightChange(opposite, this, _blockLight, source);
                if (light > 0) {
                    block?.OnLightChange(opposite, this, light, source);
                }
            }
        }

        public override void OnNeighbourLightChange(BlockFace relative, Block block, int light, Vector3i source) {
            if (_upside && relative == BlockFace.Down || !_upside && relative == BlockFace.Up) {
                return;
            }

            if (_visibleFaces[(int) relative]) {
                _chunk.Region.Render.AddData((int) relative, this, light);
            }
        }
    }
}