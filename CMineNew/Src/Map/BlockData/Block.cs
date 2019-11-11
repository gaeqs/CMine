using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CMineNew.Color;
using CMineNew.DataStructure.List;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using CMineNew.Map.BlockData.Static;
using CMineNew.Map.BlockData.Type;
using CMineNew.Util;
using OpenTK;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData {
    public abstract class Block {
        public const sbyte MaxBlockLight = 15;
        public const float MaxBlockLightF = MaxBlockLight;

        protected readonly BlockStaticData _staticData;

        protected Chunk _chunk;
        protected Vector3i _position;
        protected bool[] _collidableFaces;
        protected Rgba32I _textureFilter;
        protected WeakReference<Block>[] _neighbours;
        protected BlockLight _blockLight;
        protected BlockLightSource _blockLightSource;

        public List<int> _lightValues = new List<int>();
        public List<Block> _lightBlocks = new List<Block>();

        protected readonly WeakReference<Block> _weakReference;

        public Block(BlockStaticData staticData, Chunk chunk, Vector3i position, Rgba32I textureFilter) {
            _weakReference = new WeakReference<Block>(this);

            _staticData = staticData;
            _chunk = chunk;
            _position = position;
            _collidableFaces = new bool[6];
            _textureFilter = textureFilter;
            _neighbours = new WeakReference<Block>[6];

            _blockLight = new BlockLight();
            var lightSource = staticData.LightSource;
            _blockLightSource = lightSource ? new BlockLightSource(this, staticData.LightSourceLight) : null;
            if (!lightSource) return;
            _blockLight.Light = _blockLightSource.SourceLight;
            _blockLight.Source = _blockLightSource;
        }

        public string Id => _staticData.Id;

        public BlockModel BlockModel => _staticData.BlockModel;

        public World World => _chunk.World;

        public Chunk Chunk {
            get => _chunk;
            set => _chunk = value;
        }

        public Vector3i Position {
            get => _position;
            set => _position = value;
        }

        public virtual float BlockHeight => _staticData.BlockHeight;

        public virtual float BlockYOffset => _staticData.BlockYOffset;

        public bool Passable => _staticData.Passable;

        public bool[] CollidableFaces => _collidableFaces;

        public Rgba32I TextureFilter {
            get => _textureFilter;
            set => _textureFilter = value;
        }

        public abstract Vector3 CollisionBoxPosition { get; }

        public BlockLight BlockLight => _blockLight;

        public BlockLightSource BlockLightSource => _blockLightSource;

        public BlockStaticData StaticData => _staticData;

        public WeakReference<Block> WeakReference => _weakReference;

        public WeakReference<Block>[] NeighbourReferences {
            get => _neighbours;
            set {
                for (var i = 0; i < _neighbours.Length; i++) {
                    _neighbours[i] = value[i];
                }
            }
        }

        public Block[] Neighbours {
            set {
                for (var i = 0; i < _neighbours.Length; i++) {
                    _neighbours[i] = value[i]?._weakReference ?? ReferenceUtils.EmptyWeakBlockReference;
                }
            }
        }

        public SunlightData SunlightData {
            get {
                var regionPosition = _position - (_chunk.Region.Position << World2dRegion.WorldPositionShift);
                return _chunk.Region.World2dRegion.SunlightData[regionPosition.X, regionPosition.Z];
            }
        }

        public Block GetNeighbour(BlockFace face) {
            var neighbour = _neighbours[(int) face];
            if (neighbour == null || !neighbour.TryGetTarget(out var block)) {
                return UpdateNeighbour(face);
            }

            return block;
        }

        private Block UpdateNeighbour(BlockFace face) {
            var block = World.GetBlock(_position + BlockFaceMethods.GetRelative(face));
            if (block == null) {
                _neighbours[(int) face] = ReferenceUtils.EmptyWeakBlockReference;
                return null;
            }

            _neighbours[(int) face] = block._weakReference;
            return block;
        }

        public void OnPlace0(Block oldBlock, bool triggerWorldUpdates, bool expandSunlight, bool addToRender) {
            CalculateVisibleFaces();
            if (triggerWorldUpdates) {
                CalculateAllLight(expandSunlight);
                TriggerLightChange();
            }
            else {
                for (var i = 0; i < _neighbours.Length; i++) {
                    var neighbour = GetNeighbour((BlockFace) i);
                    if (neighbour == null) continue;
                    if (neighbour._chunk != _chunk) {
                        neighbour.OnNeighbourLightChange(BlockFaceMethods.GetOpposite((BlockFace) i), this);
                    }
                }
            }

            OnPlace(oldBlock, triggerWorldUpdates, addToRender);
        }

        public abstract void OnPlace(Block oldBlock, bool triggerWorldUpdates, bool addToRender);

        public void OnRemove0(Block newBlock, out ELinkedList<Block> blockList, out ELinkedList<Block> sunList) {
            if (_blockLightSource != null) {
                BlockLightMethods.RemoveLightSource(_blockLightSource);
            }

            SunlightMethods.RemoveLightSource(_position, this);

            blockList = BlockLightMethods.RemoveLightFromBlock(this, false);
            sunList = SunlightMethods.RemoveLightFromBlock(this, false);
            OnRemove(newBlock);
        }

        public abstract void OnRemove(Block newBlock);

        public void OnNeighbourBlockChange0(Block from, Block to, BlockFace relative) {
            _neighbours[(int) relative] = to._weakReference;
            var side = relative != BlockFace.Up && relative != BlockFace.Down &&
                       (BlockHeight > to.BlockHeight || BlockYOffset < to.BlockYOffset);

            _collidableFaces[(int) relative] = to.Passable || side;
            OnNeighbourBlockChange(from, to, relative);
        }

        public abstract void OnNeighbourBlockChange(Block from, Block to, BlockFace relative);

        public abstract void AddToRender();

        public virtual void Save(Stream stream, BinaryFormatter formatter) {
            formatter.Serialize(stream, _blockLight.Light);
            formatter.Serialize(stream, _blockLight.LinearSunlight);
            formatter.Serialize(stream, _blockLight.Sunlight);

            if (this is BlockAir) return;
            formatter.Serialize(stream, _textureFilter.R);
            formatter.Serialize(stream, _textureFilter.G);
            formatter.Serialize(stream, _textureFilter.B);
            formatter.Serialize(stream, _textureFilter.A);
        }

        public virtual void Load(Stream stream, BinaryFormatter formatter, uint version, World2dRegion region2d) {
            _blockLight.Light = (sbyte) formatter.Deserialize(stream);
            _blockLight.LinearSunlight = (sbyte) formatter.Deserialize(stream);
            _blockLight.Sunlight = (sbyte) formatter.Deserialize(stream);
            if (this is BlockAir) return;
            var r = (byte) formatter.Deserialize(stream);
            var g = (byte) formatter.Deserialize(stream);
            var b = (byte) formatter.Deserialize(stream);
            var a = (byte) formatter.Deserialize(stream);
            _textureFilter = new Rgba32I(r, g, b, a);
        }

        #region light

        public void CalculateAllLight(bool expandSunlight) {
            if (_blockLightSource != null) {
                BlockLightMethods.ExpandFrom(this, _blockLightSource,
                    (sbyte) (_blockLightSource.SourceLight - _staticData.BlockLightPassReduction));
            }

            //Block light
            var light = CalculateLightFromNeighbours(out var fromFace);
            if (light > 0) {
                var neighbour = GetNeighbour(fromFace);
                if (neighbour != null) {
                    var source = neighbour._blockLight.Source;
                    BlockLightMethods.Expand(this, source, light, neighbour, fromFace);
                }
            }

            //Sunlight
            UpdateSunlight();
            light = CalculateSunlightFromNeighbours(out fromFace);
            if (light > _blockLight.LinearSunlight) {
                var neighbour = GetNeighbour(fromFace);
                if (neighbour == null) return;
                var source = neighbour._blockLight.SunlightSource;
                SunlightMethods.Expand(this, source, light, neighbour, fromFace);
            }
            else if (expandSunlight) {
                ExpandSunlight();
            }
        }

        public void ExpandSunlight() {
            var light = (sbyte) (_blockLight.Sunlight - _staticData.BlockLightPassReduction);
            SunlightMethods.ExpandFrom(this, _position, light);
        }

        public void RemoveSunlight() {
            SunlightMethods.RemoveLightSource(_position, this);
        }


        public void UpdateLinearSunlight(sbyte light) {
            var old = _blockLight.Sunlight;
            _blockLight.LinearSunlight = light;

            if (light >= old) {
                _blockLight.Sunlight = light;
                _blockLight.SunlightSource = _position;
                SunlightMethods.ExpandFrom(this, _position, (sbyte) (light - _staticData.BlockLightPassReduction));
            }
            else {
                SunlightMethods.RemoveLightSource(_position, this);
                SunlightMethods.ExpandFrom(this, _position, (sbyte) (light - _staticData.BlockLightPassReduction));
            }
        }

        public void TriggerLightChange(bool self = true) {
            if (self) {
                OnSelfLightChange();
            }

            for (var i = 0; i < _neighbours.Length; i++) {
                var neighbour = GetNeighbour((BlockFace) i);
                neighbour?.OnNeighbourLightChange(BlockFaceMethods.GetOpposite((BlockFace) i), this);
            }
        }

        public sbyte CalculateLightFromNeighbours(out BlockFace face) {
            sbyte light = 0;
            face = BlockFace.Down;
            for (var i = 0; i < _neighbours.Length; i++) {
                var nFace = (BlockFace) i;
                var nOpposite = BlockFaceMethods.GetOpposite(nFace);
                var neighbour = GetNeighbour((BlockFace) i);
                if (neighbour == null) continue;
                if (!CanLightBePassedFrom(nFace, neighbour)
                    || !neighbour.CanLightPassThrough(nOpposite)) continue;
                var nLight = (sbyte) (neighbour.BlockLight.Light - neighbour._staticData.BlockLightPassReduction);
                if (nLight <= light) continue;
                light = nLight;
                face = nFace;
            }

            return light;
        }

        public sbyte CalculateSunlightFromNeighbours(out BlockFace face) {
            sbyte light = 0;
            face = BlockFace.Down;
            for (var i = 0; i < _neighbours.Length; i++) {
                var nFace = (BlockFace) i;
                var nOpposite = BlockFaceMethods.GetOpposite(nFace);
                var neighbour = GetNeighbour((BlockFace) i);
                if (neighbour == null) continue;
                if (!CanLightBePassedFrom(nFace, neighbour) || !neighbour.CanLightPassThrough(nOpposite)) continue;
                var nLight = (sbyte) (neighbour.BlockLight.Sunlight - neighbour._staticData.BlockLightPassReduction);
                if (nLight <= light) continue;
                light = nLight;
                face = nFace;
            }

            return light;
        }

        private void UpdateSunlight() {
            //Calculate the linear sunlight.
            SunlightData.SetBlock(_position.Y, _staticData.SunlightPassReduction, this);
        }

        #endregion

        private void CalculateVisibleFaces() {
            for (var i = 0; i < _neighbours.Length; i++) {
                var neighbour = GetNeighbour((BlockFace) i);
                var face = (BlockFace) i;
                var side = neighbour != null &&
                           face != BlockFace.Up && face != BlockFace.Down &&
                           (BlockHeight > neighbour.BlockHeight
                            || BlockYOffset < neighbour.BlockYOffset);
                _collidableFaces[i] = side || neighbour == null || neighbour.Passable;
            }
        }

        public abstract Block Clone(Chunk chunk, Vector3i position);

        public abstract bool Collides(Vector3 current, Vector3 origin, Vector3 direction);

        public abstract bool IsFaceOpaque(BlockFace face);

        public abstract void RemoveFromRender();

        public abstract bool CanLightPassThrough(BlockFace face);

        public abstract bool CanLightBePassedFrom(BlockFace face, Block from);

        public abstract void OnNeighbourLightChange(BlockFace relative, Block block);

        public abstract void OnSelfLightChange();
    }
}