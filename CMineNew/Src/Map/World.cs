using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using CMineNew.Entities;
using CMineNew.Entities.Controller;
using CMineNew.Geometry;
using CMineNew.Map.BlockData;
using CMineNew.Map.BlockData.Snapshot;
using CMineNew.Map.Generator;
using CMineNew.Map.Generator.Unloaded;
using CMineNew.Map.Task;
using CMineNew.Render;
using CMineNew.Test;
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
        private readonly WorldTaskManager _worldTaskManager;
        private readonly UnloadedChunkGenerationManager _unloadedChunkGenerationManager;

        private readonly ConcurrentDictionary<Vector3i, ChunkRegion> _chunkRegions;
        private readonly ConcurrentDictionary<Vector2i, World2dRegion> _regions2d;

        private readonly HashSet<Entity> _entities;
        private readonly Player _player;

        private readonly Collection<StaticText> _staticTexts;

        private readonly DelayViewer _delayViewer;

        public World(string name, TrueTypeFont ttf) : base(name) {
            _folder = CMine.MainFolder + Path.DirectorySeparatorChar + name;
            Directory.CreateDirectory(_folder);
            Background = Color4.Aqua;

            _renderData = new WorldRenderData();

            _chunkRegions = new ConcurrentDictionary<Vector3i, ChunkRegion>();
            _regions2d = new ConcurrentDictionary<Vector2i, World2dRegion>();

            _staticTexts = new Collection<StaticText>();
            _delayViewer = new DelayViewer(ttf);
            _staticTexts.Add(_delayViewer);

            _worldTaskManager = new WorldTaskManager();

            if (Load(out var seed)) {
                _worldGenerator = new DefaultWorldGenerator(this, seed);
            }
            else {
                _worldGenerator = new DefaultWorldGenerator(this, new Random().Next());
            }

            _entities = new HashSet<Entity>();
            _player = new Player(Guid.NewGuid(), this, new Vector3(20, 100, 20), null);
            _player.Controller = new LocalPlayerController(_player, _renderData.Camera);
            _entities.Add(_player);

            _unloadedChunkGenerationManager = new UnloadedChunkGenerationManager(this);
            _unloadedChunkGenerationManager.Load();
            _asyncChunkGenerator = new AsyncChunkGenerator(this);
            _asyncChunkGenerator.StartThread();

            _asyncChunkGenerator.GenerateChunkArea = true;
        }

        public string Folder => _folder;

        public PhysicCamera Camera => _renderData.Camera;

        public WorldRenderData RenderData => _renderData;

        public WorldGenerator WorldGenerator => _worldGenerator;

        public AsyncChunkGenerator AsyncChunkGenerator => _asyncChunkGenerator;

        public WorldTaskManager WorldTaskManager => _worldTaskManager;

        public ConcurrentDictionary<Vector3i, ChunkRegion> ChunkRegions => _chunkRegions;

        public ConcurrentDictionary<Vector2i, World2dRegion> Regions2d => _regions2d;


        public HashSet<Entity> Entities => _entities;

        public Player Player => _player;

        public Collection<StaticText> StaticTexts => _staticTexts;

        public UnloadedChunkGenerationManager UnloadedChunkGenerationManager => _unloadedChunkGenerationManager;

        public DelayViewer DelayViewer => _delayViewer;

        public ChunkRegion GetChunkRegion(Vector3i position) {
            return _chunkRegions.TryGetValue(position, out var region) ? region : null;
        }

        public ChunkRegion GetChunkRegionFromWorldPosition(Vector3i position) {
            return _chunkRegions.TryGetValue(position >> 6, out var region) ? region : null;
        }

        public Chunk GetChunk(Vector3i position) {
            return _chunkRegions.TryGetValue(position >> 2, out var region)
                ? region.GetChunkFromChunkPosition(position)
                : null;
        }

        public Chunk GetChunkFromWorldPosition(Vector3i position) {
            return _chunkRegions.TryGetValue(position >> 6, out var region)
                ? region.GetChunkFromChunkPosition(position >> 4)
                : null;
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
                _chunkRegions[regionPosition] = region;
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
                _chunkRegions[regionPosition] = region;
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
            chunk.Natural = true;
            _worldGenerator.GenerateChunkData(chunk);
            _unloadedChunkGenerationManager.FlushToAllChunks();
            region.SetChunk(chunk, chunkPositionInRegion);

            return chunk;
        }

        public World2dRegion GetOrCreate2dRegion(Vector2i regionPosition) {
            if (_regions2d.TryGetValue(regionPosition, out var value)) {
                return value;
            }

            var region = new World2dRegion(this, regionPosition);
            if (!region.Load()) {
                region.CalculateBiomes();
                region.CalculateHeightsAndCreateSunlightData();
                region.CalculateInterpolatedHeightsAndColors();
            }
            else {
                Console.WriteLine("REGION " + regionPosition + " LOADED.");
            }

            _regions2d.TryAdd(regionPosition, region);


            return region;
        }

        public void Save() {
            var file = _folder + Path.DirectorySeparatorChar + "info.dat";
            var formatter = new BinaryFormatter();
            var stream = File.Open(file, FileMode.OpenOrCreate, FileAccess.Write);
            formatter.Serialize(stream, _worldGenerator.Seed);
            stream.Close();

            foreach (var region in _chunkRegions.Values) {
                region.Save();
            }


            foreach (var region in _regions2d.Values) {
                region.Save();
            }

            _unloadedChunkGenerationManager.Save();
        }

        public bool Load(out int seed) {
            seed = 0;
            var file = _folder + Path.DirectorySeparatorChar + "info.dat";
            if (!File.Exists(file)) return false;
            var formatter = new BinaryFormatter();
            var stream = File.Open(file, FileMode.OpenOrCreate, FileAccess.Read);
            seed = (int) formatter.Deserialize(stream);
            stream.Close();
            return true;
        }

        public override void Tick(long delay) {
            _worldTaskManager.Tick(delay);

            foreach (var region in _chunkRegions.Values) {
                region.Tick(delay);
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
            //Set shader data.

            _renderData.SetShaderData(new Vector3(-1, -1, -1).Normalized(), _player.EyesOnWater);

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

            var renders = (from region in _chunkRegions.Values.Where(region => !region.Deleted) where _renderData.Camera.IsVisible(region) select region.Render).ToList();
            foreach (var render in renders) {
                render.Draw();
            }

            DrawSelectedBlock();

            //Draws background
            GL.DepthFunc(DepthFunction.Lequal);
            _renderData.DrawSkyBox();
            GL.DepthFunc(DepthFunction.Less);

            //Transfers depth buffer to main FBO.
            _renderData.TransferDepthBufferToMainFbo();

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

            foreach (var render in renders) {
                render.DrawAfterPostRender();
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
                    var pos = new Vector3i(_player.Position);
                    for (var x = -7; x < 7; x++) {
                        for (var z = -7; z < 7; z++) {
                            var bPos = pos;
                            bPos.Add(x, -1, z);
                            SetBlock(new BlockSnapshotBricks(), bPos);
                        }
                    }

                    break;
                case Key.L:
                    Save();
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
            foreach (var region in _chunkRegions.Values) {
                region.Delete();
            }

            _chunkRegions.Clear();
        }
    }
}