using System;
using OpenTK;

namespace CMineNew.Geometry{
    /// <summary>
    /// Represents a 3D integer vector.
    /// </summary>
    [Serializable]
    public struct Vector2i{
        public static readonly Vector2i Zero = new Vector2i(0);
        public static readonly Vector2i One = new Vector2i(1);

        private int _x;
        private int _y;

        /// <summary>
        /// Creates a vector filled with the given value.
        /// </summary>
        /// <param name="v">The given value.</param>
        public Vector2i(int v) {
            _x = v;
            _y = v;
        }

        /// <summary>
        /// Creates a vector using the given values.
        /// </summary>
        /// <param name="x">The "x" parameter.</param>
        /// <param name="y">The "y" parameter.</param>
        public Vector2i(int x, int y) {
            _x = x;
            _y = y;
        }

        /// <summary>
        /// Creates a vector using the given values. Floats are rounded using Math.Round(v).
        /// </summary>
        /// <param name="x">The "x" parameter.</param>
        /// <param name="y">The "y" parameter.</param>
        public Vector2i(float x, float y) {
            _x = (int) Math.Round(x);
            _y = (int) Math.Round(y);
        }

        /// <summary>
        /// Creates a vector using the given vector. Floats are rounded using Math.Round(v).
        /// </summary>
        /// <param name="vector">The float vector.</param>
        public Vector2i(Vector2 vector) {
            _x = (int) Math.Round(vector.X);
            _y = (int) Math.Round(vector.Y);
        }


        /// <summary>
        /// Creates a vector using the given vector.
        /// </summary>
        /// <param name="vector">The float vector.</param>
        /// <param name="floor">Whether values should be rounded or floored</param>
        public Vector2i(Vector2 vector, bool floor) {
            if (floor) {
                _x = (int) Math.Floor(vector.X);
                _y = (int) Math.Floor(vector.Y);
            }
            else {
                _x = (int) Math.Round(vector.X);
                _y = (int) Math.Round(vector.Y);
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

        public Vector2i Add(int x, int y) {
            _x += x;
            _y += y;
            return this;
        }

        public Vector2i Sub(int x, int y) {
            _x -= x;
            _y -= y;
            return this;
        }

        public Vector2i Mul(int x, int y) {
            _x *= x;
            _y *= y;
            return this;
        }

        public Vector2i Div(int x, int y) {
            _x /= x;
            _y /= y;
            return this;
        }

        public static Vector2i operator +(Vector2i left, Vector2i right) {
            left._x += right._x;
            left._y += right._y;
            return left;
        }

        public static Vector2i operator +(Vector2i left, int right) {
            left._x += right;
            left._y += right;
            return left;
        }

        public static Vector2i operator -(Vector2i left, Vector2i right) {
            left._x -= right._x;
            left._y -= right._y;
            return left;
        }

        public static Vector2i operator -(Vector2i left, int right) {
            left._x -= right;
            left._y -= right;
            return left;
        }

        public static Vector2i operator *(Vector2i left, Vector2i right) {
            left._x *= right._x;
            left._y *= right._y;
            return left;
        }

        public static Vector2i operator *(Vector2i left, int right) {
            left._x *= right;
            left._y *= right;
            return left;
        }

        public static Vector2i operator /(Vector2i left, Vector2i right) {
            left._x /= right._x;
            left._y /= right._y;
            return left;
        }

        public static Vector2i operator /(Vector2i left, int right) {
            left._x /= right;
            left._y /= right;
            return left;
        }

        public static Vector2i operator %(Vector2i left, Vector2i right) {
            left._x %= right._x;
            left._y %= right._y;
            return left;
        }

        public static Vector2i operator %(Vector2i left, int right) {
            left._x %= right;
            left._y %= right;
            return left;
        }

        public static Vector2i operator <<(Vector2i left, int right) {
            left._x <<= right;
            left._y <<= right;
            return left;
        }

        public static Vector2i operator >>(Vector2i left, int right) {
            left._x >>= right;
            left._y >>= right;
            return left;
        }


        public int Dot(Vector2i right) {
            return _x * right._x + _y * right._y;
        }

        public int LengthSquared() {
            return _x * _x + _y * _y;
        }

        public double Length() {
            return Math.Sqrt(LengthSquared());
        }

        public Vector2 ToFloat() {
            return new Vector2(_x, _y);
        }

        public override string ToString() {
            return "{" + _x + ", " + _y + "}";
        }

        public static bool operator ==(Vector2i left, Vector2i right) {
            return left.Equals(right);
        }

        public static bool operator !=(Vector2i left, Vector2i right) {
            return !left.Equals(right);
        }

        public bool Equals(Vector2i other) {
            return _x == other._x && _y == other._y;
        }

        public override bool Equals(object obj) {
            return obj is Vector2i other && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                return (_x * 397) ^ _y;
            }
        }
    }
}