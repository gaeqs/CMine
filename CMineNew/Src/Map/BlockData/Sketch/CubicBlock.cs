using System;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using OpenTK;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Sketch{
    public abstract class CubicBlock : Block{
        private bool[] _visibleFaces;

        public CubicBlock(string id, Chunk chunk, Vector3i position, Color4 textureFilter, bool passable = false,
            bool lightSource = false, int blockLight = 0, int blockLightReduction = 1)
            : base(id, BlockModelManager.GetModelOrNull(CubicBlockModel.Key), chunk, position, textureFilter,
                passable, 1, 0, lightSource, blockLight, blockLightReduction) {
            _visibleFaces = new bool[6];
        }

        public override Vector3 CollisionBoxPosition => _position.ToFloat();

        public override void OnPlace(Block oldBlock, Block[] neighbours, bool triggerWorldUpdates) {
            var render = _chunk.Region.Render;
            for (var i = 0; i < _visibleFaces.Length; i++) {
                var block = neighbours[i];
                _visibleFaces[i] = block == null || !block.IsFaceOpaque(BlockFaceMethods.GetOpposite((BlockFace) i));
                if (_visibleFaces[i]) {
                    render.AddData(i, this, block?.BlockLight ?? _blockLight);
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
            var newVisible = !to.IsFaceOpaque(BlockFaceMethods.GetOpposite(relative));
            if (oldVisible == newVisible) return;
            _visibleFaces[faceInt] = newVisible;
            if (newVisible) {
                _chunk.Region.Render.AddData(faceInt, this, to.BlockLight);
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

        public override void OnLightChange(BlockFace from, Block fromBlock, int light, Vector3i source) {
            if (IsFaceOpaque(from)) return;
            if (light <= _blockLight) return;
            _blockLight = light;
            _blockLightSource = source;
            light -= _blockLightReduction;

            var blocks = _chunk.GetNeighbourBlocks(new Block[6], _position,
                _position - (_chunk.Position << Chunk.WorldPositionShift));
            for (var i = 0; i < blocks.Length; i++) {
                var face = (BlockFace) i;
                var opposite = BlockFaceMethods.GetOpposite(face);
                var block = blocks[i];
                block?.OnNeighbourLightChange(opposite, this, _blockLight, source);
                if (light > 0) {
                    block?.OnLightChange(opposite, this, light, source);
                }
            }
        }

        public override void OnNeighbourLightChange(BlockFace relative, Block block, int light, Vector3i source) {
            if (_visibleFaces[(int) relative]) {
                _chunk.Region.Render.AddData((int) relative, this, light);
            }
        }
    }
}