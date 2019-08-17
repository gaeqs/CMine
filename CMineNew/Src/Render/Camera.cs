using System;
using OpenTK;

namespace CMineNew.Render{
    public class Camera{
        private Vector3 _position, _lookAt, _up;
        private Vector3 _n, _v, _u;
        private Matrix4 _matrix, _invertedMatrix;
        private Frustum _frustum;
        private bool _requiresRecalculation;

        public Camera(Vector3 position, Vector3 lookAt, Vector3 up, float fov) {
            _position = position;
            _lookAt = lookAt;
            _up = up;
            _frustum = new Frustum(this, 0.03f, 500, fov, CMine._window.Width / (float) CMine._window.Height);
            RecalculateN();
        }

        public Vector3 Position {
            get => _position;
            set => _position = value;
        }

        public Vector3 LookAt {
            get => _lookAt;
            set {
                _lookAt = value;
                RecalculateN();
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

        public void SetRotation(Vector2 rotation) {
            var yawCos = (float) Math.Cos(rotation.Y);
            var yawSin = (float) Math.Sin(rotation.Y);
            var pitchCos = (float) Math.Cos(rotation.X);
            var pitchSin = (float) Math.Sin(rotation.X);

            LookAt = new Vector3(yawCos * pitchCos, pitchSin, yawSin * pitchCos);
        }

        public bool IsVisible(Vector3 position, float radius) {
            if (_requiresRecalculation) {
                GenerateMatrix();
            }

            return _frustum.IsVisible(position, radius);
        }

        private void RecalculateN() {
            _n = -_lookAt.Normalized();
            RecalculateV();
        }

        private void RecalculateV() {
            _v = _up - Vector3.Dot(_n, _up) * _n;
            _n.Normalize();
            RecalculateU();
        }

        private void RecalculateU() {
            _u = Vector3.Cross(_v, _n);
            _requiresRecalculation = true;
        }

        private void GenerateMatrix() {
            if (!_requiresRecalculation) return;
            _invertedMatrix = new Matrix4(_u.X, _u.Y, _u.Z, 0,
                _v.X, _v.Y, _v.Z, 0,
                _n.X, _n.Y, _n.Z, 0,
                _position.X, _position.Y, _position.Z, 1);
            var p = -_position;
            _matrix = new Matrix4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, -_position.X, -_position.Y, -_position.Z, 1)
                      * new Matrix4(_u.X, _v.X, _n.X, 0,
                          _u.Y, _v.Y, _n.Y, 0,
                          _u.Z, _v.Z, _n.Z, 0,
                          Vector3.Dot(_u, p), Vector3.Dot(_v, p), Vector3.Dot(_n, p), 1);
            _requiresRecalculation = false;
            _frustum.GeneratePlanes();
        }
    }
}