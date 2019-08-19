using System;
using CMine.Collision;
using CMineNew.Geometry;
using CMineNew.Map;
using OpenTK;

namespace CMineNew.Entities{
    public class PhysicEntity : Entity{
        private static readonly float Mass = 10;
        private static readonly Vector3 Force = new Vector3(0, -21f * Mass, 0);
        private const float DampingConstantGround = -200;
        private const float DampingConstantAir = -20;

        protected Vector3 _velocity;
        protected long _lastOnGroundTick;
        protected long _lastJumpTick;

        public PhysicEntity(World world, Vector3 position, Aabb collisionBox) : base(world, position, collisionBox) {
            _velocity = Vector3.Zero;
            _lastOnGroundTick = 0;
            _lastJumpTick = 0;
        }

        public PhysicEntity(Guid guid, World world, Vector3 position, Aabb collisionBox) : base(guid, world, position,
            collisionBox) {
            _velocity = Vector3.Zero;
            _lastOnGroundTick = 0;
            _lastJumpTick = 0;
        }

        public Vector3 Velocity {
            get => _velocity;
            set => _velocity = value;
        }

        public bool OnGround => DateTime.Now.Ticks - _lastOnGroundTick < CMine.TicksPerSecond / 10;

        public long LastOnGroundTick => _lastOnGroundTick;

        public void Jump() {
            if (DateTime.Now.Ticks - _lastOnGroundTick < CMine.TicksPerSecond / 10 &&
                DateTime.Now.Ticks - _lastJumpTick > CMine.TicksPerSecond / 3) {
                _velocity += new Vector3(0, 7, 0);
                _lastJumpTick = DateTime.Now.Ticks;
            }
        }

        public override void Tick(long dif) {
            var oldPosition = _position;
            var h = dif / CMine.TicksPerSecondF;
            var force = Force + (OnGround ? DampingConstantGround : DampingConstantAir) * _velocity *
                        new Vector3(1, 0, 1);
            _velocity += force * h / Mass;
            _position += _velocity * h;
            ManageCollision();
            UpdatePosition(oldPosition);
        }

        protected void ManageCollision() {
            var mix = (int) Math.Floor(_collisionBox.X + _position.X - 1);
            var miy = (int) Math.Floor(_collisionBox.Y + _position.Y - 1);
            var miz = (int) Math.Floor(_collisionBox.Z + _position.Z - 1);
            var max = (int) Math.Ceiling(_collisionBox.X + _position.X + _collisionBox.Width + 1);
            var may = (int) Math.Ceiling(_collisionBox.Y + _position.Y + _collisionBox.Height + 1);
            var maz = (int) Math.Ceiling(_collisionBox.Z + _position.Z + _collisionBox.Depth + 1);

            for (var x = mix; x <= max; x++) {
                for (var y = miy; y <= may; y++) {
                    for (var z = miz; z <= maz; z++) {
                        var block = _world.GetBlock(new Vector3i(x, y, z));
                        if (block == null || block.Passable) continue;
                        if (!_collisionBox.Collides(block.BlockModel.BlockCollision, _position,
                            block.Position.ToFloat(), block.CollidableFaces, out var data)) continue;
                        _position += data.Distance * BlockFaceMethods.GetRelative(data.BlockFace).ToFloat();
                        ReduceVelocity(data.BlockFace);

                        if (data.BlockFace == BlockFace.Up) {
                            _lastOnGroundTick = DateTime.Now.Ticks;
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