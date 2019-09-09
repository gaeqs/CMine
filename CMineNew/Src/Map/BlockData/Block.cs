using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Model;
using OpenTK;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData{
    public abstract class Block{
        public const int MaxBlockLight = 16;
        public const float MaxBlockLightF = MaxBlockLight;

        protected readonly string _id;
        protected readonly BlockModel _blockModel;

        protected Chunk _chunk;
        protected Vector3i _position;
        protected bool _passable;
        protected bool[] _collidableFaces;
        protected Color4 _textureFilter;
        protected float _blockHeight, _blockYOffset;

        protected bool _lightSource;
        protected Vector3i _blockLightSource;
        protected int _blockLight;
        protected int _blockLightReduction;

        public Block(string id, BlockModel blockModel, Chunk chunk, Vector3i position,
            Color4 textureFilter, bool passable = false, float blockHeight = 1, float blockYOffset = 0,
            bool lightSource = false, int blockLight = 0, int blockLightReduction = 1) {
            _id = id;
            _blockModel = blockModel;
            _chunk = chunk;
            _position = position;
            _passable = passable;
            _collidableFaces = new bool[6];
            _textureFilter = textureFilter;
            _blockHeight = blockHeight;
            _blockYOffset = blockYOffset;

            _lightSource = lightSource;
            _blockLightSource = position;
            _blockLight = blockLight;
            _blockLightReduction = blockLightReduction;
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

        public bool LightSource {
            get => _lightSource;
            set => _lightSource = value;
        }

        public Vector3i BlockLightSource {
            get => _blockLightSource;
            set => _blockLightSource = value;
        }

        public int BlockLight {
            get => _blockLight;
            set => _blockLight = value;
        }

        public int BlockLightReduction {
            get => _blockLightReduction;
            set => _blockLightReduction = value;
        }

        public abstract Vector3 CollisionBoxPosition { get; }

        public void OnPlace0(Block oldBlock, Block[] neighbours, bool triggerWorldUpdates) {
            var light = 0;
            Block brightestBlock = null;
            BlockFace brightestBlockFace = BlockFace.Down;
            for (var i = 0; i < neighbours.Length; i++) {
                var face = (BlockFace) i;
                var neighbour = neighbours[i];
                var side = neighbour != null &&
                           face != BlockFace.Up && face != BlockFace.Down &&
                           (_blockHeight > neighbours[i]?._blockHeight
                            || _blockYOffset < neighbours[i]._blockYOffset);
                _collidableFaces[i] = side || neighbour == null || neighbour._passable;

                if (_lightSource) {
                    neighbour?.OnLightChange(BlockFaceMethods.GetOpposite(face), this, _blockLight, _position);
                }
                else if (neighbour != null && light < neighbour._blockLight) {
                    light = neighbour._blockLight;
                    brightestBlock = neighbour;
                    brightestBlockFace = face;
                }
            }

            if (brightestBlock != null) {
                OnLightChange(brightestBlockFace, brightestBlock, light, brightestBlock._blockLightSource);
            }

            OnPlace(oldBlock, neighbours, triggerWorldUpdates);
        }

        public abstract void OnPlace(Block oldBlock, Block[] neighbours, bool triggerWorldUpdates);

        public abstract void OnRemove(Block newBlock);

        public void OnNeighbourBlockChange0(Block from, Block to, BlockFace relative) {
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

        public abstract void OnNeighbourBlockChange(Block from, Block to, BlockFace relative);
        public abstract Block Clone(Chunk chunk, Vector3i position);

        public abstract bool Collides(Vector3 current, Vector3 origin, Vector3 direction);

        public abstract bool IsFaceOpaque(BlockFace face);

        public abstract void RemoveFromRender();

        public abstract void OnLightChange(BlockFace from, Block fromBlock, int light, Vector3i source);
        
        public abstract void OnNeighbourLightChange(BlockFace relative, Block block, int light, Vector3i source);
    }
}