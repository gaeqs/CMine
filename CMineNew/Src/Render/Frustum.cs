using System;
using System.Linq;
using CMineNew.Geometry;
using CMineNew.Map;
using OpenTK;

namespace CMineNew.Render{
    public class Frustum{
        private const float CosFlexibility = 1.2f;
        private const float Pi360 = (float) Math.PI / 360;

        private Camera _camera;
        private float _near, _far, _left, _right, _top, _bottom;
        private float _fov;
        private float _aspectRatio;
        private Matrix4 _matrix;

        private Plane[] _planes;

        public Frustum(Camera camera, float near, float far, float left, float right, float top, float bottom) {
            _camera = camera;
            _near = near;
            _far = far;
            _left = left;
            _right = right;
            _top = top;
            _bottom = bottom;
            _planes = new Plane[6];
            RecalculateAspectRatioAndFov();
            GenerateMatrix();
        }

        public Frustum(Camera camera, float near, float far, float fov, float aspectRatio) {
            _camera = camera;
            _near = near;
            _far = far;
            _fov = fov;
            _aspectRatio = aspectRatio;
            RecalculateFrustum();
        }

        public float Near {
            get => _near;
            set {
                _near = value;
                RecalculateAspectRatioAndFov();
                GenerateMatrix();
            }
        }

        public float Far {
            get => _far;
            set {
                _far = value;
                RecalculateAspectRatioAndFov();
                GenerateMatrix();
            }
        }

        public float Left {
            get => _left;
            set {
                _left = value;
                RecalculateAspectRatioAndFov();
                GenerateMatrix();
            }
        }

        public float Right {
            get => _right;
            set {
                _right = value;
                RecalculateAspectRatioAndFov();
                GenerateMatrix();
            }
        }

        public float Top {
            get => _top;
            set {
                _top = value;
                RecalculateAspectRatioAndFov();
                GenerateMatrix();
            }
        }

        public float Bottom {
            get => _bottom;
            set {
                _bottom = value;
                RecalculateAspectRatioAndFov();
                GenerateMatrix();
            }
        }

        public float Fov {
            get => _fov;
            set {
                _fov = value;
                RecalculateFrustum();
                GenerateMatrix();
            }
        }

        public float AspectRatio {
            get => _aspectRatio;
            set {
                _aspectRatio = value;
                RecalculateFrustum();
                GenerateMatrix();
            }
        }

        public Matrix4 Matrix => _matrix;

        public Plane[] Planes => _planes;

        private void RecalculateFrustum() {
            var tan = (float) Math.Tan(_fov * Pi360);
            _top = tan * _near;
            _bottom = -_top;

            _right = _top * _aspectRatio;
            _left = -_right;
            _planes = new Plane[6];
        }

        private void  RecalculateAspectRatioAndFov() {
            _aspectRatio = (_right - _left) / (_top - _bottom);
            _fov = (float) Math.Atan(_top / _near) / Pi360;
        }

        public void GenerateMatrix() {
            _matrix = new Matrix4(2 * _near / (_right - _left), 0, 0, 0,
                0, 2 * _near / (_top - _bottom), 0, 0,
                (_right + _left) / (_right - _left), (_top + _bottom) / (_top - _bottom),
                -(_far + _near) / (_far - _near), -1,
                0, 0, -2 * _far * _near / (_far - _near), 0);
            GeneratePlanes();
            _camera.GenerateViewProjectionMatrix();
        }

        public void GeneratePlanes() {
            var mat = _camera.ViewProjection;
            //Left
            _planes[0].A = mat.M14 + mat.M11;
            _planes[0].B = mat.M24 + mat.M21;
            _planes[0].C = mat.M34 + mat.M31;
            _planes[0].D = mat.M44 + mat.M41;
            //Right
            _planes[1].A = mat.M14 - mat.M11;
            _planes[1].B = mat.M24 - mat.M21;
            _planes[1].C = mat.M34 - mat.M31;
            _planes[1].D = mat.M44 - mat.M41;
            //Bottom
            _planes[3].A = mat.M14 + mat.M12;
            _planes[3].B = mat.M24 + mat.M22;
            _planes[3].C = mat.M34 + mat.M32;
            _planes[3].D = mat.M44 + mat.M42;
            //Top
            _planes[2].A = mat.M14 - mat.M12;
            _planes[2].B = mat.M24 - mat.M22;
            _planes[2].C = mat.M34 - mat.M32;
            _planes[2].D = mat.M44 - mat.M42;
            //Near
            _planes[4].A = mat.M14 + mat.M13;
            _planes[4].B = mat.M24 + mat.M23;
            _planes[4].C = mat.M34 + mat.M33;
            _planes[4].D = mat.M44 + mat.M43;
            //Far
            _planes[5].A = mat.M14 - mat.M13;
            _planes[5].B = mat.M24 - mat.M23;
            _planes[5].C = mat.M34 - mat.M33;
            _planes[5].D = mat.M44 - mat.M43;

            for (var i = 0; i < _planes.Length; i++) {
                _planes[i].Normalize();
            }
        }

        public bool IsVisible(ChunkRegion region) {
            return IsVisible(((region.Position << 6) + 32).ToFloat(), ChunkRegion.RegionRadius);
        }

        public bool IsVisible(Vector3 position, float radius) {
            return _planes.All(t => t.Distance(position) >= -radius);
        }
    }
}