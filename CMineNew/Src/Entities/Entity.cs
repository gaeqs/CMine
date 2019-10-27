using System;
using CMineNew.Collision;
using CMineNew.Map;
using OpenTK;

namespace CMineNew.Entities{
    
    /// <summary>
    /// Represents a movable object in the world.
    /// Entities always have a GUID, a world position and a collision box.
    /// </summary>
    public class Entity{
        protected readonly World _world;
        protected readonly Guid _guid;
        protected Vector3 _position, _renderPosition;
        protected readonly Aabb _collisionBox;
        
        /// <summary>
        /// Creates an entity using a world, a position and a collision box.
        /// The GUID is generated using the method Guid.NewGuid().
        /// </summary>
        /// <param name="world">The world.</param>
        /// <param name="position">The position.</param>
        /// <param name="collisionBox">The collision box.</param>
        public Entity(World world, Vector3 position, Aabb collisionBox)
            : this(Guid.NewGuid(), world, position, collisionBox) {
        }
        
        /// <summary>
        /// Creates an entity using a GUID, a world, a position and a collision box.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="world">The world.</param>
        /// <param name="position">The position.</param>
        /// <param name="collisionBox">The collision box.</param>
        public Entity(Guid guid, World world, Vector3 position, Aabb collisionBox) {
            _world = world;
            _guid = guid;
            _position = position;
            _renderPosition = position;
            _collisionBox = collisionBox;
        }

        /// <summary>
        /// The entity's world.
        /// </summary>
        public World World => _world;

        /// <summary>
        /// The entity's GUID.
        /// </summary>
        public Guid Guid => _guid;

        /// <summary>
        /// The entity's position.
        /// When set the method UpdatePosition(Vector3i) is called.
        /// </summary>
        public Vector3 Position {
            get => _position;
            set {
                var old = _position;
                _position = value;
                UpdatePosition(old);
            }
        }

        public Vector3 RenderPosition => _renderPosition;

        /// <summary>
        /// The entity's collision box.
        /// </summary>
        public Aabb CollisionBox => _collisionBox;

        /// <summary>
        /// A virtual method that is called when the position of the entity is called.
        /// This method is meant to be overriden by special entities such as players.
        /// </summary>
        /// <param name="old"></param>
        public virtual void UpdatePosition(Vector3 old) {
        }

        /// <summary>
        /// A virtual method that is called every game loop.
        /// This method is meant to be overriden by other entities.
        /// </summary>
        /// <param name="dif">The time difference difference between the last tick and the current one.</param>
        public virtual void Tick(long dif) {
        }

        /// <summary>
        /// A virtual method that is called every render loop.
        /// This method is meant to be overriden by other entities.
        /// </summary>
        /// <param name="dif">The time difference difference between the last tick and the current one.</param>
        public virtual void RenderTick(long dif) {
            const int MaxDelay = CMine.TicksPerSecond / 70;
            if (dif >= MaxDelay) {
                _renderPosition = _position;
            }
            else {
                var u = (_position - _renderPosition) / MaxDelay;
                _renderPosition += u * dif;
            }
        }

        protected bool Equals(Entity other) {
            return _guid.Equals(other._guid);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Entity) obj);
        }

        public override int GetHashCode() {
            return _guid.GetHashCode();
        }
    }
}