using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CMineNew.DataStructure.List;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using OpenTK;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData{
    public abstract class Block{
        public const int MaxBlockLight = 15;
        public const float MaxBlockLightF = MaxBlockLight;

        protected readonly string _id;
        protected readonly BlockModel _blockModel;

        protected Chunk _chunk;
        protected Vector3i _position;
        protected bool _passable;
        protected bool[] _collidableFaces;
        protected Color4 _textureFilter;
        protected float _blockHeight, _blockYOffset;

        protected Block[] _neighbours;

        protected BlockLight _blockLight;
        protected BlockLightSource _blockLightSource;

        public Block(string id, BlockModel blockModel, Chunk chunk, Vector3i position,
            Color4 textureFilter, bool passable = false, float blockHeight = 1, float blockYOffset = 0,
            bool isSource = false, int sourceLight = 0, int lightPassReduction = 1) {
            _id = id;
            _blockModel = blockModel;
            _chunk = chunk;
            _position = position;
            _passable = passable;
            _collidableFaces = new bool[6];
            _textureFilter = textureFilter;
            _blockHeight = blockHeight;
            _blockYOffset = blockYOffset;
            _neighbours = new Block[6];

            _blockLight = new BlockLight(lightPassReduction);
            _blockLightSource = isSource ? new BlockLightSource(this, sourceLight) : null;
        }

        public string Id => _id;

        public BlockModel BlockModel => _blockModel;

        public World World => _chunk.World;

        public Chunk Chunk {
            get => _chunk;
            set => _chunk = value;
        }

        public Vector3i Position {
            get => _position;
            set => _position = value;
        }

        public float BlockHeight => _blockHeight;

        public float BlockYOffset => _blockYOffset;

        public bool Passable => _passable;

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

        public void OnPlace0(Block oldBlock, bool triggerWorldUpdates) {
            for (var i = 0; i < _neighbours.Length; i++) {
                var neighbour = _neighbours[i];
                var face = (BlockFace) i;
                var side = neighbour != null &&
                           face != BlockFace.Up && face != BlockFace.Down &&
                           (_blockHeight > _neighbours[i]?._blockHeight
                            || _blockYOffset < _neighbours[i]._blockYOffset);
                _collidableFaces[i] = side || neighbour == null || neighbour._passable;
            }

            if (_blockLightSource != null) {
                BlockLightMethods.ExpandFrom(this, _blockLightSource,
                    _blockLightSource.SourceLight - _blockLight.LightPassReduction);
            }

            var light = CalculateLightFromNeighbours(out var fromFace);
            if (light > 0) {
                var neighbour = _neighbours[(int) fromFace];
                var source = neighbour._blockLight.Source;
                BlockLightMethods.Expand(this, source, light, neighbour, fromFace);
            }

            OnPlace(oldBlock, _neighbours, triggerWorldUpdates);
        }

        public abstract void OnPlace(Block oldBlock, Block[] neighbours, bool triggerWorldUpdates);

        public ELinkedList<Block> OnRemove0(Block newBlock) {
            if (_blockLightSource != null) {
                BlockLightMethods.RemoveLightSource(_blockLightSource);
            }
            var list = BlockLightMethods.RemoveLightFromBlock(this, false);
            OnRemove(newBlock);
            return list;
        }

        public abstract void OnRemove(Block newBlock);

        public void OnNeighbourBlockChange0(Block from, Block to, BlockFace relative) {
            _neighbours[(int) relative] = to;
            var side = relative != BlockFace.Up && relative != BlockFace.Down &&
                       (_blockHeight > to._blockHeight || _blockYOffset < to._blockYOffset);

            _collidableFaces[(int) relative] = to == null || to._passable || side;
            OnNeighbourBlockChange(from, to, relative);
        }

        public virtual void Save(Stream stream, BinaryFormatter formatter) {
            formatter.Serialize(stream, _textureFilter.R);
            formatter.Serialize(stream, _textureFilter.G);
            formatter.Serialize(stream, _textureFilter.B);
            formatter.Serialize(stream, _textureFilter.A);
        }

        public virtual void Load(Stream stream, BinaryFormatter formatter, uint version, World2dRegion region2d) {
            var r = (float) formatter.Deserialize(stream);
            var g = (float) formatter.Deserialize(stream);
            var b = (float) formatter.Deserialize(stream);
            var a = (float) formatter.Deserialize(stream);
            _textureFilter = new Color4(r, g, b, a);
        }

        public void TriggerLightChange() {
            OnSelfLightChange();
            for (var i = 0; i < _neighbours.Length; i++) {
                _neighbours[i]?.OnNeighbourLightChange(BlockFaceMethods.GetOpposite((BlockFace) i), this);
            }
        }

        public int CalculateLightFromNeighbours(out BlockFace face) {
            var light = 0;
            face = BlockFace.Down;
            for (var i = 0; i < _neighbours.Length; i++) {
                var nFace = (BlockFace) i;
                var nOpposite = BlockFaceMethods.GetOpposite(nFace);
                var neighbour = _neighbours[i];
                if(neighbour == null 
                   || !CanLightBePassedFrom(nFace, neighbour)
                   || !neighbour.CanLightPassThrough(nOpposite)) continue;
                var nLight = neighbour.BlockLight.Light - neighbour.BlockLight.LightPassReduction;
                if(nLight <= light) continue;
                light = nLight;
                face = nFace;
            }

            return light;
        }

        public abstract void OnNeighbourBlockChange(Block from, Block to, BlockFace relative);
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