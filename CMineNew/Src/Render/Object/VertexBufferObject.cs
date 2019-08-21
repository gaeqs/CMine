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
        private volatile bool _mapping;
        private volatile object _lock = new object();


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
            var error = GL.GetError();
            if (error != ErrorCode.NoError) {
                Console.WriteLine("---  VBO BIND ERROR --- ");
                Console.WriteLine("VBO: " + _id);
                Console.WriteLine(error);
                Console.WriteLine("------------------------");
                throw new System.Exception("Error while binding VBO.");
            }
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
            lock (_lock) {
                if (_mapping) return;
                Bind(BufferTarget.ArrayBuffer);
                unsafe {
                    var old = _pointer;
                    _pointer = (float*) GL.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.ReadWrite).ToPointer();
                    var error = GL.GetError();
                    if (error != ErrorCode.NoError) {
                        Console.WriteLine("---  VBO MAPPING ERROR --- ");
                        Console.WriteLine("VBO: " + _id);
                        Console.WriteLine(error);
                        Console.WriteLine("---------------------------");
                        throw new System.Exception("Error while mapping VBO.");
                    }
                }

                _mapping = true;
            }
        }

        public void FinishMapping() {
            lock (_lock) {
                unsafe {
                    if (!_mapping) return;
                    Bind(BufferTarget.ArrayBuffer);
                    GL.UnmapBuffer(BufferTarget.ArrayBuffer);
                    _pointer = null;
                    _mapping = false;
                }
            }
        }

        public bool AddToMap(IEnumerable<float> data, int offset) {
            lock (_lock) {
                if (!_mapping) return false;
                unsafe {
                    var p = _pointer + offset;
                    foreach (var f in data) {
                        *p++ = f;
                    }
                }

                return true;
            }
        }

        public bool MoveMapData(int offset, int toOffset, int length) {
            lock (_lock) {
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
        }

        #endregion
    }
}