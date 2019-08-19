using System;
using CMine.Collision;
using CMineNew.Entities.Controller;
using CMineNew.Geometry;
using CMineNew.Map;
using OpenTK;

namespace CMineNew.Entities{
    public class Player : PhysicEntity{
        private PlayerController _controller;
        private float _eyesHeight;

        private Vector2 _headRotation;

        public Player(Guid guid, World world, Vector3 position, PlayerController controller)
            : base(guid, world, position, new Aabb(-0.4f, 0, -0.4f, 0.8f, 1.8f, 0.8f)) {
            _eyesHeight = 1.7f;
            _controller = controller;
            _headRotation = Vector2.Zero;
        }

        public PlayerController Controller {
            get => _controller;
            set => _controller = value;
        }

        public float EyesHeight {
            get => _eyesHeight;
            set => _eyesHeight = value;
        }

        //X = Pitch, Y = Yaw
        public Vector2 HeadRotation {
            get => _headRotation;
            set => _headRotation = value;
        }

        public override void Tick(long dif) {
            base.Tick(dif);
            _controller?.Tick(dif);
        }

        public override void UpdatePosition(Vector3 old) {
            var positionI = new Vector3i(_position) >> 4;
            if (new Vector3i(old) >> 4 != positionI) {
                //_world.AsyncChunkGenerator.GenerateChunkArea = true;
            }
        }
    }
}