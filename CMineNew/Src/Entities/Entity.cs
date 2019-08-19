using System;
using CMine.Collision;
using CMineNew.Map;
using OpenTK;

namespace CMineNew.Entities{
    public class Entity{
        protected readonly World _world;
        protected readonly Guid _guid;
        protected Vector3 _position;
        protected Aabb _collisionBox;

        public Entity(World world, Vector3 position, Aabb collisionBox)
            : this(Guid.NewGuid(), world, position, collisionBox) {
        }

        public Entity(Guid guid, World world, Vector3 position, Aabb collisionBox) {
            _world = world;
            _guid = guid;
            _position = position;
            _collisionBox = collisionBox;
        }

        public World World => _world;

        public Guid Guid => _guid;

        public Vector3 Position {
            get => _position;
            set {
                var old = _position;
                _position = value;
                UpdatePosition(old);
            }
        }

        public Aabb CollisionBox => _collisionBox;

        public virtual void UpdatePosition(Vector3 old) {
        }

        public virtual void Tick(long dif) {
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