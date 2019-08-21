using System;
using CMineNew.Map;
using CMineNew.Map.BlockData.Snapshot;
using CMineNew.Render;
using OpenTK;
using OpenTK.Input;

namespace CMineNew.Entities.Controller{
    public class LocalPlayerController : PlayerController{
        private const float RotationVelocity = 180;

        private Player _player;
        private readonly Camera _camera;
        private bool _w, _a, _s, _d, _control, _space;
        private float _toPitch, _toYaw;

        public LocalPlayerController(Player player, Camera camera) : base(player) {
            _player = player;
            _camera = camera;
        }

        public override void Tick(long dif) {
            var velocity = Vector3.Zero;
            if (_w && !_s) velocity += (_camera.LookAt * new Vector3(1, 0, 1)).Normalized();
            if (_s && !_w) velocity -= (_camera.LookAt * new Vector3(1, 0, 1)).Normalized();
            if (_d && !_a) velocity += _camera.U;
            if (_a && !_d) velocity -= _camera.U;

            velocity *= _control ? 180 : 100;
            velocity *= _player.OnGround ? 1 : 0.3f;

            var maximum = _control ? 5 : 3;

            var pVelocity = new Vector3(_player.Velocity.X, 0, _player.Velocity.Z);
            if (pVelocity.LengthSquared < maximum * maximum)
                _player.Velocity += velocity * dif / CMine.TicksPerSecondF;
            if (_space)
                _player.Jump();

            var rotation = _player.HeadRotation;
            var dPitch = _toPitch - rotation.X;
            var dYaw = _toYaw - rotation.Y;

            var rPitchVelocity = RotationVelocity * dif / CMine.TicksPerSecondF;
            var rYawVelocity = RotationVelocity * dif / CMine.TicksPerSecondF;

            rotation.X = dPitch < 0
                ? Math.Max(rotation.X - rPitchVelocity, _toPitch)
                : Math.Min(rotation.X + rPitchVelocity, _toPitch);
            rotation.Y = dYaw < 0
                ? Math.Max(rotation.Y - rYawVelocity, _toYaw)
                : Math.Min(rotation.Y + rYawVelocity, _toYaw);
            _player.HeadRotation = rotation;
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
            if (args.Button == MouseButton.Right) {
                var matInstance = new BlockSnapshotTallGrass();
                if (_player.BlockRayTracer.Result == null) return;
                var result = _player.BlockRayTracer.Result;
                var position = result.Position + BlockFaceMethods.GetRelative(_player.BlockRayTracer.Face);
                if (!matInstance.CanBePlaced(position, _player.World)) return;
                if (!matInstance.Passable &&
                    _player.CollisionBox.Collides(matInstance.BlockModel.BlockCollision, _player.Position,
                        position.ToFloat(), null, out var data) && data.Distance > 0.01f) return;
                _player.World.SetBlock(matInstance, position);
            }
            else if (args.Button == MouseButton.Left) {
                if (_player.BlockRayTracer.Result == null) return;
                _player.World.SetBlock(new BlockSnapshotAir(), _player.BlockRayTracer.Result.Position);
            }
        }

        public override void HandleMouseRelease(MouseButtonEventArgs args) {
        }

        public override void HandleMouseMove(MouseMoveEventArgs args) {
            GameWindow window = CMine.Window;
            if (window.Focused) {
                var deltaX = Mouse.GetCursorState().X - (window.X + window.Width / 2);
                var deltaY = Mouse.GetCursorState().Y - (window.Y + window.Height / 2);

                if (deltaX == 0 && deltaY == 0) return;
                _toPitch -= deltaY / 100f;
                _toYaw += deltaX / 100f;

                if (_toPitch > Camera.ExtremePitch) _toPitch = Camera.ExtremePitch;
                else if (_toPitch < -Camera.ExtremePitch) _toPitch = -Camera.ExtremePitch;
                Mouse.SetPosition(window.X + window.Width / 2, window.Y + window.Height / 2);
            }
        }
    }
}