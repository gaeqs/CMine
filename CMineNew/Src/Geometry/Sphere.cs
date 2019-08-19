using System;
using OpenTK;

namespace CMineNew.Geometry{
    /// <summary>
    /// Represents a sphere.
    /// </summary>
    public struct Sphere{
        private float _x, _y, _z;
        private float _radius;

        /// <summary>
        /// Creates a sphere with the given point and radius.
        /// </summary>
        /// <param name="x">The X value of the point.</param>
        /// <param name="y">The Y value of the point.</param>
        /// <param name="z">The Z value of the point.</param>
        /// <param name="radius"></param>
        public Sphere(float x, float y, float z, float radius) {
            _x = x;
            _y = y;
            _z = z;
            _radius = radius;
        }

        /// <summary>
        /// The X position.
        /// </summary>
        public float X {
            get => _x;
            set => _x = value;
        }

        /// <summary>
        /// The Y position.
        /// </summary>
        public float Y {
            get => _y;
            set => _y = value;
        }

        /// <summary>
        /// The Z position.
        /// </summary>
        public float Z {
            get => _z;
            set => _z = value;
        }

        /// <summary>
        /// The radius.
        /// </summary>
        public float Radius {
            get => _radius;
            set => _radius = value;
        }

        /// <summary>
        /// Returns the distance between the given point and the sphere.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public float Distance(Vector3 point) {
            return (float) Math.Sqrt(DistanceSquared(point));
        }


        /// <summary>
        /// Returns the distance between the given point and the sphere.
        /// </summary>
        /// <param name="x">The X value of the point.</param>
        /// <param name="y">The Y value of the point.</param>
        /// <param name="z">The Z value of the point.</param>
        /// <returns></returns>
        public float Distance(float x, float y, float z) {
            return (float) Math.Sqrt(DistanceSquared(x, y, z));
        }

        /// <summary>
        /// Returns the distance between the given point and the sphere squared.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public float DistanceSquared(Vector3 point) {
            var x = point.X - _x;
            var y = point.Y - _y;
            var z = point.Z - _z;
            return x * x + y * y + z * z;
        }

        /// <summary>
        /// Returns the distance between the given point and the sphere squared.
        /// </summary>
        /// <param name="x">The X value of the point.</param>
        /// <param name="y">The Y value of the point.</param>
        /// <param name="z">The Z value of the point.</param>
        /// <returns></returns>
        public float DistanceSquared(float x, float y, float z) {
            var dx = x - _x;
            var dy = y - _y;
            var dz = z - _z;
            return dx * dx + dy * dy + dz * dz;
        }

        /// <summary>
        /// Returns whether the given point is inside the sphere.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        public bool IsPointInside(Vector3 point) {
            return DistanceSquared(point) < _radius * _radius;
        }

        /// <summary>
        /// Returns whether the given point is inside the sphere.
        /// </summary>
        /// <param name="x">The X value of the point.</param>
        /// <param name="y">The Y value of the point.</param>
        /// <param name="z">The Z value of the point.</param>
        /// <returns></returns>
        public bool IsPointInside(float x, float y, float z) {
            return DistanceSquared(x, y, z) < _radius * _radius;
        }

        
        /// <summary>
        /// Returns whether the given sphere intersects this one.
        /// </summary>
        /// <param name="sphere">The given sphere.</param>
        /// <returns></returns>
        public bool Intersects(Sphere sphere) {
            var add = sphere._radius + _radius;
            return DistanceSquared(sphere._x, sphere._y, sphere._z) < add * add;
        }
    }
}