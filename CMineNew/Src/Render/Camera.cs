using System;
using CMineNew.Map;
using OpenTK;

namespace CMineNew.Render{
    public class Camera{
        public const float ExtremePitch = (float) Math.PI / 2 - 0.001f;

        protected Vector3 _position, _lookAt, _up;
        protected Vector2 _rotation;
        protected Vector3 _n, _v, _u;
        protected Matrix4 _matrix, _invertedMatrix, _viewProjection;
        protected Frustum _frustum;
        protected bool _requiresRecalculation;

        public Camera(Vector3 position, Vector2 rotation, Vector3 up, float fov) {
            _position = position;
            _rotation = rotation;
            _up = up;
            _frustum = new Frustum(this, 0.03f, 500, fov, CMine.Window.Width / (float) CMine.Window.Height);
            _frustum.GenerateMatrix();
            RecalculateLookAt();
        }

        public virtual Vector3 Position {
            get => _position;
            set {
                _position = value;
                GenerateMatrix();
            }
        }

        public virtual Vector3 LookAt => _lookAt;

        public virtual Vector2 Rotation {
            get => _rotation;
            set {
                _rotation = new Vector2(Math.Max(-ExtremePitch, Math.Min(ExtremePitch, value.X)), value.Y);
                RecalculateLookAt();
            }
        }

        public Vector3 Up {
            get => _up;
            set {
                _up = value;
                RecalculateV();
            }
        }

        public Frustum Frustum {
            get => _frustum;
            set => _frustum = value;
        }

        public Vector3 N => _n;

        public Vector3 V => _v;

        public Vector3 U => _u;

        public Matrix4 Matrix {
            get {
                if (_requiresRecalculation) {
                    GenerateMatrix();
                }

                return _matrix;
            }
        }

        public Matrix4 InvertedMatrix {
            get {
                if (_requiresRecalculation) {
                    GenerateMatrix();
                }

                return _invertedMatrix;
            }
        }

        public Matrix4 ViewProjection {
            get {
                if (_requiresRecalculation) {
                    GenerateMatrix();
                }

                return _viewProjection;
            }
        }

        public bool IsVisible(ChunkRegion region) {
            if (_requiresRecalculation) {
                GenerateMatrix();
            }

            return _frustum.IsVisible(region);
        }

        public bool IsVisible(Vector3 position, float radius) {
            if (_requiresRecalculation) {
                GenerateMatrix();
            }

            return _frustum.IsVisible(position, radius);
        }

        public void GenerateViewProjectionMatrix() {
            if (_requiresRecalculation) {
                GenerateMatrix();
            }
            else {
                _viewProjection = _matrix * _frustum.Matrix;
            }
        }

        protected void RecalculateLookAt() {
            var yawCos = (float) Math.Cos(_rotation.Y);
            var yawSin = (float) Math.Sin(_rotation.Y);
            var pitchCos = (float) Math.Cos(_rotation.X);
            var pitchSin = (float) Math.Sin(_rotation.X);

            _lookAt = new Vector3(yawCos * pitchCos, pitchSin, yawSin * pitchCos);
            RecalculateN();
        }

        protected void RecalculateN() {
            _n = -_lookAt.Normalized();
            RecalculateV();
        }

        private void RecalculateV() {
            _v = _up - Vector3.Dot(_n, _up) * _n;
            _v.Normalize();
            RecalculateU();
        }

        private void RecalculateU() {
            _u = Vector3.Cross(_v, _n);
            _requiresRecalculation = true;
        }

        protected void GenerateMatrix() {
            if (!_requiresRecalculation) return;
            _invertedMatrix = new Matrix4(_u.X, _u.Y, _u.Z, 0,
                _v.X, _v.Y, _v.Z, 0,
                _n.X, _n.Y, _n.Z, 0,
                _position.X, _position.Y, _position.Z, 1);


            var p = -_position;
            _matrix = new Matrix4(_u.X, _v.X, _n.X, 0,
                _u.Y, _v.Y, _n.Y, 0,
                _u.Z, _v.Z, _n.Z, 0,
                Vector3.Dot(_u, p), Vector3.Dot(_v, p), Vector3.Dot(_n, p), 1);
            _requiresRecalculation = false;
            _frustum.GeneratePlanes();
            GenerateViewProjectionMatrix();
        }
    }
}