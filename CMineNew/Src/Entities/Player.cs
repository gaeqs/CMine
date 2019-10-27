using System;
using CMineNew.Collision;
using CMineNew.Entities.Controller;
using CMineNew.Geometry;
using CMineNew.Map;
using CMineNew.Map.BlockData.Type;
using CMineNew.RayTrace;
using CMineNew.Util;
using OpenTK;

namespace CMineNew.Entities{
    
    /// <summary>
    /// Represents a player.
    /// </summary>
    public class Player : PhysicEntity{
        /// <summary>
        /// The force applied when the player is running on ground. 5 m/s.
        /// </summary>
        public static readonly float OnGroundRunningForce =
            ForceUtils.CalculateForceFromDamping(DampingConstantGround, 5);

        /// <summary>
        /// The force applied when the player is walking on ground. 3 m/s.
        /// </summary>
        public static readonly float OnGroundForce = ForceUtils.CalculateForceFromDamping(DampingConstantGround, 3);

        /// <summary>
        /// The force applied when the player is moving on water. 2 m/s.
        /// </summary>
        public static readonly float OnWaterForce = ForceUtils.CalculateForceFromDamping(DampingConstantWater, 2);

        /// <summary>
        /// The force applied when the player is running on air. 4 m/s.
        /// </summary>
        public static readonly float OnAirRunningForce = ForceUtils.CalculateForceFromDamping(DampingConstantAir, 4f);

        /// <summary>
        /// The force applied when the player is moving on air. 3 m/s.
        /// </summary>
        public static readonly float OnAirForce = ForceUtils.CalculateForceFromDamping(DampingConstantAir, 3f);

        /// <summary>
        /// The maximum velocity the player can reach when moving vertically on water.
        /// </summary>
        public static readonly float MaxWaterUpVelocity = 2.5f;
        
        /// <summary>
        /// The force applied when the player is moving upwards on water.
        /// </summary>
        public static readonly Vector3 OnWaterUpForce = new Vector3(0, 15 * Mass, 0);

        private PlayerController _controller;
        private float _eyesHeight;

        private Vector2 _headRotation;
        private readonly BlockRayTracer _blockRayTracer;
        private bool _eyesOnWater;

        /// <summary>
        /// Creates a player.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="world">The world.</param>
        /// <param name="position">The position.</param>
        /// <param name="controller">The controller.</param>
        public Player(Guid guid, World world, Vector3 position, PlayerController controller)
            : base(guid, world, position, new Aabb(-0.4f, 0, -0.4f, 0.8f, 1.9f, 0.8f)) {
            _eyesHeight = 1.7f;
            _controller = controller;
            _headRotation = Vector2.Zero;
            _blockRayTracer = new BlockRayTracer(world, _position + new Vector3(0, _eyesHeight, 0),
                world.Camera.LookAt, 5);
        }

        /// <summary>
        /// The controller of the player.
        /// </summary>
        public PlayerController Controller {
            get => _controller;
            set => _controller = value;
        }

        /// <summary>
        /// The local height of the player's eyes.
        /// </summary>
        public float EyesHeight {
            get => _eyesHeight;
            set => _eyesHeight = value;
        }

        /// <summary>
        /// The rotation of the player's head.
        /// The X parameter is the pitch, while the Y parameter is the yaw.
        /// </summary>
        public Vector2 HeadRotation {
            get => _headRotation;
            set => _headRotation = value;
        }

        /// <summary>
        /// The player's block ray tracer. This is used to calculate the selected block.
        /// </summary>
        public BlockRayTracer BlockRayTracer => _blockRayTracer;

        /// <summary>
        /// Whether the player's eyes are on water. This is used to apply the water shading.
        /// </summary>
        public bool EyesOnWater => _eyesOnWater;

        /// <summary>
        /// Moves the player towards the given direction.
        /// </summary>
        /// <param name="direction">A normalized vector, representing the direction.</param>
        /// <param name="run">Whether the player is running.</param>
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

        /// <summary>
        /// Manages the vertical movement of the player.
        /// </summary>
        public void ManageJump() {
            //If the player is on water, apply vertical force.
            if (_onWater) {
                if (_velocity.Y < MaxWaterUpVelocity) {
                    _force += OnWaterUpForce;
                }
            }
            else {
                //If the player was on water, apply an impulse to allow it to reach the surface.
                //This must be standardized.
                if (_wasOnWater) {
                    var impulse = Math.Max(BlockWater.MaxWaterLevel - _waterBlock.WaterLevel, 1);
                    _velocity += new Vector3(0, impulse / 1.5f, 0);
                }
                else {
                    //If the player is on ground, try to jump.
                    Jump();
                }
            }
        }

        public override void Tick(long dif) {
            base.Tick(dif);
            
            //Calls the controller.
            _controller?.Tick(dif);
            
            //Runs the block ray tracer.
            _blockRayTracer.Reset(_position + new Vector3(0, _eyesHeight, 0), _world.Camera.LookAt);
            _blockRayTracer.Run();


            //Calculates whether the eyes of the player are on water.
            var eyesPosition = _position + new Vector3(0, _eyesHeight, 0);
            var eyesBlock = _world.GetBlock(new Vector3i(eyesPosition, true));
            _eyesOnWater = eyesBlock is BlockWater water && water.WaterHeight + water.Position.Y - _position.Y >= _eyesHeight;
        }

        public override void RenderTick(long dif) {
            base.RenderTick(dif);
            _controller?.RenderTick(dif);
        }

        /// <summary>
        /// When a position update is sent and the chunk where the player is has changed,
        /// sends a request to load nearby chunks and unloaded far ones.
        ///
        /// This method may be improved if multi-player mode is added.
        /// </summary>
        /// <param name="old">The old position.</param>
        public override void UpdatePosition(Vector3 old) {
            var positionI = new Vector3i(_position) >> 4;
            if (new Vector3i(old) >> 4 != positionI) {
                _world.AsyncChunkGenerator.GenerateChunkArea = true;
            }
        }
    }
}