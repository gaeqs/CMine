using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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

        protected BlockLight _blockLight;

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
            _blockLight = new BlockLight(isSource, sourceLight, lightPassReduction);

            if (_blockLight.IsSource) {
                _blockLight.AddSource(_position, _blockLight.SourceLight);
            }
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

        public BlockLight BlockLight => _blockLight;

        public Color4 TextureFilter {
            get => _textureFilter;
            set => _textureFilter = value;
        }

        public abstract Vector3 CollisionBoxPosition { get; }

        public void OnPlace0(Block oldBlock, Block[] neighbours, bool triggerWorldUpdates) {
            var sourceLight = _blockLight.SourceLight - _blockLight.LightPassReduction;
            for (var i = 0; i < neighbours.Length; i++) {
                var face = (BlockFace) i;
                var opposite = BlockFaceMethods.GetOpposite(face);
                var neighbour = neighbours[i];
                var side = neighbour != null &&
                           face != BlockFace.Up && face != BlockFace.Down &&
                           (_blockHeight > neighbours[i]?._blockHeight
                            || _blockYOffset < neighbours[i]._blockYOffset);
                _collidableFaces[i] = side || neighbour == null || neighbour._passable;

                if (!_blockLight.IsSource) continue;
                if (!CanLightPassThrough(face)) return;
                neighbours[i]?.ExpandLight(_position, sourceLight, opposite, this);
            }

            OnPlace(oldBlock, neighbours, triggerWorldUpdates);
        }

        public abstract void OnPlace(Block oldBlock, Block[] neighbours, bool triggerWorldUpdates);

        public void OnRemove0(Block newBlock, Block[] neighbours) {
            foreach (var source in _blockLight.Sources.Keys) {
                RemoveLight(source);
            }

            OnRemove(newBlock);
        }

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

        public void TriggerLightChange(Block[] neighbours) {
            OnSelfLightChange();
            //TODO
        }

        private void ExpandLight(Vector3i source, int light, BlockFace from, Block fromBlock) {
            if (!CanLightBePassedFrom(from, fromBlock)) return;
            
            var neighbours = new Block[6];
            var chunkPosition = _position - (_chunk.Position << Chunk.WorldPositionShift);
            _chunk.GetNeighbourBlocks(neighbours, _position, chunkPosition);

            if (_blockLight.AddSource(source, light)) {
                TriggerLightChange(neighbours);
            }
            light -= _blockLight.LightPassReduction;
            if (light <= 0) return;
            for (var i = 0; i < neighbours.Length; i++) {
                var face = (BlockFace) i;
                if (!CanLightPassThrough(face)) continue;
                var opposite = BlockFaceMethods.GetOpposite(face);
                neighbours[i]?.ExpandLight(source, light, opposite, this);
            }
        }

        private void RemoveLight(Vector3i source) {
            
            var neighbours = new Block[6];
            var chunkPosition = _position - (_chunk.Position << Chunk.WorldPositionShift);
            _chunk.GetNeighbourBlocks(neighbours, _position, chunkPosition);

            if (_blockLight.RemoveSource(source)) {
                TriggerLightChange(neighbours);
            }

            foreach (var neighbour in neighbours) {
                if (neighbour.BlockLight.TryGetSourceLight(source, out var nLight) && nLight < _blockLight.Light) {
                    neighbour.RemoveLight(source);
                }
            }
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