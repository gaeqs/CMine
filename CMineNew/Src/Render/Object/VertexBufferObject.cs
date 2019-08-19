using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Render.Object{
    public class VertexBufferObject{
        public static void Unbind(BufferTarget target) {
            GL.BindBuffer(target, 0);
        }

        private int _id;
        private unsafe float* _pointer;
        private bool _mapping;


        public VertexBufferObject() {
            GL.GenBuffers(1, out _id);
            if (_id == 0) {
                throw new System.Exception("Couldn't create VBO. ID is 0.");
            }

            _mapping = false;
        }

        public int Id => _id;

        public unsafe float* Pointer => _pointer;

        public bool Mapping => _mapping;

        public void Bind(BufferTarget target) {
            GL.BindBuffer(target, _id);
        }

        public void SetData(BufferTarget target, float[] data, BufferUsageHint usageHint) {
            GL.BufferData(target, data.Length * sizeof(float), data, usageHint);
        }

        public void SetData(BufferTarget target, int[] data, BufferUsageHint usageHint) {
            GL.BufferData(target, data.Length * sizeof(int), data, usageHint);
        }

        public void SetData(BufferTarget target, int length, BufferUsageHint usageHint) {
            GL.BufferData(target, length, IntPtr.Zero, usageHint);
        }

        public void CleanUp() {
            GL.DeleteBuffer(_id);
        }

        #region mapping

        public void StartMapping() {
            if (_mapping) return;
            Bind(BufferTarget.ArrayBuffer);
            unsafe {
                _pointer = (float*) GL.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.ReadWrite).ToPointer();
            }

            _mapping = true;
        }

        public void FinishMapping() {
            if (!_mapping) return;
            Bind(BufferTarget.ArrayBuffer);
            GL.UnmapBuffer(BufferTarget.ArrayBuffer);
            _mapping = false;
        }

        public bool AddToMap(IEnumerable<float> data, int offset) {
            if (!_mapping) return false;
            unsafe {
                var p = _pointer + offset;
                foreach (var f in data) {
                    *p++ = f;
                }
            }

            return true;
        }

        public bool MoveMapData(int offset, int toOffset, int length) {
            if (!_mapping) return false;
            unsafe {
                var pF = _pointer + offset;
                var pI = _pointer + toOffset;
                for (var i = 0; i < length; i++) {
                    *pI++ = *pF++;
                }
            }

            return true;
        }

        #endregion
    }
}