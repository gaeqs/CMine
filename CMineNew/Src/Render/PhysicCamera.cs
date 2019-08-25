using OpenTK;

namespace CMineNew.Render{
    public class PhysicCamera : Camera{

        public const float DefaultMovementAmplification = 30;
        public const float DefaultRotationAmplification = 40;
        
        private Vector3 _toPosition;
        private Vector2 _toRotation;
        private float _movementAmplification, _rotationAmplification;

        public PhysicCamera(Vector3 position, Vector2 rotation, Vector3 up, float fov)
            : base(position, rotation, up, fov) {
            _toPosition = position;
            _movementAmplification = DefaultMovementAmplification;
            _rotationAmplification = DefaultRotationAmplification;
        }

        public override Vector3 Position {
            set {
                base.Position = value;
                _toPosition = value;
            }
        }

        public override Vector2 Rotation {
            set {
                base.Rotation = value;
                _toRotation = _rotation;
            }
        }

        public Vector3 ToPosition {
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

        public void Tick(long delay) {
            var h = delay / CMine.TicksPerSecondF;
            if (_toPosition != _position) {
                var velocity = (_toPosition - _position) * _movementAmplification;
                var oldPos = _position;
                _position += velocity * h;
                if (Vector3.Dot(_toPosition - oldPos, _toPosition - _position) < 0) {
                    _position = _toPosition;
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
        }
    }
}