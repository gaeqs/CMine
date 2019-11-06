using System;
using OpenTK;

namespace CMineNew.Geometry {
    /// <summary>
    /// Represents a 2D area.
    /// </summary>
    public struct Area2d {
        private float _minX, _minY, _maxX, _maxY;

        /// <summary>
        /// Creates the 2D area.
        /// </summary>
        /// <param name="minX">The minimum position for the x axis.</param>
        /// <param name="minY">The minimum position for the y axis.</param>
        /// <param name="maxX">The maximum position for the x axis.</param>
        /// <param name="maxY">The maximum position for the y axis.</param>
        public Area2d(float minX, float minY, float maxX, float maxY) {
            _minX = minX;
            _minY = minY;
            _maxX = maxX;
            _maxY = maxY;
        }

        /// <summary>
        /// The minimum X value.
        /// </summary>
        public float MinX {
            get => _minX;
            set {
                _minX = value;
                RecalculatePositions(true, false);
            }
        }

        /// <summary>
        /// The minimum Y value.
        /// </summary>
        public float MinY {
            get => _minY;
            set {
                _minY = value;
                RecalculatePositions(false, true);
            }
        }

        /// <summary>
        /// The maximum X value.
        /// </summary>
        public float MaxX {
            get => _maxX;
            set {
                _maxX = value;
                RecalculatePositions(true, false);
            }
        }

        /// <summary>
        /// The maximum Y value.
        /// </summary>
        public float MaxY {
            get => _maxY;
            set {
                _maxY = value;
                RecalculatePositions(false, true);
            }
        }

        /// <summary>
        /// Sets the minimum position.
        /// </summary>
        /// <param name="min">The minimum position.</param>
        /// <returns></returns>
        public Area2d SetMin(Vector2 min) {
            _minX = min.X;
            _minY = min.Y;
            RecalculatePositions(true, true);
            return this;
        }

        /// <summary>
        /// Sets the maximum position.
        /// </summary>
        /// <param name="max">The maximum position.</param>
        /// <returns></returns>
        public Area2d SetMax(Vector2 max) {
            _maxX = max.X;
            _maxY = max.Y;
            RecalculatePositions(true, true);
            return this;
        }

        /// <summary>
        /// Recalculates all values, so min values are less or equal than max values.
        /// </summary>
        /// <param name="x">Whether the X axis should be recalculated.</param>
        /// <param name="y">Whether the Y axis should be recalculates.</param>
        private void RecalculatePositions(bool x, bool y) {
            if (x) {
                var minX = Math.Min(_minX, _maxX);
                _maxX = Math.Max(_minX, _maxX);
                _minX = minX;
            }

            if (!y) return;
            var minY = Math.Min(_minY, _maxY);
            _maxY = Math.Max(_minY, _maxY);
            _minY = minY;
        }

        public Vector4 ToVector() {
            return new Vector4(_minX, _minY, _maxX, _maxY);
        }

        public override string ToString() {
            return "[{" + _minX + ", " + _minY + "},{" + _maxX + ", " + _maxY + "}]";
        }
    }
}