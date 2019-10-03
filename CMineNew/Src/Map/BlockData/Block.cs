using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CMineNew.DataStructure.List;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using CMineNew.Map.BlockData.Static;
using CMineNew.Map.BlockData.Type;
using CMineNew.Texture;
using CMineNew.Util;
using OpenTK;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData{
    public abstract class Block{
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

        public void OnPlace0(Block oldBlock, bool triggerWorldUpdates, bool expandSunlight, bool addToRender) {
            CalculateVisibleFaces();
            if (triggerWorldUpdates) {
                CalculateAllLight(expandSunlight);
                TriggerLightChange();
            }
            else {
                for (var i = 0; i < _neighbours.Length; i++) {
                    if (!_neighbours[i].TryGetTarget(out var neighbour)) continue;
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
                if (_neighbours[(int) fromFace].TryGetTarget(out var neighbour)) {
                    var source = neighbour._blockLight.Source;
                    BlockLightMethods.Expand(this, source, light, neighbour, fromFace);
                }
            }

            //Sunlight
            UpdateSunlight();
            light = CalculateSunlightFromNeighbours(out fromFace);
            if (light > _blockLight.LinearSunlight) {
                if (_neighbours[(int) fromFace].TryGetTarget(out var neighbour)) {
                    var source = neighbour._blockLight.SunlightSource;
                    SunlightMethods.Expand(this, source, light, neighbour, fromFace);
                }
            }
            else if (expandSunlight) {
                ExpandSunlight();
            }
        }

        public void ExpandSunlight() {
            var light = (sbyte) (_blockLight.Sunlight - _staticData.BlockLightPassReduction);
            SunlightMethods.ExpandFrom(this, _position, light);
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
                if (!_neighbours[i].TryGetTarget(out var neighbour)) {
                    neighbour = World.GetBlock(_position + BlockFaceMethods.GetRelative((BlockFace) i));
                    _neighbours[i] = neighbour?._weakReference ?? ReferenceUtils.EmptyWeakBlockReference;
                    if (neighbour == null) {
                        continue;
                    }
                }

                neighbour.OnNeighbourLightChange(BlockFaceMethods.GetOpposite((BlockFace) i), this);
            }
        }

        public sbyte CalculateLightFromNeighbours(out BlockFace face) {
            sbyte light = 0;
            face = BlockFace.Down;
            for (var i = 0; i < _neighbours.Length; i++) {
                var nFace = (BlockFace) i;
                var nOpposite = BlockFaceMethods.GetOpposite(nFace);
                if (!_neighbours[i].TryGetTarget(out var neighbour)) continue;
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
                if (!_neighbours[i].TryGetTarget(out var neighbour)) continue;
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
            var regionPosition = _position - (_chunk.Region.Position << World2dRegion.WorldPositionShift);
            var sunlightData = _chunk.Region.World2dRegion.SunlightData[regionPosition.X, regionPosition.Z];
            var upLight = sunlightData.GetLightFor(_position.Y + 1);
            var linearSunlight = Math.Max(0, upLight - _staticData.SunlightPassReduction);
            _blockLight.LinearSunlight = (sbyte) Math.Max(_blockLight.LinearSunlight, linearSunlight);
            sunlightData.SetBlock(_position.Y, _staticData.SunlightPassReduction);

            //Calculates the initial sunlight.

            _blockLight.Sunlight = _blockLight.LinearSunlight;
            _blockLight.SunlightSource = _position;
        }

        #endregion

        private void CalculateVisibleFaces() {
            for (var i = 0; i < _neighbours.Length; i++) {
                _neighbours[i].TryGetTarget(out var neighbour);
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