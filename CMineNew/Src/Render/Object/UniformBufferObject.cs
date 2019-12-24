using System;
using CMineNew.Geometry;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Render.Object{
    public class UniformBufferObject{
        private readonly UboObject[] _objects;
        private readonly VertexBufferObject _vbo;

        public UniformBufferObject(UboObject[] objects) {
            _objects = objects;
            _vbo = new VertexBufferObject();
            Bind();
            _vbo.SetData(BufferTarget.UniformBuffer, CalculateSize(), BufferUsageHint.StreamDraw);
            Unbind();
        }

        public int Id => _vbo.Id;
        
        public void Bind() {
            _vbo.Bind(BufferTarget.UniformBuffer);
        }

        public void Unbind() {
            VertexBufferObject.Unbind(BufferTarget.UniformBuffer);
        }

        public void BindToBase(int index) {
            _vbo.BindBase(BufferRangeTarget.UniformBuffer, index);
        }

        public void StartMapping(bool invalidate) {
            _vbo.StartMapping(BufferTarget.UniformBuffer, invalidate);
        }

        public void Orphan() {
            _vbo.Orphan(BufferTarget.UniformBuffer, BufferUsageHint.DynamicDraw);
        }

        public void FinishMapping() {
            _vbo.FinishMapping(BufferTarget.UniformBuffer);
        }

        public void Set(float f, int index) {
            var o = _objects[index];
            if (o.Id != UboObject.Float.Id)
                throw new ArgumentException("The argument can't be a Float! Id: " + o.Id);
            var offset = GetStartOffset(index, o);
            if (_vbo.Mapping) {
                unsafe {
                    var pointer = (float*) _vbo.Pointer;
                    pointer += offset >> 2;
                    *pointer = f;
                    return;
                }
            }

            _vbo.SetSubData(BufferTarget.UniformBuffer, f, offset);
        }

        public void Set(int i, int index) {
            var o = _objects[index];
            if (o.Id != UboObject.Integer.Id)
                throw new ArgumentException("The argument can't be an Integer! Id: " + o.Id);
            var offset = GetStartOffset(index, o);
            if (_vbo.Mapping) {
                unsafe {
                    var pointer = (int*) _vbo.Pointer;
                    pointer += offset >> 2;
                    *pointer = i;
                    return;
                }
            }

            _vbo.SetSubData(BufferTarget.UniformBuffer, i, offset);
        }

        public void Set(bool b, int index) {
            var o = _objects[index];
            if (o.Id != UboObject.Boolean.Id)
                throw new ArgumentException("The argument can't be a Boolean! Id: " + o.Id);
            var offset = GetStartOffset(index, o);
            if (_vbo.Mapping) {
                unsafe {
                    var pointer = (int*) _vbo.Pointer;
                    pointer += offset >> 2;
                    *pointer = b ? 1 : 0;
                    return;
                }
            }

            _vbo.SetSubData(BufferTarget.UniformBuffer, b ? 1 : 0, offset);
        }

        public void Set(Vector2 vector2, int index) {
            var o = _objects[index];
            if (o.Id != UboObject.Vector2.Id)
                throw new ArgumentException("The argument can't be a Vector2! Id: " + o.Id);
            var offset = GetStartOffset(index, o);
            if (_vbo.Mapping) {
                unsafe {
                    var pointer = (float*) _vbo.Pointer;
                    pointer += offset >> 2;
                    *pointer++ = vector2.X;
                    *pointer = vector2.Y;
                    return;
                }
            }

            _vbo.SetSubData(BufferTarget.UniformBuffer, vector2, offset);
        }

        public void Set(Vector3 vector3, int index) {
            var o = _objects[index];
            if (o.Id != UboObject.Vector3.Id)
                throw new ArgumentException("The argument can't be a Vector3! Id: " + o.Id);
            var offset = GetStartOffset(index, o);
            if (_vbo.Mapping) {
                unsafe {
                    var pointer = (float*) _vbo.Pointer;
                    pointer += offset >> 2;
                    *pointer++ = vector3.X;
                    *pointer++ = vector3.Y;
                    *pointer = vector3.Z;
                    return;
                }
            }

            _vbo.SetSubData(BufferTarget.UniformBuffer, vector3, offset);
        }

        public void Set(Vector3i vector3i, int index) {
            var o = _objects[index];
            if (o.Id != UboObject.Vector3.Id)
                throw new ArgumentException("The argument can't be a Vector3! Id: " + o.Id);
            var offset = GetStartOffset(index, o);
            
            if (_vbo.Mapping) {
                unsafe {
                    var pointer = (int*) _vbo.Pointer;
                    pointer += offset >> 2;
                    *pointer++ = vector3i.X;
                    *pointer++ = vector3i.Y;
                    *pointer = vector3i.Z;
                    return;
                }
            }

            _vbo.SetSubData(BufferTarget.UniformBuffer, vector3i, offset);
        }
        
        public void Set(Vector4 vector4, int index) {
            var o = _objects[index];
            if (o.Id != UboObject.Vector4.Id)
                throw new ArgumentException("The argument can't be a Vector3! Id: " + o.Id);
            var offset = GetStartOffset(index, o);
            if (_vbo.Mapping) {
                unsafe {
                    var pointer = (float*) _vbo.Pointer;
                    pointer += offset >> 2;
                    *pointer++ = vector4.X;
                    *pointer++ = vector4.Y;
                    *pointer++ = vector4.Z;
                    *pointer = vector4.W;
                    return;
                }
            }

            _vbo.SetSubData(BufferTarget.UniformBuffer, vector4, offset);
        }

        public void Set(Matrix3 matrix3, int index) {
            var o = _objects[index];
            if (o.Id != UboObject.Matrix3.Id)
                throw new ArgumentException("The argument can't be a Matrix3! Id: " + o.Id);
            var offset = GetStartOffset(index, o);
            if (_vbo.Mapping) {
                unsafe {
                    var pointer = (float*) _vbo.Pointer;
                    pointer += offset >> 2;
                    AddColumn(pointer, matrix3.Column0);
                    AddColumn(pointer + 4, matrix3.Column1);
                    AddColumn(pointer + 8, matrix3.Column2);
                    return;
                }
            }

            _vbo.SetSubData(BufferTarget.UniformBuffer, matrix3, offset);
        }

        private unsafe void AddColumn(float* pointer, Vector3 vector3) {
            *pointer++ = vector3.X;
            *pointer++ = vector3.Y;
            *pointer = vector3.Z;
        }

        public void Set(Matrix4 matrix4, int index) {
            var o = _objects[index];
            if (o.Id != UboObject.Matrix4.Id)
                throw new ArgumentException("The argument can't be a Matrix4! Id: " + o.Id);
            var offset = GetStartOffset(index, o);
            if (_vbo.Mapping) {
                unsafe {
                    var pointer = (float*) _vbo.Pointer;
                    pointer += offset >> 2;
                    AddColumn(pointer, matrix4.Row0);
                    AddColumn(pointer + 4, matrix4.Row1);
                    AddColumn(pointer + 8, matrix4.Row2);
                    AddColumn(pointer + 12, matrix4.Row3);
                    return;
                }
            }

            _vbo.SetSubData(BufferTarget.UniformBuffer, matrix4, offset);
        }

        private unsafe void AddColumn(float* pointer, Vector4 vector4) {
            *pointer++ = vector4.X;
            *pointer++ = vector4.Y;
            *pointer++ = vector4.Z;
            *pointer = vector4.W;
        }

        private int GetStartOffset(int index, UboObject o) {
            var current = 0;
            for (var i = 0; i < index; i++) {
                var next = _objects[i];
                var currentOffset = current % next.Alignment;
                current += next.Size + (currentOffset == 0 ? 0 : next.Alignment - currentOffset);
            }

            var offset = current % o.Alignment;
            if (offset == 0) return current;
            return current + o.Alignment - offset;
        }

        private int CalculateSize() {
            var size = 0;
            foreach (var o in _objects) {
                var currentOffset = size % o.Alignment;
                size += o.Size + (currentOffset == 0 ? 0 : o.Alignment - currentOffset);
            }

            return size;
        }
    }

    public class UboObject{
        public static readonly UboObject Float = new UboObject(0, 4, 4);
        public static readonly UboObject Integer = new UboObject(1, 4, 4);
        public static readonly UboObject Boolean = new UboObject(2, 4, 4);
        public static readonly UboObject Vector2 = new UboObject(3, 8, 8);
        public static readonly UboObject Vector3 = new UboObject(4, 16, 12);
        public static readonly UboObject Vector4 = new UboObject(5, 16, 16);
        public static readonly UboObject Matrix3 = new UboObject(6, 16, 48);
        public static readonly UboObject Matrix4 = new UboObject(7, 16, 64);


        private int _id;
        private int _alignment, _size;

        private UboObject(int id, int alignment, int size) {
            _id = id;
            _alignment = alignment;
            _size = size;
        }

        public int Id => _id;

        public int Alignment => _alignment;

        public int Size => _size;
    }
}