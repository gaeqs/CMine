using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CMineNew.DataStructure.List;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using CMineNew.Map.BlockData.Static;
using CMineNew.Map.BlockData.Type;
using OpenTK;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData{
    public abstract class Block{
        public const int MaxBlockLight = 15;
        public const float MaxBlockLightF = MaxBlockLight;

        protected readonly BlockStaticData _staticData;

        protected Chunk _chunk;
        protected Vector3i _position;
        protected bool[] _collidableFaces;
        protected Color4 _textureFilter;
        protected Block[] _neighbours;
        protected BlockLight _blockLight;
        protected BlockLightSource _blockLightSource;

        public Block(BlockStaticData staticData, Chunk chunk, Vector3i position, Color4 textureFilter) {
            _staticData = staticData;
            _chunk = chunk;
            _position = position;
            _collidableFaces = new bool[6];
            _textureFilter = textureFilter;
            _neighbours = new Block[6];

            _blockLight = new BlockLight(staticData.BlockLightPassReduction, staticData.SunlightPassReduction);
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

        public Color4 TextureFilter {
            get => _textureFilter;
            set => _textureFilter = value;
        }

        public abstract Vector3 CollisionBoxPosition { get; }

        public BlockLight BlockLight => _blockLight;

        public BlockLightSource BlockLightSource => _blockLightSource;

        public Block[] Neighbours {
            get => _neighbours;
            set {
                for (var i = 0; i < _neighbours.Length; i++) {
                    _neighbours[i] = value[i];
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
                    var neighbour = _neighbours[i];
                    if (neighbour != null && neighbour._chunk != _chunk) {
                        neighbour.OnNeighbourLightChange(BlockFaceMethods.GetOpposite((BlockFace) i), this);
                    }
                }
            }

            OnPlace(oldBlock, _neighbours, triggerWorldUpdates, addToRender);
        }

        public abstract void OnPlace(Block oldBlock, Block[] neighbours, bool triggerWorldUpdates, bool addToRender);

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
            _neighbours[(int) relative] = to;
            var side = relative != BlockFace.Up && relative != BlockFace.Down &&
                       (BlockHeight > to.BlockHeight || BlockYOffset < to.BlockYOffset);

            _collidableFaces[(int) relative] = to == null || to.Passable || side;
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
            _blockLight.Light = (int) formatter.Deserialize(stream);
            _blockLight.LinearSunlight = (int) formatter.Deserialize(stream);
            _blockLight.Sunlight = (int) formatter.Deserialize(stream);
            if (this is BlockAir) return;
            var r = (float) formatter.Deserialize(stream);
            var g = (float) formatter.Deserialize(stream);
            var b = (float) formatter.Deserialize(stream);
            var a = (float) formatter.Deserialize(stream);
            _textureFilter = new Color4(r, g, b, a);
        }

        #region light

        public void CalculateAllLight(bool expandSunlight) {
            if (_blockLightSource != null) {
                BlockLightMethods.ExpandFrom(this, _blockLightSource,
                    _blockLightSource.SourceLight - _blockLight.BlockLightPassReduction);
            }

            //Block light
            var light = CalculateLightFromNeighbours(out var fromFace);
            if (light > 0) {
                var neighbour = _neighbours[(int) fromFace];
                var source = neighbour._blockLight.Source;
                BlockLightMethods.Expand(this, source, light, neighbour, fromFace);
            }

            //Sunlight
            UpdateSunlight();
            light = CalculateSunlightFromNeighbours(out fromFace);
            if (light > _blockLight.LinearSunlight) {
                var neighbour = _neighbours[(int) fromFace];
                var source = neighbour._blockLight.SunlightSource;
                SunlightMethods.Expand(this, source, light, neighbour, fromFace);
            }
            else if (expandSunlight) {
                ExpandSunlight();
            }
        }

        public void ExpandSunlight() {
            var light = _blockLight.Sunlight - _blockLight.BlockLightPassReduction;
            SunlightMethods.ExpandFrom(this, _position, light);
        }

        public void UpdateLinearSunlight(int light) {
            var old = _blockLight.Sunlight;
            _blockLight.LinearSunlight = light;

            if (light >= old) {
                _blockLight.Sunlight = light;
                _blockLight.SunlightSource = _position;
                SunlightMethods.ExpandFrom(this, _position, light - _blockLight.BlockLightPassReduction);
            }
            else {
                SunlightMethods.RemoveLightSource(_position, this);
                SunlightMethods.ExpandFrom(this, _position, light - _blockLight.BlockLightPassReduction);
            }
        }

        public void TriggerLightChange(bool self = true) {
            if (self) {
                OnSelfLightChange();
            }

            for (var i = 0; i < _neighbours.Length; i++) {
                var neighbour = _neighbours[i];
                if (neighbour == null) {
                    neighbour = World.GetBlock(_position + BlockFaceMethods.GetRelative((BlockFace) i));
                    _neighbours[i] = neighbour;
                    if (neighbour == null) {
                        continue;
                    }
                }

                neighbour.OnNeighbourLightChange(BlockFaceMethods.GetOpposite((BlockFace) i), this);
            }
        }

        public int CalculateLightFromNeighbours(out BlockFace face) {
            var light = 0;
            face = BlockFace.Down;
            for (var i = 0; i < _neighbours.Length; i++) {
                var nFace = (BlockFace) i;
                var nOpposite = BlockFaceMethods.GetOpposite(nFace);
                var neighbour = _neighbours[i];
                if (neighbour == null
                    || !CanLightBePassedFrom(nFace, neighbour)
                    || !neighbour.CanLightPassThrough(nOpposite)) continue;
                var nLight = neighbour.BlockLight.Light - neighbour.BlockLight.BlockLightPassReduction;
                if (nLight <= light) continue;
                light = nLight;
                face = nFace;
            }

            return light;
        }

        public int CalculateSunlightFromNeighbours(out BlockFace face) {
            var light = 0;
            face = BlockFace.Down;
            for (var i = 0; i < _neighbours.Length; i++) {
                var nFace = (BlockFace) i;
                var nOpposite = BlockFaceMethods.GetOpposite(nFace);
                var neighbour = _neighbours[i];
                if (neighbour == null
                    || !CanLightBePassedFrom(nFace, neighbour)
                    || !neighbour.CanLightPassThrough(nOpposite)) continue;
                var nLight = neighbour.BlockLight.Sunlight - neighbour.BlockLight.BlockLightPassReduction;
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
            var linearSunlight = Math.Max(0, upLight - _blockLight.SunlightPassReduction);
            _blockLight.LinearSunlight = Math.Max(_blockLight.LinearSunlight, linearSunlight);
            sunlightData.SetBlock(_position.Y, _blockLight.SunlightPassReduction);

            //Calculates the initial sunlight.

            _blockLight.Sunlight = _blockLight.LinearSunlight;
            _blockLight.SunlightSource = _position;
        }

        #endregion

        private void CalculateVisibleFaces() {
            for (var i = 0; i < _neighbours.Length; i++) {
                var neighbour = _neighbours[i];
                var face = (BlockFace) i;
                var side = neighbour != null &&
                           face != BlockFace.Up && face != BlockFace.Down &&
                           (BlockHeight > _neighbours[i]?.BlockHeight
                            || BlockYOffset < _neighbours[i].BlockYOffset);
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