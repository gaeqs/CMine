using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CMineNew.DataStructure.List;
using CMineNew.Entities;
using CMineNew.Entities.Controller;
using CMineNew.Geometry;
using CMineNew.Light;
using CMineNew.Map.BlockData;
using CMineNew.Map.BlockData.Snapshot;
using CMineNew.Map.Generator;
using CMineNew.Map.Generator.Unloaded;
using CMineNew.Map.Task;
using CMineNew.Render;
using CMineNew.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace CMineNew.Map{
    public class World : Room{
        private readonly string _folder;

        private readonly WorldRenderData _renderData;

        private readonly WorldGenerator _worldGenerator;
        private readonly AsyncChunkGenerator _asyncChunkGenerator;
        private readonly AsyncChunkTrashCan _asyncChunkTrashCan;
        private readonly WorldTaskManager _worldTaskManager;
        private readonly UnloadedChunkGenerationManager _unloadedChunkGenerationManager;

        private readonly Dictionary<Vector3i, ChunkRegion> _chunkRegions;
        private readonly Dictionary<Vector2i, World2dRegion> _regions2d;
        private readonly ELinkedList<ChunkRegion> _tickRegions;

        private readonly HashSet<Entity> _entities;
        private readonly Player _player;

        private readonly Collection<StaticText> _staticTexts;

        private readonly object _regionsLock = new object();

        public World(string name) : base(name) {
            _folder = CMine.MainFolder + Path.DirectorySeparatorChar + name;
            Directory.CreateDirectory(_folder);
            Background = Color4.Aqua;

            _renderData = new WorldRenderData();

            _chunkRegions = new Dictionary<Vector3i, ChunkRegion>();
            _regions2d = new Dictionary<Vector2i, World2dRegion>();
            _tickRegions = new ELinkedList<ChunkRegion>();

            _staticTexts = new Collection<StaticText>();

            _worldTaskManager = new WorldTaskManager();
            _worldGenerator = new DefaultWorldGenerator(this, new Random().Next());

            _entities = new HashSet<Entity>();
            _player = new Player(Guid.NewGuid(), this, new Vector3(20, 100, 20), null);
            _player.Controller = new LocalPlayerController(_player, _renderData.Camera);
            _entities.Add(_player);

            _unloadedChunkGenerationManager = new UnloadedChunkGenerationManager(this);
            _asyncChunkTrashCan = new AsyncChunkTrashCan(this);
            _asyncChunkTrashCan.StartThread();
            _asyncChunkGenerator = new AsyncChunkGenerator(this);
            _asyncChunkGenerator.StartThread();

            _asyncChunkGenerator.GenerateChunkArea = true;
        }

        public string Folder => _folder;

        public PhysicCamera Camera => _renderData.Camera;

        public WorldRenderData RenderData => _renderData;

        public WorldGenerator WorldGenerator => _worldGenerator;

        public AsyncChunkGenerator AsyncChunkGenerator => _asyncChunkGenerator;

        public AsyncChunkTrashCan AsyncChunkTrashCan => _asyncChunkTrashCan;

        public WorldTaskManager WorldTaskManager => _worldTaskManager;

        public Dictionary<Vector3i, ChunkRegion> ChunkRegions {
            get {
                lock (_regionsLock) {
                    return _chunkRegions;
                }
            }
        }

        public Dictionary<Vector2i, World2dRegion> Regions2d => _regions2d;

        public object RegionsLock => _regionsLock;

        public HashSet<Entity> Entities => _entities;

        public Player Player => _player;

        public Collection<StaticText> StaticTexts => _staticTexts;

        public UnloadedChunkGenerationManager UnloadedChunkGenerationManager => _unloadedChunkGenerationManager;

        public ChunkRegion GetChunkRegion(Vector3i position) {
            lock (_regionsLock) {
                return _chunkRegions.TryGetValue(position, out var region) ? region : null;
            }
        }

        public ChunkRegion GetChunkRegionFromWorldPosition(Vector3i position) {
            lock (_regionsLock) {
                return _chunkRegions.TryGetValue(position >> 6, out var region) ? region : null;
            }
        }

        public Chunk GetChunk(Vector3i position) {
            lock (_regionsLock) {
                return _chunkRegions.TryGetValue(position >> 2, out var region)
                    ? region.GetChunkFromChunkPosition(position)
                    : null;
            }
        }

        public Chunk GetChunkFromWorldPosition(Vector3i position) {
            lock (_regionsLock) {
                return _chunkRegions.TryGetValue(position >> 6, out var region)
                    ? region.GetChunkFromChunkPosition(position >> 4)
                    : null;
            }
        }

        public Block GetBlock(Vector3i position) {
            return GetChunkFromWorldPosition(position)?.GetBlockFromWorldPosition(position);
        }

        public Block SetBlock(BlockSnapshot snapshot, Vector3i position) {
            return SetBlock(snapshot, position, b => true);
        }

        public Block SetBlock(BlockSnapshot snapshot, Vector3i position, Func<Block, bool> canBeReplaced) {
            var regionPosition = position >> 6;
            var chunkPosition = position >> 4;

            var region = GetChunkRegion(regionPosition);
            if (region == null) {
                region = new ChunkRegion(this, regionPosition);
                region.LoadIfDeleted();
                lock (_regionsLock) {
                    _chunkRegions[regionPosition] = region;
                }
            }

            var chunk = region.GetChunkFromChunkPosition(chunkPosition);
            if (chunk != null) return chunk.SetBlockFromWorldPosition(snapshot, position, canBeReplaced);
            _unloadedChunkGenerationManager.AddBlock(position, snapshot, true);
            return null;
        }

        public Chunk CreateChunk(Vector3i position) {
            var regionPosition = position >> 2;

            var region = GetChunkRegion(regionPosition);
            if (region == null) {
                region = new ChunkRegion(this, regionPosition);
                region.LoadIfDeleted();
                lock (_regionsLock) {
                    _chunkRegions[regionPosition] = region;
                }
            }
            else {
                region.LoadIfDeleted();
            }

            var chunk = region.GetChunkFromChunkPosition(position);
            if (chunk != null) return chunk;

            var chunkPositionInRegion = position - (regionPosition << 2);
            if (region.TryLoadSavedChunk(chunkPositionInRegion, out chunk)) {
                _unloadedChunkGenerationManager.OnChunkLoad(chunk);
                return chunk;
            }

            chunk = new Chunk(region, position);
            _worldGenerator.GenerateChunkData(chunk);
            _unloadedChunkGenerationManager.FlushToAllChunks();
            chunk.Natural = true;
            region.SetChunk(chunk, chunkPositionInRegion);

            return chunk;
        }

        public World2dRegion GetOrCreate2dRegion(Vector2i regionPosition) {
            if (_regions2d.TryGetValue(regionPosition, out var value)) {
                return value;
            }

            var region = new World2dRegion(this, regionPosition);
            region.CalculateBiomes();
            region.CalculateHeights();
            region.CalculateInterpolatedHeightsAndColors();
            _regions2d.Add(regionPosition, region);
            return region;
        }

        public override void Tick(long delay) {
            _worldTaskManager.Tick(delay);

            lock (_regionsLock) {
                _tickRegions.Clear();
                foreach (var region in _chunkRegions.Values) {
                    _tickRegions.Add(region);
                }

                foreach (var region in _tickRegions) {
                    region.Tick(delay);
                }
            }

            foreach (var entity in _entities) {
                entity.Tick(delay);
            }

            foreach (var staticText in _staticTexts) {
                staticText.Tick(delay, this);
            }

            _renderData.CameraTick(_player, delay);
        }

        public override void Draw() {
            //Bind GBuffer and draw background.
            _renderData.BindGBuffer();
            GL.ClearColor(0, 0, 0, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.DepthMask(true);
            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);

            //Starts GBuffer drawing.

            GL.Enable(EnableCap.DepthTest);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, CMine.Textures.Texture);
            lock (_regionsLock) {
                foreach (var region in _chunkRegions.Values.Where(region => !region.Deleted)) {
                    if (_renderData.Camera.IsVisible(region)) {
                        region.Render.Draw();
                    }
                    else {
                        region.Render.FlushInBackground();
                    }
                }
            }

            DrawSelectedBlock();

            //Draws background
            GL.DepthFunc(DepthFunction.Lequal);
            _renderData.DrawSkyBox();
            GL.DepthFunc(DepthFunction.Less);

            //Transfers depth buffer to main FBO.
            _renderData.TransferDepthBufferToMainFbo();

            _renderData.DrawLights();

            //Draws GBuffer quad.
            _renderData.DrawGBuffer(_player.EyesOnWater);

            //Draws objects in main FBO.
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, CMine.Textures.Texture);

            lock (_regionsLock) {
                foreach (var region in _chunkRegions.Values) {
                    region.Render.DrawAfterPostRender();
                }
            }

            //Draws front data

            GL.Disable(EnableCap.DepthTest);
            foreach (var staticText in _staticTexts) {
                staticText.Draw();
            }

            _renderData.DrawPointer();
        }

        private void DrawSelectedBlock() {
            var tracer = _player.BlockRayTracer;
            _renderData.DrawSelectedBlock(tracer.Result);
        }

        public override void KeyPush(KeyboardKeyEventArgs args) {
            _player.Controller?.HandleKeyPush(args);
            base.KeyPush(args);
            switch (args.Key) {
                case Key.R:
                    _player.Position = new Vector3(20, 100, 20);
                    _player.Velocity = Vector3.Zero;
                    break;
                case Key.K:
                    _renderData.LightManager.AddPointLight(new PointLight(
                        _player.Position + new Vector3(0, _player.EyesHeight, 0),
                        new Vector3(1, 1, 1), new Vector3(1, 1, 1),
                        new Vector3(1, 1, 1), 1, 0.5f, 0.3f));
                    break;
                case Key.L:
                    lock (_regionsLock) {
                        foreach (var region in _chunkRegions.Values) {
                            region.Save();
                        }
                    }

                    break;
            }
        }

        public override void KeyRelease(KeyboardKeyEventArgs args) {
            _player.Controller?.HandleKeyRelease(args);
        }


        public override void MousePush(MouseButtonEventArgs args) {
            _player.Controller?.HandleMousePush(args);
        }

        public override void MouseRelease(MouseButtonEventArgs args) {
            _player.Controller?.HandleMouseRelease(args);
        }

        public override void Close() {
            _asyncChunkGenerator.Kill();
            _asyncChunkTrashCan.Kill();
            lock (_regionsLock) {
                foreach (var region in _chunkRegions.Values) {
                    region.Delete();
                }

                _chunkRegions.Clear();
            }
        }
    }
}