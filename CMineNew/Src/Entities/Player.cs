using System;
using CMine.Collision;
using CMineNew.Entities.Controller;
using CMineNew.Geometry;
using CMineNew.Map;
using CMineNew.Map.BlockData.Type;
using CMineNew.RayTrace;
using CMineNew.Util;
using OpenTK;

namespace CMineNew.Entities{
    public class Player : PhysicEntity{
        //5 m/s
        public static readonly float OnGroundRunningForce =
            ForceUtils.CalculateForceFromDamping(DampingConstantGround, 5);

        //3 m/s
        public static readonly float OnGroundForce = ForceUtils.CalculateForceFromDamping(DampingConstantGround, 3);

        //2 m/s
        public static readonly float OnWaterForce = ForceUtils.CalculateForceFromDamping(DampingConstantWater, 2);

        public static readonly float OnAirRunningForce = ForceUtils.CalculateForceFromDamping(DampingConstantAir, 4f);

        //3 m/s
        public static readonly float OnAirForce = ForceUtils.CalculateForceFromDamping(DampingConstantAir, 3f);

        public static readonly float MaxWaterUpVelocity = 2.5f;
        public static readonly Vector3 OnWaterUpForce = new Vector3(0, 15 * Mass, 0);

        private PlayerController _controller;
        private float _eyesHeight;

        private Vector2 _headRotation;
        private readonly BlockRayTracer _blockRayTracer;
        private bool _eyesOnWater;

        public Player(Guid guid, World world, Vector3 position, PlayerController controller)
            : base(guid, world, position, new Aabb(-0.4f, 0, -0.4f, 0.8f, 1.8f, 0.8f)) {
            _eyesHeight = 1.7f;
            _controller = controller;
            _headRotation = Vector2.Zero;
            _blockRayTracer = new BlockRayTracer(world, _position + new Vector3(0, _eyesHeight, 0),
                world.Camera.LookAt, 5);
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

        public BlockRayTracer BlockRayTracer => _blockRayTracer;

        public bool EyesOnWater => _eyesOnWater;

        public void Move(Vector3 direction, bool run) {
            Vector3 force;
            if (_onWater) {
                force = OnWaterForce * direction;
            }
            else if (OnGround) {
                force = (run ? OnGroundRunningForce : OnGroundForce) * direction;
            }
            else {
                force = (run ? OnAirRunningForce : OnAirForce) * direction;
            }

            _force += force;
        }

        public void ManageJump() {
            if (_onWater) {
                if (_velocity.Y < MaxWaterUpVelocity) {
                    _force += OnWaterUpForce;
                }
            }
            else {
                if (_wasOnWater) {
                    var impulse = BlockWater.MaxWaterLevel - _waterBlock.WaterLevel;
                    _velocity += new Vector3(0, impulse / 1.5f, 0);
                }
                else {
                    Jump();
                }
            }
        }

        public override void Tick(long dif) {
            base.Tick(dif);
            _controller?.Tick(dif);

            _blockRayTracer.Reset(_position + new Vector3(0, _eyesHeight, 0), _world.Camera.LookAt);
            _blockRayTracer.Run();


            var eyesPosition = _position + new Vector3(0, _eyesHeight, 0);
            var eyesBlock = _world.GetBlock(new Vector3i(eyesPosition, true));
            _eyesOnWater = eyesBlock is BlockWater water && water.WaterHeight - water.Position.Y > _eyesHeight;
        }

        public override void UpdatePosition(Vector3 old) {
            var positionI = new Vector3i(_position) >> 4;
            if (new Vector3i(old) >> 4 != positionI) {
                _world.AsyncChunkGenerator.GenerateChunkArea = true;
            }
        }
    }
}