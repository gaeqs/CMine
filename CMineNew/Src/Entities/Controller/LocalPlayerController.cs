using CMineNew.Map;
using CMineNew.Map.BlockData.Snapshot;
using CMineNew.Render;
using OpenTK;
using OpenTK.Input;

namespace CMineNew.Entities.Controller {
    /// <summary>
    /// Represents a PlayerControlled that translates mouse and keyboard inputs intro player's actions.
    /// </summary>
    public class LocalPlayerController : PlayerController {
        private readonly Player _player;
        private readonly Camera _camera;
        private bool _w, _a, _s, _d, _control, _space;
        private Vector2 _lastMousePosition;

        /// <summary>
        /// Creates the controller.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="camera">The world's camera.</param>
        public LocalPlayerController(Player player, Camera camera) : base(player) {
            _player = player;
            _camera = camera;
            var mouse = Mouse.GetState();
            _lastMousePosition = new Vector2(mouse.X, mouse.Y);
        }

        public override void Tick(long dif) {
            //Calculates the movement's direction.
            var direction = Vector3.Zero;
            if (_w && !_s) {
                var add = _camera.LookAt * new Vector3(1, 0, 1);
                add.NormalizeFast();
                direction -= add;
            }

            if (_s && !_w) {
                var add = _camera.LookAt * new Vector3(1, 0, 1);
                add.NormalizeFast();
                direction += add;
            }

            if (_d && !_a) direction -= _camera.U;
            if (_a && !_d) direction += _camera.U;

            //Moves the player.
            _player.Move(direction, _control);

            //If space is pressed, try to jump.
            if (_space) {
                _player.ManageJump();
            }
        }

        public override void RenderTick(long dif) {
            //Handle mouse movement.
            HandleMouseMovement();
        }

        public override void HandleKeyPush(KeyboardKeyEventArgs eventArgs) {
            switch (eventArgs.Key) {
                case Key.W:
                    _w = true;
                    break;
                case Key.A:
                    _a = true;
                    break;
                case Key.S:
                    _s = true;
                    break;
                case Key.D:
                    _d = true;
                    break;
                case Key.ControlLeft:
                    _control = true;
                    break;
                case Key.Space:
                    _space = true;
                    break;
            }
        }

        public override void HandleKeyRelease(KeyboardKeyEventArgs eventArgs) {
            switch (eventArgs.Key) {
                case Key.W:
                    _w = false;
                    break;
                case Key.A:
                    _a = false;
                    break;
                case Key.S:
                    _s = false;
                    break;
                case Key.D:
                    _d = false;
                    break;
                case Key.ControlLeft:
                    _control = false;
                    break;
                case Key.Space:
                    _space = false;
                    break;
            }
        }

        public override void HandleMousePush(MouseButtonEventArgs args) {
            //This method is provisional! It will be reformed when inventories are added.
            if (args.Button == MouseButton.Right) {
                var matInstance = _player.Inventory.Hotbar.SelectedBlock;
                if (_player.BlockRayTracer.Result == null) return;
                var result = _player.BlockRayTracer.Result;
                var position = result.Position + BlockFaceMethods.GetRelative(_player.BlockRayTracer.Face);
                if (!matInstance.CanBePlaced(position, _player.World)) return;
                if (!matInstance.Passable && _player.CollisionBox.Collides(matInstance.BlockModel.BlockCollision,
                        _player.Position,
                        position.ToFloat(), null, out var data) && data.Distance > 0.01f) return;
                _player.World.SetBlock(matInstance, position);
            }
            else if (args.Button == MouseButton.Left) {
                if (_player.BlockRayTracer.Result == null) return;
                _player.World.SetBlock(BlockSnapshotAir.Instance, _player.BlockRayTracer.Result.Position);
            }
        }

        public override void HandleMouseRelease(MouseButtonEventArgs args) {
        }

        /// <summary>
        /// Handles the mouse movement.
        /// </summary>
        public void HandleMouseMovement() {
            GameWindow window = CMine.Window;
            if (window.Focused) {
                var mouse = Mouse.GetState();
                var newPosition = new Vector2(mouse.X, mouse.Y);

                var deltaX = newPosition.X - _lastMousePosition.X;
                var deltaY = newPosition.Y - _lastMousePosition.Y;

                _lastMousePosition = newPosition;

                var rotation = _player.HeadRotation;
                rotation.X -= deltaY / 200f;
                rotation.Y += deltaX / 200f;

                if (rotation.X > Camera.ExtremePitch) rotation.X = Camera.ExtremePitch;
                else if (rotation.X < -Camera.ExtremePitch) rotation.X = -Camera.ExtremePitch;
                _player.HeadRotation = rotation;

                //Moves the cursor to the center of the window.
                Mouse.SetPosition(window.X + window.Width / 2, window.Y + window.Height / 2);
            }
        }

        public override void HandleMouseWheel(MouseWheelEventArgs eventArgs) {
            _player.Inventory.Hotbar.Selected += eventArgs.Delta > 0 ? -1 : 1;
        }
    }
}