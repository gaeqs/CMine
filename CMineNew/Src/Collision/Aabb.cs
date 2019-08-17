using System.Linq;
using CMineNew.Map;
using OpenTK;

namespace CMine.Collision{
    public class Aabb{
        private float _x, _y, _z, _width, _height, _depth;

        public Aabb(float x, float y, float z, float width, float height, float depth) {
            _x = x;
            _y = y;
            _z = z;
            _width = width;
            _height = height;
            _depth = depth;
        }

        public float X => _x;

        public float Y => _y;

        public float Z => _z;

        public float Width => _width;

        public float Height => _height;

        public float Depth => _depth;

        public bool Collides(Vector3 point, Vector3 thisPosition) {
            var tx = _x + thisPosition.X;
            var ty = _y + thisPosition.Y;
            var tz = _z + thisPosition.Z;

            return tx <= point.X && tx + _width >= point.X &&
                   ty <= point.Y && ty + _height >= point.Y &&
                   tz <= point.Z && tz + _depth >= point.Z;
        }

        public bool Collides(Aabb o, Vector3 thisPosition, Vector3 otherPosition, bool[] faces,
            out CollisionData data) {
            if (faces != null && !Enumerable.Any(faces, f => f)) {
                data = null;
                return false;
            }

            var tx = _x + thisPosition.X;
            var ty = _y + thisPosition.Y;
            var tz = _z + thisPosition.Z;
            var ox = o._x + otherPosition.X;
            var oy = o._y + otherPosition.Y;
            var oz = o._z + otherPosition.Z;

            var dxa = tx + _width - ox;
            var dxb = ox + o._width - tx;

            var dya = ty + _height - oy;
            var dyb = oy + o._height - ty;

            var dza = tz + _depth - oz;
            var dzb = oz + o._depth - tz;

            if (dxa <= 0 || dxb <= 0 || dya <= 0 || dyb <= 0 || dza <= 0 || dzb <= 0) {
                data = null;
                return false;
            }

            var distance = dxa;
            var face = BlockFace.West;

            if (dxb < distance) {
                distance = dxb;
                face = BlockFace.East;
            }

            if (dya < distance) {
                distance = dya;
                face = BlockFace.Down;
            }

            if (dyb < distance) {
                distance = dyb;
                face = BlockFace.Up;
            }

            if (dza < distance) {
                distance = dza;
                face = BlockFace.North;
            }

            if (dzb < distance) {
                distance = dzb;
                face = BlockFace.South;
            }

            data = new CollisionData(face, distance);
            return faces == null || faces[(int) face];
        }
    }
}