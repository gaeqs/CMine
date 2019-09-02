using System;
using System.Collections.Generic;
using CMineNew.Collision;
using CMineNew.Geometry;
using CMineNew.Map;
using CMineNew.Map.BlockData;
using CMineNew.Map.BlockData.Type;
using OpenTK;

namespace CMineNew.Entities{
    /// <summary>
    /// Represent an entity whose position is modifies by velocities and forces.
    /// These entities are affected by the ground and by water.
    /// </summary>
    public class PhysicEntity : Entity{
        /// <summary>
        /// The default mass of the entity. Currently this cannot be changed.
        /// </summary>
        public static readonly float Mass = 10;

        /// <summary>
        /// The gravity of the entity. Currently this cannot be changed.
        /// </summary>
        public static readonly Vector3 OnGroundGravity = new Vector3(0, -21f * Mass, 0);

        /// <summary>
        /// The gravity of the entity in the water. Currently this cannot be changed.
        /// </summary>
        public static readonly Vector3 WaterGravity = new Vector3(0, -3f * Mass, 0);

        /// <summary>
        /// A constant that represents local position of the center of a block.
        /// </summary>
        private static readonly Vector3 BlockCenter = new Vector3(0.5f);

        /// <summary>
        /// The max distance used by a collision loop. Check the method ManageMovement(float, int).
        /// </summary>
        private static readonly Vector3 MaxDistance = new Vector3(0.4f);

        /// <summary>
        /// The damping constant when the entity is on ground.
        /// </summary>
        public const float DampingConstantGround = -200;

        /// <summary>
        /// The damping constant when the entity is on water.
        /// </summary>
        public const float DampingConstantWater = -550;

        /// <summary>
        /// The damping constant when the entity is on air.
        /// This damping constant doesn't affect the Y axis.
        /// </summary>
        public const float DampingConstantAir = -50;

        protected Vector3 _force, _velocity;
        protected bool _onWater, _wasOnWater, _onGroundInstant;
        protected long _lastOnGroundTick, _onGroundTicks;
        protected long _lastJumpTick;

        protected BlockWater _waterBlock;
        private readonly List<Block> _collisionBlocks, _collisionUpBlocks;

        /// <summary>
        /// Creates a physic entity.
        /// </summary>
        /// <param name="world">The world.</param>
        /// <param name="position">The position.</param>
        /// <param name="collisionBox">The collision box.</param>
        public PhysicEntity(World world, Vector3 position, Aabb collisionBox) : base(world, position, collisionBox) {
            _force = Vector3.Zero;
            _velocity = Vector3.Zero;
            _lastOnGroundTick = 0;
            _onGroundTicks = 0;
            _lastJumpTick = 0;
            _collisionBlocks = new List<Block>();
            _collisionUpBlocks = new List<Block>();
        }

        /// <summary>
        /// Creates a physic entity.
        /// </summary>
        /// <param name="guid">THe GUID.</param>
        /// <param name="world">The world.</param>
        /// <param name="position">The position.</param>
        /// <param name="collisionBox">The collision box.</param>
        public PhysicEntity(Guid guid, World world, Vector3 position, Aabb collisionBox) : base(guid, world, position,
            collisionBox) {
            _force = Vector3.Zero;
            _velocity = Vector3.Zero;
            _lastOnGroundTick = 0;
            _lastJumpTick = 0;
            _collisionBlocks = new List<Block>();
            _collisionUpBlocks = new List<Block>();
        }

        /// <summary>
        /// The force that will applied to the entity when its method Tick() is executed.
        /// </summary>
        public Vector3 Force {
            get => _force;
            set => _force = value;
        }

        /// <summary>
        /// The velocity of the entity.
        /// </summary>
        public Vector3 Velocity {
            get => _velocity;
            set => _velocity = value;
        }

        /// <summary>
        /// The water block the entity is on, or null if the entity is not on water.
        /// </summary>
        public BlockWater WaterBlock => _waterBlock;


        /// <summary>
        /// Whether the entity is on ground. This boolean has a delay.
        /// The entity can be on air, but if it was on ground not long ago this method will return true.
        /// This is used to allow entity to have a better control.
        /// If it's required to check whether the entity is on ground in the exact moment, use OnGroundInstant.
        /// </summary>
        public bool OnGround => DateTime.Now.Ticks - _lastOnGroundTick < CMine.TicksPerSecond / 10;

        /// <summary>
        /// Whether the entity is on ground in this exact moment.
        /// </summary>
        public bool OnGroundInstant => _onGroundInstant;

        /// <summary>
        /// Whether the entity is on water.
        /// </summary>
        public bool OnWater => _onWater;

        /// <summary>
        /// The last tick the entity was on ground.
        /// </summary>
        public long LastOnGroundTick => _lastOnGroundTick;

        /// <summary>
        /// Tries to execute a jump on the entity.
        /// If it's possible, the entity will gain a impulse that allows it to jump a block.
        /// </summary>
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
            //If the entity is in a position where the chunk is not loaded, don't do anything.
            if (_world.GetChunk(new Vector3i(_position, true) >> Chunk.WorldPositionShift) == null)
                return;

            UpdateWaterStatus();

            _force += (OnWater ? WaterGravity : OnGroundGravity) + CalculateDampingForce();

            //Apply implicit euler on the velocity. 
            var h = dif / CMine.TicksPerSecondF;
            _velocity += _force * h / Mass;
            _force = Vector3.Zero;

            var oldPosition = _position;
            ManageMovement(h, dif);
            UpdatePosition(oldPosition);
        }


