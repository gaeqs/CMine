using System;
using OpenTK;

namespace CMineNew.Render{
    public class PhysicCamera : Camera{
        public const float DefaultMovementAmplification = 30;
        public const float DefaultRotationAmplification = 40;

        private Vector3d _restPosition;
        private Vector3d _toPosition;
        private Vector2 _toRotation;
        private float _movementAmplification, _rotationAmplification;

        private float _cameraVelocity;
        private bool _onGround;
        private float _bobbingTimer;

        public PhysicCamera(Vector3d position, Vector2 rotation, Vector3 up, float fov)
            : base(position, rotation, up, fov) {
            _toPosition = position;
            _movementAmplification = DefaultMovementAmplification;
            _rotationAmplification = DefaultRotationAmplification;
            _cameraVelocity = 0;
            _onGround = true;
            ResetBobbingTimer();
        }

        public override Vector3d Position {
            set {
                _restPosition = value;
                _toPosition = value;
            }
        }

        public override Vector2 Rotation {
            set {
                base.Rotation = value;
                _toRotation = _rotation;
            }
        }

        public Vector3d RestPosition {
            get => _restPosition;
            set => _restPosition = value;
        }

        public Vector3d ToPosition {
            get => _toPosition;
            set => _toPosition = value;
        }

        public Vector2 ToRotation {
            get => _toRotation;
            set => _toRotation = value;
        }

        public float MovementAmplification {
            get => _movementAmplification;
            set => _movementAmplification = value;
        }

        public float RotationAmplification {
            get => _rotationAmplification;
            set => _rotationAmplification = value;
        }


        public float CameraVelocity => _cameraVelocity;

        public bool OnGround {
            get => _onGround;
            set => _onGround = value;
        }

        public void Tick(long delay) {
            _cameraVelocity =
                (float) Math.Max(
                    Math.Min(
                        ((_toPosition - _restPosition) * new Vector3d(1, 0, 1) * _movementAmplification).Length -
                        0.002f, 10), 0);
            var h = delay / CMine.TicksPerSecondF;
            if (_toPosition != _restPosition) {
                var velocity = (_toPosition - _restPosition) * _movementAmplification;
                var oldPos = _restPosition;
                _restPosition += velocity * h;
                if (Vector3d.Dot(_toPosition - oldPos, _toPosition - _restPosition) < 0) {
                    _restPosition = _toPosition;
                }

                _requiresRecalculation = true;
            }

            if (_rotation != _toRotation) {
                var velocity = (_toRotation - _rotation) * _rotationAmplification;
                _rotation += velocity * h;
                if (Vector2.Dot(_toRotation - _rotation, _toRotation - _rotation) < 0) {
                    _rotation = _toRotation;
                }

                Rotation = _rotation;

                _requiresRecalculation = true;
            }

            UpdateBobbing(h);
        }

        private void UpdateBobbing(float delta) {
            const float bobbingSpeed = 1.8f;
            const float bobbingAmplitude = 0.05f;

            _bobbingTimer += bobbingSpeed * _cameraVelocity * delta * (_onGround ? 1 : 0);
            var deltaMovement = new Vector3(
                0,
                (float) Math.Abs(Math.Sin(_bobbingTimer) * bobbingAmplitude),
                (float) Math.Cos(_bobbingTimer) * bobbingAmplitude);

            var matrix = Matrix3.CreateRotationY(_rotation.Y);
            deltaMovement = matrix * deltaMovement;
            _position = _restPosition + new Vector3d(deltaMovement.X, deltaMovement.Y, deltaMovement.Z);


            if (_bobbingTimer > Math.PI * 2)
                _bobbingTimer -= (float) Math.PI * 2;
        }

        private void ResetBobbingTimer() {
            _bobbingTimer = (float) Math.PI / 2;
        }
    }
}