using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CMineNew.Entities;
using CMineNew.Entities.Controller;
using CMineNew.Geometry;
using CMineNew.Map.BlockData;
using CMineNew.RayTrace;
using CMineNew.Render;
using CMineNew.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace CMineNew.Map{
    public class World : Room{
        private readonly Camera _camera;
        private readonly BlockRayTracer _blockRayTracer;
        private readonly WorldGBuffer _gBuffer;

        private readonly Dictionary<Vector3i, ChunkRegion> _chunkRegions;

        private readonly HashSet<Entity> _entities;
        private readonly Player _player;

        private readonly Collection<StaticText> _staticTexts;

        public World(string name) : base(name) {
            Background = Color4.Aqua;

            _camera = new Camera(new Vector3(0), new Vector3(0, 0, 1), new Vector3(0, 1, 0), 110);
            _blockRayTracer = new BlockRayTracer(this, _camera.Position, _camera.LookAt, 5);
            _gBuffer = new WorldGBuffer(CMine.Window);

            _chunkRegions = new Dictionary<Vector3i, ChunkRegion>();

            _entities = new HashSet<Entity>();
            _player = new Player(Guid.NewGuid(), this, new Vector3(20, 100, 20), null);
            _player.Controller = new LocalPlayerController(_player, _camera);
            _entities.Add(_player);

            _staticTexts = new Collection<StaticText>();
        }

        public Camera Camera => _camera;

        public BlockRayTracer BlockRayTracer => _blockRayTracer;

        public Dictionary<Vector3i, ChunkRegion> ChunkRegions => _chunkRegions;

        public WorldGBuffer GBuffer => _gBuffer;

        public HashSet<Entity> Entities => _entities;

        public Player Player => _player;

        public Collection<StaticText> StaticTexts => _staticTexts;

        public ChunkRegion GetChunkRegion(Vector3i position) {
            return ChunkRegions.TryGetValue(position, out var region) ? region : null;
        }

        public ChunkRegion GetChunkRegionFromWorldPosition(Vector3i position) {
            return ChunkRegions.TryGetValue(position >> 6, out var region) ? region : null;
        }

        public Chunk GetChunk(Vector3i position) {
            return ChunkRegions.TryGetValue(position >> 2, out var region)
                ? region.GetChunkFromChunkPosition(position)
                : null;
        }

        public Chunk GetChunkFromWorldPosition(Vector3i position) {
            return ChunkRegions.TryGetValue(position >> 6, out var region)
                ? region.GetChunkFromChunkPosition(position >> 4)
                : null;
        }

        public Block GetBlock(Vector3i position) {
            return GetChunkFromWorldPosition(position)?.GetBlockFromWorldPosition(position);
        }

        public void SetBlock(Block block, Vector3i position) {
            var regionPosition = position >> 6;
            var chunkPosition = position >> 4;

            var region = GetChunkRegion(regionPosition);
            if (region == null) {
                region = new ChunkRegion(this, regionPosition);
                ChunkRegions[regionPosition] = region;
            }

            var chunk = region.GetChunkFromChunkPosition(chunkPosition);
            if (chunk == null) {
                chunk = new Chunk(region, chunkPosition);
                region.SetChunk(chunk, chunkPosition - (regionPosition << 2));
            }

            chunk.SetBlockFromWorldPosition(block, position);
        }

        public override void Tick(long delay) {
            foreach (var entity in _entities) {
                entity.Tick(delay);
            }

            foreach (var staticText in _staticTexts) {
                staticText.Tick(delay, this);
            }

            _camera.Position = _player.Position + new Vector3(0, _player.EyesHeight, 0);
            _camera.SetRotation(_player.HeadRotation);
        }

        public override void Draw() {
            //Bind GBuffer and draw background.
            _gBuffer.Bind();
            GL.ClearColor(0, 0, 0, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.DepthMask(true);
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);
            _gBuffer.DrawBackground(_background);

            //Starts GBuffer drawing.

            GL.Enable(EnableCap.DepthTest);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, CMine.Textures.Texture);
            lock (_chunkRegions) {
                foreach (var region in _chunkRegions.Values) {
                    if (region.Deleted) continue;
                    if (_camera.IsVisible(region)) {
                        region.Render.Draw();
                    }
                    else {
                        region.Render.FlushInBackground();
                    }
                }
            }

            DrawSelectedBlock();

            //Draws GBuffer squad.
            _gBuffer.Draw(Vector3.One, 0.5f);

            //Transfers depth buffer to main FBO.
            _gBuffer.TransferDepthBufferToMainFbo();

            //Draws objects in main FBO.
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, CMine.Textures.Texture);

            lock (_chunkRegions) {
                foreach (var region in _chunkRegions.Values) {
                    region.Render.DrawAfterPostRender();
                }
            }

            //Draws front data

            GL.Disable(EnableCap.DepthTest);
            foreach (var staticText in _staticTexts) {
                staticText.Draw();
            }

            //TODO draw front data.
        }

        private void DrawSelectedBlock() {
            _blockRayTracer.Reset(_camera.Position, _camera.LookAt);
            _blockRayTracer.Run();
            _blockRayTracer.Result?.BlockModel.DrawLines(_camera, _blockRayTracer.Result.Position);
        }

        public override void KeyPush(KeyboardKeyEventArgs args) {
            _player.Controller?.HandleKeyPush(args);
            base.KeyPush(args);
            switch (args.Key) {
                case Key.R:
                    _player.Position = new Vector3(20, 100, 20);
                    _player.Velocity = Vector3.Zero;
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

        public override void MouseMove(MouseMoveEventArgs args) {
            _player.Controller?.HandleMouseMove(args);
        }
    }
}