using System;
using CMine.Collision;
using CMineNew.Geometry;
using CMineNew.Map;
using CMineNew.Map.BlockData.Type;
using OpenTK;

namespace CMineNew.Entities{
    public class PhysicEntity : Entity{
        public static readonly float Mass = 10;
        public static readonly Vector3 OnGroundGravity = new Vector3(0, -21f * Mass, 0);
        public static readonly Vector3 WaterGravity = new Vector3(0, -3f * Mass, 0);

        public const float DampingConstantGround = -200;

        public const float DampingConstantWater = -550;

        //Doesn't afect Y axis.
        public const float DampingConstantAir = -50;

        protected Vector3 _force, _velocity;
        protected bool _onWater, _wasOnWater;
        protected long _lastOnGroundTick, _onGroundTicks;
        protected long _lastJumpTick;

        protected BlockWater _waterBlock;

        public PhysicEntity(World world, Vector3 position, Aabb collisionBox) : base(world, position, collisionBox) {
            _force = Vector3.Zero;
            _velocity = Vector3.Zero;
            _lastOnGroundTick = 0;
            _onGroundTicks = 0;
            _lastJumpTick = 0;
        }

        public PhysicEntity(Guid guid, World world, Vector3 position, Aabb collisionBox) : base(guid, world, position,
            collisionBox) {
            _force = Vector3.Zero;
            _velocity = Vector3.Zero;
            _lastOnGroundTick = 0;
            _lastJumpTick = 0;
        }

        public Vector3 Force {
            get => _force;
            set => _force = value;
        }

        public Vector3 Velocity {
            get => _velocity;
            set => _velocity = value;
        }

        public BlockWater WaterBlock => _waterBlock;


        public bool OnGround => DateTime.Now.Ticks - _lastOnGroundTick < CMine.TicksPerSecond / 10;

        public bool OnWater => _onWater;

        public long LastOnGroundTick => _lastOnGroundTick;

        public void Jump() {
            var now = DateTime.Now.Ticks;
            if (now - _lastOnGroundTick < CMine.TicksPerSecond / 10 &&
                now - _lastJumpTick > CMine.TicksPerSecond / 3 &&
                _onGroundTicks > CMine.TicksPerSecond / 10) {
                _velocity += new Vector3(0, 7, 0);
                _lastJumpTick = DateTime.Now.Ticks;
            }
        }

        public override void Tick(long dif) {
            _wasOnWater = _onWater;
            if (_world.GetBlock(new Vector3i(_position, true)) is BlockWater water
                && water.WaterHeight >= _position.Y - Math.Floor(_position.Y)) {
                _onWater = true;
                _waterBlock = water;
            }
            else {
                _onWater = false;
            }


            var oldPosition = _position;
            var h = dif / CMine.TicksPerSecondF;

            _force += (OnWater ? WaterGravity : OnGroundGravity) + CalculateDampingForce();
            _velocity += _force * h / Mass;
            _position += _velocity * h;
            _force = Vector3.Zero;
            ManageCollision(dif);
            UpdatePosition(oldPosition);
        }

        private Vector3 CalculateDampingForce() {
            float constant;
            if (_onWater) {
                constant = DampingConstantWater;
            }
            else {
                constant = OnGround ? DampingConstantGround : DampingConstantAir;
            }

            return constant * _velocity * new Vector3(1, 0, 1);
        }

        protected void ManageCollision(long dif) {
            var mix = (int) Math.Floor(_collisionBox.X + _position.X - 1);
            var miy = (int) Math.Floor(_collisionBox.Y + _position.Y - 1);
            var miz = (int) Math.Floor(_collisionBox.Z + _position.Z - 1);
            var max = (int) Math.Ceiling(_collisionBox.X + _position.X + _collisionBox.Width + 1);
            var may = (int) Math.Ceiling(_collisionBox.Y + _position.Y + _collisionBox.Height + 1);
            var maz = (int) Math.Ceiling(_collisionBox.Z + _position.Z + _collisionBox.Depth + 1);

            CollisionData closestCollision = null;
            for (var x = mix; x <= max; x++) {
                for (var y = miy; y <= may; y++) {
                    for (var z = miz; z <= maz; z++) {
                        var block = _world.GetBlock(new Vector3i(x, y, z));
                        if (block == null || block.Passable) continue;
                        if (!_collisionBox.Collides(block.BlockModel.BlockCollision, _position,
                            block.Position.ToFloat(), block.CollidableFaces, out var data)) continue;

                        if (closestCollision == null || closestCollision.Distance > data.Distance) {
                            closestCollision = data;
                        }

                        _position += data.Distance * BlockFaceMethods.GetRelative(data.BlockFace).ToFloat();
                        ReduceVelocity(data.BlockFace);

                        var now = DateTime.Now.Ticks;
                        if (data.BlockFace == BlockFace.Up) {
                            _lastOnGroundTick = now;
                            _onGroundTicks += dif;
                        }
                        else if (now - _lastOnGroundTick > CMine.TicksPerSecond / 3) {
                            _onGroundTicks = 0;
                        }
                    }
                }
            }
        }

        protected void ReduceVelocity(BlockFace face) {
            switch (face) {
                case BlockFace.Down:
                    if (_velocity.Y > 0)
                        _velocity.Y = 0;
                    break;
                case BlockFace.Up:
                    if (_velocity.Y < 0)
                        _velocity.Y = 0;
                    break;
                case BlockFace.North:
                    if (_velocity.Z < 0)
                        _velocity.Z = 0;
                    break;
                case BlockFace.South:
                    if (_velocity.Z > 0)
                        _velocity.Z = 0;
                    break;
                case BlockFace.East:
                    if (_velocity.X > 0)
                        _velocity.X = 0;
                    break;
                case BlockFace.West:
                    if (_velocity.X < 0)
                        _velocity.X = 0;
                    break;
            }
        }
    }
}