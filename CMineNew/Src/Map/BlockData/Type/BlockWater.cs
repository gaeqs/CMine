using System;
using System.Collections.Generic;
using System.Linq;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using CMineNew.Map.Task.Type;
using OpenTK;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Type{
    public class BlockWater : Block{
        public const int MaxWaterLevel = 7;
        public const int ExpandTicks = CMine.TicksPerSecond / 3;
        public const int RemoveTicks = CMine.TicksPerSecond / 7;
        protected const int NorthLeft = 0;
        protected const int NorthRight = 1;
        protected const int SouthLeft = 2;
        protected const int SouthRight = 3;

        public static Area2d TextureArea = CMine.Textures.Areas["default:water"];

        private readonly bool[] _visibleFaces;
        protected int _waterLevel;
        protected bool _hasWaterOnTop;

        private Vector3i _parent;
        private List<Vector3i> _children;
        private bool _removing;


        //0 = north-left, 1 = north-right, 2 = south-left, 3 = south-right
        private readonly float[] _vertexWaterLevel;

        public BlockWater(Chunk chunk, Vector3i position, int level)
            : base("default:water", BlockModelManager.GetModelOrNull(WaterBlockModel.Key), chunk, position,
                Color4.Transparent, true) {
            _visibleFaces = new bool[6];
            _waterLevel = level;
            _vertexWaterLevel = new float[4];
            _parent = position;
            _children = new List<Vector3i>(5);
            _removing = false;
        }

        public int WaterLevel {
            get => _waterLevel;
            set {
                _waterLevel = value;
                UpdateWaterVertices(true, true, true, true, true);
            }
        }

        public float WaterHeight => _hasWaterOnTop ? 1f : (_waterLevel + 1) / 9f;

        public float[] VertexWaterLevel => _vertexWaterLevel;

        public bool HasWaterOnTop => _hasWaterOnTop;

        public Vector3i Parent {
            get => _parent;
            set => _parent = value;
        }

        public List<Vector3i> Children => _children;

        public bool Removing => _removing;

        public override void OnPlace(Block oldBlock, Block[] neighbours, bool triggerWorldUpdates) {
            _hasWaterOnTop = neighbours[(int) BlockFace.Up] is BlockWater;
            for (var i = 0; i < neighbours.Length; i++) {
                var block = neighbours[i];
                if (i == (int) BlockFace.Up) {
                    _visibleFaces[i] = !(neighbours[i] is BlockWater);
                }
                else {
                    _visibleFaces[i] = block == null ||
                                       !(block is BlockWater)
                                       && !block.IsFaceOpaque(BlockFaceMethods.GetOpposite((BlockFace) i));
                }
            }

            if (neighbours.Sum(target => target is BlockWater water && water.Parent == water.Position ? 1 : 0) > 1) {
                var parentBlock = World.GetBlock(_parent);
                if (parentBlock is BlockWater parentWater) {
                    parentWater.Children.Remove(_position);
                }

                _parent = _position;
                _waterLevel = MaxWaterLevel;
            }

            UpdateWaterVertices(true, true, true, true, true);
            if (triggerWorldUpdates) {
                _chunk.TaskManager.AddTask(new WorldTaskExpandWater(World, _position));
            }
        }

        public override void OnRemove(Block newBlock) {
            if (_blockModel.Id == newBlock.BlockModel?.Id) return;
            if (_chunk.Region.Deleted) return;
            var render = _chunk.Region.Render;
            ForEachVisibleFaceInt(face => render.RemoveData(face, this));
            UpdateWaterVertices(false, true, true, true, true);

            foreach (var child in _children) {
                var chunk = World.GetChunkFromWorldPosition(child);
                var block = chunk.GetBlockFromWorldPosition(child);
                if (!(block is BlockWater water)) return;
                water._removing = true;
                if (!(newBlock is BlockWater)) {
                    chunk.TaskManager.AddTask(new WorldTaskRemoveWater(World, child));
                }
            }
        }

        public override void OnNeighbourBlockChange(Block from, Block to, BlockFace relative) {
            var render = _chunk.Region.Render;

            if (relative == BlockFace.Up) {
                if (to is BlockWater) {
                    if (_visibleFaces[(int) relative]) {
                        _visibleFaces[(int) relative] = false;
                        render.RemoveData((int) relative, this);
                    }

                    _hasWaterOnTop = true;
                    UpdateWaterVertices(true, true, true, true, true);
                    _waterLevel = MaxWaterLevel;
                }
                else if (!_visibleFaces[(int) relative]) {
                    _visibleFaces[(int) relative] = true;
                    render.AddData((int) relative, this);
                }
            }
            else {
                if (!(to is BlockWater)) {
                    _chunk.TaskManager.AddTask(new WorldTaskExpandWater(World, _position));
                }

                var old = _visibleFaces[(int) relative];
                var newData = to == null ||
                              !(to is BlockWater)
                              && !to.IsFaceOpaque(BlockFaceMethods.GetOpposite(relative));
                _visibleFaces[(int) relative] = newData;
                if (old == newData) return;
                if (newData) {
                    render.AddData((int) relative, this);
                }
                else {
                    render.RemoveData((int) relative, this);
                }
            }
        }

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new BlockWater(chunk, position, _waterLevel);
        }

        public override bool Collides(Vector3 current, Vector3 origin, Vector3 direction) {
            return true;
        }

        public override bool IsFaceOpaque(BlockFace face) {
            return false;
        }

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

        public void UpdateWaterLevel() {
            var render = _chunk.Region.Render;
            ForEachVisibleFaceInt(face => render.AddData(face, this));
        }

        private void UpdateWaterVertices(bool updateSelf, bool ln, bool rn, bool ls, bool rs) {
            if (ln) {
                UpdateWaterVertices(updateSelf, new Vector3i(-1, 0, 0), new Vector3i(0, 0, -1),
                    new Vector3i(-1, 0, -1), NorthLeft, NorthRight, SouthLeft, SouthRight);
            }

            if (rn) {
                UpdateWaterVertices(updateSelf, new Vector3i(1, 0, 0), new Vector3i(0, 0, -1),
                    new Vector3i(1, 0, -1), NorthRight, NorthLeft, SouthRight, SouthLeft);
            }

            if (ls) {
                UpdateWaterVertices(updateSelf, new Vector3i(-1, 0, 1), new Vector3i(0, 0, 1),
                    new Vector3i(-1, 0, 0), SouthLeft, NorthRight, NorthLeft, SouthRight);
            }

            if (rs) {
                UpdateWaterVertices(updateSelf, new Vector3i(0, 0, 1), new Vector3i(1, 0, 1),
                    new Vector3i(1, 0, 0), SouthRight, NorthRight, NorthLeft, SouthLeft);
            }
        }

        private void UpdateWaterVertices(bool updateSelf, Vector3i posA, Vector3i posB, Vector3i posC, int self, int a,
            int b, int c) {
            var blockA = World.GetBlock(_position + posA);
            var blockB = World.GetBlock(_position + posB);
            var blockC = World.GetBlock(_position + posC);
            var level = (float) _waterLevel;
            var amount = 1;
            var waterA = blockA is BlockWater wa ? wa : null;
            var waterB = blockB is BlockWater wb ? wb : null;
            var waterC = blockC is BlockWater wc ? wc : null;

            if (_hasWaterOnTop || waterA != null && waterA._hasWaterOnTop ||
                waterB != null && waterB._hasWaterOnTop || waterC != null && waterC._hasWaterOnTop) {
                level = MaxWaterLevel + 1;
                amount = 1;
            }
            else {
                if (waterA != null) {
                    level += waterA._waterLevel;
                    amount++;
                }

                if (waterB != null) {
                    level += waterB._waterLevel;
                    amount++;
                }

                if (waterC != null) {
                    level += waterC._waterLevel;
                    amount++;
                }
            }

            level /= amount;
            _vertexWaterLevel[self] = level;
            if (updateSelf) {
                UpdateWaterLevel();
            }

            if (waterA != null) {
                waterA._vertexWaterLevel[a] = level;
                waterA.UpdateWaterLevel();
            }

            if (waterB != null) {
                waterB._vertexWaterLevel[b] = level;
                waterB.UpdateWaterLevel();
            }

            if (waterC != null) {
                waterC._vertexWaterLevel[c] = level;
                waterC.UpdateWaterLevel();
            }
        }
    }
}