        /// <summary>
        /// Checks whether the entity is on water.
        /// </summary>
        private void UpdateWaterStatus() {
            _wasOnWater = _onWater;
            if (_world.GetBlock(new Vector3i(_position, true)) is BlockWater water
                && water.WaterHeight >= _position.Y - Math.Floor(_position.Y)) {
                _onWater = true;
                _waterBlock = water;
            }
            else {
                _onWater = false;
            }
        }

        /// <summary>
        /// Calculates the damping force applied to the entity.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Manages the movement of the entity and collisions.
        /// </summary>
        /// <param name="h">The delay between the last tick and the current one in seconds.</param>
        /// <param name="delay">The delay between the last tick and the current one in ticks.</param>
        protected void ManageMovement(float h, long delay) {
            var to = _velocity * h;

            //Applied the position several times if any of its parameters in their absolute form is bigger than MaxDistance.
            //This avoids the effect named "tunneling" in blocks.
            //(This can be optimized)
            while (Math.Abs(to.X) > 0 || Math.Abs(to.Y) > 0 || Math.Abs(to.Z) > 0) {
                var movement = Vector3.ComponentMax(Vector3.ComponentMin(to, MaxDistance), -MaxDistance);
                _position += movement;
                ManageCollision(delay);
                to -= movement;
            }
        }

        /// <summary>
        /// Gets the blocks that can collide with the entity and manages collisions.
        /// </summary>
        /// <param name="dif">The delay between the last tick and the current one in ticks.</param>
        protected void ManageCollision(long dif) {
            GetCollisionBlocks(dif);
            ManageBlockCollision();
        }

        /// <summary>
        /// Gets the blocks that can collide the entity.
        /// </summary>
        /// <param name="dif">The delay between the last tick and the current one in ticks.</param>
        protected void GetCollisionBlocks(long dif) {
            //Min a max positions of blocks that can collide with the entity.
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

                        //Collisions on top faces are managed after other collisions to avoid "ghost jumps".
                        if (data.BlockFace == BlockFace.Up) {
                            _collisionUpBlocks.Add(block);
                        }

                        else {
                            _collisionBlocks.Add(block);
                        }

                        var now = DateTime.Now.Ticks;
                        if (data.BlockFace == BlockFace.Up) {
                            _lastOnGroundTick = now;
                            //This must be changed.
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

        /// <summary>
        /// Manages block collisions.
        /// </summary>
        protected void ManageBlockCollision() {
            //Sorts the blocks. Nearer ones are managed first.
            _collisionBlocks.Sort(BlockComparer);

            var tryTeleportUp = false;
            var teleportHeight = 0f;
            CollisionData teleportCollision = null;

            foreach (var block in _collisionBlocks) {
                if (!_collisionBox.Collides(block.BlockModel.BlockCollision, _position,
                    block.CollisionBoxPosition, block.CollidableFaces, out var data)) continue;
                
                //If collides, reduces velocity.
                ReduceVelocity(data.BlockFace);
                
                //If the entity is on ground and the distance between its position and the top of the block
                //is less than 0.55, try to teleport to the top after all collisions are managed.
                //Else perform position fix.
                var yDistance = block.Position.Y + block.BlockHeight - _position.Y;
                if (data.BlockFace != BlockFace.Up && data.BlockFace != BlockFace.Down && _onGroundInstant &&
                    block.CollidableFaces[(int) BlockFace.Up] && yDistance > 0 && yDistance < 0.55f) {
                    tryTeleportUp = true;
                    if (teleportCollision != null && !(yDistance > teleportHeight)) continue;
                    teleportHeight = yDistance;
                    teleportCollision = data;
                }
                else {
                    _position += data.Distance * BlockFaceMethods.GetRelative(data.BlockFace).ToFloat();
                }
            }

            _onGroundInstant = false;

            //Collisions of top faces are managed after other collisions.
            foreach (var block in _collisionUpBlocks) {
                if (!_collisionBox.Collides(block.BlockModel.BlockCollision, _position,
                        block.CollisionBoxPosition, block.CollidableFaces, out var data) ||
                    data.Distance < 0.0000001f) continue;
                ReduceVelocity(data.BlockFace);
                _onGroundInstant = true;
                _position += data.Distance * BlockFaceMethods.GetRelative(data.BlockFace).ToFloat();
            }
            
            if (!tryTeleportUp) return;
            
            //Try to teleport the entity to the top of the block.
            var to = _position + new Vector3(0, teleportHeight, 0)
                     - BlockFaceMethods.GetRelative(teleportCollision.BlockFace).ToFloat() * 0.01f;
            if (CanMoveToTopBlock(to)) {
                _position.Y += teleportHeight;
                _position = to;
            }
            else {
                _position += teleportCollision.Distance *
                             BlockFaceMethods.GetRelative(teleportCollision.BlockFace).ToFloat();
            }
        }

        /// <summary>
        /// Returns whether the entity collides any block if it is moved to the given position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>Whether the entity collides any block if it is moved to the given position.</returns>
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

        /// <summary>
        /// Reduces the velocity when the player hits a block face.
        /// </summary>
        /// <param name="face">The hit block face.</param>
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

        /// <summary>
        /// Compares two blocks by its distance to the player.
        /// </summary>
        /// <param name="o1">The first block.</param>
        /// <param name="o2">The second block.</param>
        /// <returns>-1 if the distance between the player and the first block is less than the distance
        /// between the player and the second block. Else, it returns 1.</returns>
        private int BlockComparer(Block o1, Block o2) {
            var l1 = (_position - o1.Position.ToFloat() - BlockCenter).LengthSquared;
            var l2 = (_position - o2.Position.ToFloat() - BlockCenter).LengthSquared;
            return l1 < l2 ? -1 : 1;
        }
    }
}