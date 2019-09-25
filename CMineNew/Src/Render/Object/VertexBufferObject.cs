using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Render.Object{
    public class VertexBufferObject{
        private static int BuffersCount;
        private static long BufferMemory;

        public static int Buffers => BuffersCount;

        public static long BuffersMemory => BufferMemory;

        public static void Unbind(BufferTarget target) {
            GL.BindBuffer(target, 0);
        }

        private int _id;
        private unsafe void* _pointer;
        private volatile bool _mapping;
        private volatile int _size;
        private volatile object _lock = new object();


        public VertexBufferObject() {
            GL.GenBuffers(1, out _id);
            if (_id == 0) {
                throw new System.Exception("Couldn't create VBO. ID is 0.");
            }

            BuffersCount++;

            _mapping = false;
            _size = 0;
        }

        public int Id => _id;

        public unsafe void* Pointer => _pointer;

        public bool Mapping => _mapping;

        public void Bind(BufferTarget target) {
            GL.GetError();
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

        public void BindBase(BufferTarget target, int index) {
            GL.GetError();
            GL.BindBufferBase(target, index, _id);
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
            BufferMemory -= _size;
            _size = data.Length * sizeof(float);
            BufferMemory += _size;
            GL.BufferData(target, _size, data, usageHint);
        }

        public void SetData(BufferTarget target, int[] data, BufferUsageHint usageHint) {
            BufferMemory -= _size;
            _size = data.Length * sizeof(int);
            BufferMemory += _size;
            GL.BufferData(target, _size, data, usageHint);
        }

        public void SetData(BufferTarget target, int length, BufferUsageHint usageHint) {
            BufferMemory -= _size;
            _size = length;
            BufferMemory += _size;
            GL.BufferData(target, length, IntPtr.Zero, usageHint);
        }

        public void CleanUp() {
            FinishMapping();
            GL.DeleteBuffer(_id);
            BuffersCount--;
            BufferMemory -= _size;
        }

        #region mapping

        public void StartMapping(BufferTarget target = BufferTarget.ArrayBuffer) {
            lock (_lock) {
                if (_mapping) return;
                Bind(target);
                unsafe {
                    GL.GetError();
                    _pointer = GL.MapBuffer(target, BufferAccess.ReadWrite).ToPointer();
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

        public void FinishMapping(BufferTarget target = BufferTarget.ArrayBuffer) {
            lock (_lock) {
                unsafe {
                    if (!_mapping) return;
                    GL.GetError();
                    Bind(target);
                    GL.UnmapBuffer(target);
                    _pointer = null;
                    _mapping = false;

                    var error = GL.GetError();
                    if (error != ErrorCode.NoError) {
                        Console.WriteLine("---  VBO UNMAPPING ERROR --- ");
                        Console.WriteLine("VBO: " + _id);
                        Console.WriteLine(error);
                        Console.WriteLine("---------------------------");
                        throw new System.Exception("Error while mapping VBO.");
                    }
                }
            }
        }

        public bool AddToMap(IEnumerable<float> data, int offset) {
            lock (_lock) {
                if (!_mapping) return false;
                unsafe {
                    var p = (float*) _pointer + offset;
                    foreach (var f in data) {
                        *p++ = f;
                    }
                }

                return true;
            }
        }
        
        public bool AddToMap(IEnumerable<int> data, int offset) {
            lock (_lock) {
                if (!_mapping) return false;
                unsafe {
                    var p = (int*) _pointer + offset;
                    foreach (var f in data) {
                        *p++ = f;
                    }
                }

                return true;
            }
        }
        
        public bool AddToMap(IEnumerable<uint> data, int offset) {
            lock (_lock) {
                if (!_mapping) return false;
                unsafe {
                    var p = (uint*) _pointer + offset;
                    foreach (var f in data) {
                        *p++ = f;
                    }
                }

                return true;
            }
        }
        
        public bool AddToMap(IEnumerable<byte> data, int offset) {
            lock (_lock) {
                if (!_mapping) return false;
                unsafe {
                    var p = (byte*) _pointer + offset;
                    foreach (var f in data) {
                        *p++ = f;
                    }
                }

                return true;
            }
        }
        
        public bool AddToMap(IEnumerable<bool> data, int offset) {
            lock (_lock) {
                if (!_mapping) return false;
                unsafe {
                    var p = (bool*) _pointer + offset;
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
                    var pF = (byte*) _pointer + offset;
                    var pI = (byte*) _pointer + toOffset;
                    for (var i = 0; i < length; i++) {
                        *pI++ = *pF++;
                    }
                }

                return true;
            }
        }
        
        public bool MoveMapDataFloat(int offset, int toOffset, int length) {
            lock (_lock) {
                if (!_mapping) return false;
                unsafe {
                    var pF = (float*) _pointer + offset;
                    var pI = (float*) _pointer + toOffset;
                    for (var i = 0; i < length; i++) {
                        *pI++ = *pF++;
                    }
                }

                return true;
            }
        }

        public float[] GetDataFromMap(int offset, int length) {
            var array = new float[length];
            lock (_lock) {
                if (!_mapping) throw new System.Exception("VBO is not mapped.");
                unsafe {
                    var pF = (float*) _pointer + offset;
                    for (var i = 0; i < length; i++) {
                        array[i] = *pF++;
                    }
                }

                return array;
            }
        }

        #endregion
    }
}