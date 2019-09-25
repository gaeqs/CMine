using System;
using System.Collections.Generic;
using OpenTK;

namespace CMineNew.Geometry{
    
    /// <summary>
    /// Represents a 3D integer vector.
    /// </summary>
    [Serializable]
    public struct Vector3i{
        
        public static readonly Vector3i Zero = new Vector3i(0);
        public static readonly Vector3i One = new Vector3i(1);
        
        private int _x;
        private int _y;
        private int _z;

        
        /// <summary>
        /// Creates a vector filled with the given value.
        /// </summary>
        /// <param name="v">The given value.</param>
        public Vector3i(int v) {
            _x = v;
            _y = v;
            _z = v;
        }
        
        /// <summary>
        /// Creates a vector filled with the given array.
        /// </summary>
        /// <param name="array">The given array.</param>
        public Vector3i(IReadOnlyList<int> array) {
            _x = array[0];
            _y = array[1];
            _z = array[2];
        }

        /// <summary>
        /// Creates a vector using the given values.
        /// </summary>
        /// <param name="x">The "x" parameter.</param>
        /// <param name="y">The "y" parameter.</param>
        /// <param name="z">The "z" parameter.</param>
        public Vector3i(int x, int y, int z) {
            _x = x;
            _y = y;
            _z = z;
        }

        /// <summary>
        /// Creates a vector using the given values. Floats are rounded using Math.Round(v).
        /// </summary>
        /// <param name="x">The "x" parameter.</param>
        /// <param name="y">The "y" parameter.</param>
        /// <param name="z">The "z" parameter.</param>
        public Vector3i(float x, float y, float z) {
            _x = (int) Math.Round(x);
            _y = (int) Math.Round(y);
            _z = (int) Math.Round(z);
        }
        
        /// <summary>
        /// Creates a vector filled with the given array.
        /// </summary>
        /// <param name="array">The given array.</param>
        public Vector3i(IReadOnlyList<float> array) {
            _x = (int) Math.Round(array[0]);
            _y = (int) Math.Round(array[1]);
            _z = (int) Math.Round(array[2]);
        }
        
        /// <summary>
        /// Creates a vector filled with the given array.
        /// </summary>
        /// <param name="array">The given array.</param>
        /// <param name="floor">Whether values should be rounded or floored.</param>
        public Vector3i(IReadOnlyList<float> array, bool floor) {
            if (floor) {
                _x = (int) Math.Floor(array[0]);
                _y = (int) Math.Floor(array[1]);
                _z = (int) Math.Floor(array[2]);
            }
            else {
                _x = (int) Math.Round(array[0]);
                _y = (int) Math.Round(array[1]);
                _z = (int) Math.Round(array[2]);
            }
        }

        /// <summary>
        /// Creates a vector using the given vector. Floats are rounded using Math.Round(v).
        /// </summary>
        /// <param name="vector">The float vector.</param>
        public Vector3i(Vector3 vector) {
            _x = (int) Math.Round(vector.X);
            _y = (int) Math.Round(vector.Y);
            _z = (int) Math.Round(vector.Z);
        }


        /// <summary>
        /// Creates a vector using the given vector.
        /// </summary>
        /// <param name="vector">The float vector.</param>
        /// <param name="floor">Whether values should be rounded or floored.</param>
        public Vector3i(Vector3 vector, bool floor) {
            if (floor) {
                _x = (int) Math.Floor(vector.X);
                _y = (int) Math.Floor(vector.Y);
                _z = (int) Math.Floor(vector.Z);
            }
            else {
                _x = (int) Math.Round(vector.X);
                _y = (int) Math.Round(vector.Y);
                _z = (int) Math.Round(vector.Z);
            }
        }

        /// <summary>
        /// The X value.
        /// </summary>
        public int X {
            get => _x;
            set => _x = value;
        }

        /// <summary>
        /// THe Y value.
        /// </summary>
        public int Y {
            get => _y;
            set => _y = value;
        }

        /// <summary>
        /// The Z value.
        /// </summary>
        public int Z {
            get => _z;
            set => _z = value;
        }
        
        public Vector3i Add(int x, int y, int z) {
            _x += x;
            _y += y;
            _z += z;
            return this;
        }

        public Vector3i Sub(int x, int y, int z) {
            _x -= x;
            _y -= y;
            _z -= z;
            return this;
        }

        public Vector3i Mul(int x, int y, int z) {
            _x *= x;
            _y *= y;
            _z *= z;
            return this;
        }

        public Vector3i Div(int x, int y, int z) {
            _x /= x;
            _y /= y;
            _z /= z;
            return this;
        }

        public static Vector3i operator +(Vector3i left, Vector3i right) {
            left._x += right._x;
            left._y += right._y;
            left._z += right._z;
            return left;
        }

        public static Vector3i operator +(Vector3i left, int right) {
            left._x += right;
            left._y += right;
            left._z += right;
            return left;
        }

        public static Vector3i operator -(Vector3i left, Vector3i right) {
            left._x -= right._x;
            left._y -= right._y;
            left._z -= right._z;
            return left;
        }

        public static Vector3i operator -(Vector3i left, int right) {
            left._x -= right;
            left._y -= right;
            left._z -= right;
            return left;
        }

        public static Vector3i operator *(Vector3i left, Vector3i right) {
            left._x *= right._x;
            left._y *= right._y;
            left._z *= right._z;
            return left;
        }

        public static Vector3i operator *(Vector3i left, int right) {
            left._x *= right;
            left._y *= right;
            left._z *= right;
            return left;
        }

        public static Vector3i operator /(Vector3i left, Vector3i right) {
            left._x /= right._x;
            left._y /= right._y;
            left._z /= right._z;
            return left;
        }

        public static Vector3i operator /(Vector3i left, int right) {
            left._x /= right;
            left._y /= right;
            left._z /= right;
            return left;
        }

        public static Vector3i operator %(Vector3i left, Vector3i right) {
            left._x %= right._x;
            left._y %= right._y;
            left._z %= right._z;
            return left;
        }

        public static Vector3i operator %(Vector3i left, int right) {
            left._x %= right;
            left._y %= right;
            left._z %= right;
            return left;
        }

        public static Vector3i operator <<(Vector3i left, int right) {
            left._x <<= right;
            left._y <<= right;
            left._z <<= right;
            return left;
        }

        public static Vector3i operator >>(Vector3i left, int right) {
            left._x >>= right;
            left._y >>= right;
            left._z >>= right;
            return left;
        }


        public int Dot(Vector3i right) {
            return _x * right._x + _y * right._y + _z * right._z;
        }

        public int LengthSquared() {
            return _x * _x + _y * _y + _z * _z;
        }

        public double Length() {
            return Math.Sqrt(LengthSquared());
        }

        public Vector3 ToFloat() {
            return new Vector3(_x, _y, _z);
        }

        public override string ToString() {
            return "{" + _x + ", " + _y + ", " + _z + "}";
        }

        public static bool operator ==(Vector3i left, Vector3i right) {
            return left.Equals(right);
        }

        public static bool operator !=(Vector3i left, Vector3i right) {
            return !left.Equals(right);
        } 

        public bool Equals(Vector3i other) {
            return _x == other._x && _y == other._y && _z == other._z;
        }

        public override bool Equals(object obj) {
            return obj is Vector3i other && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = _x;
                hashCode = (hashCode * 397) ^ _y;
                hashCode = (hashCode * 397) ^ _z;
                return hashCode;
            }
        }
    }
}