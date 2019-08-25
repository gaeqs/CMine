using System;
using System.Collections.Generic;
using CMine.Collision;
using CMineNew.Geometry;
using CMineNew.Map;
using CMineNew.Map.BlockData;
using CMineNew.Map.BlockData.Type;
using OpenTK;

namespace CMineNew.Entities{
    public class PhysicEntity : Entity{
        public static readonly float Mass = 10;
        public static readonly Vector3 OnGroundGravity = new Vector3(0, -21f * Mass, 0);
        public static readonly Vector3 WaterGravity = new Vector3(0, -3f * Mass, 0);
        private static readonly Vector3 BlockCenter = new Vector3(0.5f);
        private static readonly Vector3 MaxDistance = new Vector3(0.4f);

        public const float DampingConstantGround = -200;

        public const float DampingConstantWater = -550;

        //Doesn't afect Y axis.
        public const float DampingConstantAir = -50;

        protected Vector3 _force, _velocity;
        protected bool _onWater, _wasOnWater, _onGroundInstant;
        protected long _lastOnGroundTick, _onGroundTicks;
        protected long _lastJumpTick;

        protected BlockWater _waterBlock;
        private List<Block> _collisionBlocks, _collisionUpBlocks;

        public PhysicEntity(World world, Vector3 position, Aabb collisionBox) : base(world, position, collisionBox) {
            _force = Vector3.Zero;
            _velocity = Vector3.Zero;
            _lastOnGroundTick = 0;
            _onGroundTicks = 0;
            _lastJumpTick = 0;
            _collisionBlocks = new List<Block>();
            _collisionUpBlocks = new List<Block>();
        }

        public PhysicEntity(Guid guid, World world, Vector3 position, Aabb collisionBox) : base(guid, world, position,
            collisionBox) {
            _force = Vector3.Zero;
            _velocity = Vector3.Zero;
            _lastOnGroundTick = 0;
            _lastJumpTick = 0;
            _collisionBlocks = new List<Block>();
            _collisionUpBlocks = new List<Block>();
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

        public bool OnGroundInstant => _onGroundInstant;

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
            _force = Vector3.Zero;

            var to = _velocity * h;

            while (Math.Abs(to.X) > 0 || Math.Abs(to.Y) > 0 || Math.Abs(to.Z) > 0) {
                var movement = Vector3.ComponentMax(Vector3.ComponentMin(to, MaxDistance), -MaxDistance);
                _position += movement;
                ManageCollision(dif);
                to -= movement;
            }

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
            GetCollisionBlocks(dif);
            ManageCollisionBlocks();
        }

        protected void GetCollisionBlocks(long dif) {
            var mix = (int) Math.Floor(_collisionBox.X + _position.X - 1);
            var miy = (int) Math.Floor(_collisionBox.Y + _position.Y - 1);
            var miz = (int) Math.Floor(_collisionBox.Z + _position.Z - 1);
            var max = (int) Math.Ceiling(_collisionBox.X + _position.X + _collisionBox.Width + 1);
            var may = (int) Math.Ceiling(_collisionBox.Y + _position.Y + _collisionBox.Height + 1);
            var maz = (int) Math.Ceiling(_collisionBox.Z + _position.Z + _collisionBox.Depth + 1);

            _collisionBlocks.Clear();
            _collisionUpBlocks.Clear();

            for (var x = mix; x <= max; x++) {
                for (var y = miy; y <= may; y++) {
                    for (var z = miz; z <= maz; z++) {
                        var block = _world.GetBlock(new Vector3i(x, y, z));
                        if (block == null || block.Passable) continue;
                        if (!_collisionBox.Collides(block.BlockModel.BlockCollision, _position,
                            block.CollisionBoxPosition, block.CollidableFaces, out var data)) continue;

                        if (data.BlockFace == BlockFace.Up) {
                            _collisionUpBlocks.Add(block);
                        }

                        else {
                            _collisionBlocks.Add(block);
                        }

                        var now = DateTime.Now.Ticks;
                        if (data.BlockFace == BlockFace.Up) {
                            _lastOnGroundTick = now;
                            _onGroundTicks += dif;
                            dif = 0;
                        }
                        else if (now - _lastOnGroundTick > CMine.TicksPerSecond / 3) {
                            _onGroundTicks = 0;
                        }
                    }
                }
            }
        }

        protected void ManageCollisionBlocks() {
            _collisionBlocks.Sort((o1, o2) => (_position - o1.Position.ToFloat() - BlockCenter).LengthSquared <
                                              (_position - o2.Position.ToFloat() - BlockCenter).LengthSquared
                ? -1
                : 1);

            foreach (var block in _collisionBlocks) {
                if (!_collisionBox.Collides(block.BlockModel.BlockCollision, _position,
                    block.CollisionBoxPosition, block.CollidableFaces, out var data)) continue;
                ReduceVelocity(data.BlockFace);
                var yDistance = block.Position.Y + block.BlockHeight - _position.Y;
                if (data.BlockFace != BlockFace.Up && data.BlockFace != BlockFace.Down && _onGroundInstant &&
                    block.CollidableFaces[(int) BlockFace.Up] && yDistance > 0 && yDistance < 0.55f
                    && CanMoveToTopBlock(_position + new Vector3(0, yDistance, 0))) {
                    _position.Y += yDistance;
                    _position -= BlockFaceMethods.GetRelative(data.BlockFace).ToFloat() * 0.001f;
                }
                else {
                    _position += data.Distance * BlockFaceMethods.GetRelative(data.BlockFace).ToFloat();
                }
            }

            _onGroundInstant = false;

            foreach (var block in _collisionUpBlocks) {
                if (!_collisionBox.Collides(block.BlockModel.BlockCollision, _position,
                        block.CollisionBoxPosition, block.CollidableFaces, out var data) ||
                    data.Distance < 0.0000001f) continue;
                ReduceVelocity(data.BlockFace);
                _onGroundInstant = true;
                _position += data.Distance * BlockFaceMethods.GetRelative(data.BlockFace).ToFloat();
            }
        }

        protected bool CanMoveToTopBlock(Vector3 position) {
            var mix = (int) Math.Floor(_collisionBox.X + position.X - 1);
            var miy = (int) Math.Floor(_collisionBox.Y + position.Y - 1);
            var miz = (int) Math.Floor(_collisionBox.Z + position.Z - 1);
            var max = (int) Math.Ceiling(_collisionBox.X + position.X + _collisionBox.Width + 1);
            var may = (int) Math.Ceiling(_collisionBox.Y + position.Y + _collisionBox.Height + 1);
            var maz = (int) Math.Ceiling(_collisionBox.Z + position.Z + _collisionBox.Depth + 1);

            for (var x = mix; x <= max; x++) {
                for (var y = miy; y <= may; y++) {
                    for (var z = miz; z <= maz; z++) {
                        var block = _world.GetBlock(new Vector3i(x, y, z));
                        if (block == null || block.Passable) continue;
                        if (_collisionBox.Collides(block.BlockModel.BlockCollision, position,
                            block.CollisionBoxPosition, block.CollidableFaces, out _)) return false;
                    }
                }
            }

            return true;
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