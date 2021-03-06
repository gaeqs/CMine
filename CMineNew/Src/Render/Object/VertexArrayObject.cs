using System;
using System.Collections.ObjectModel;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Render.Object{
    public class VertexArrayObject{
        private static int _currentBindVao = 0;

        public static void Unbind() {
            GL.BindVertexArray(0);
            _currentBindVao = 0;
        }

        public static void Bind(int id) {
            GL.BindVertexArray(id);
            _currentBindVao = id;
        }

        private readonly int _id;
        private readonly int _indicesAmount;
        private readonly Collection<VertexBufferObject> _buffers;
        private readonly Collection<int> _attributes;

        public VertexArrayObject(Vertex[] vertices, int[] indices) {
            GL.GenVertexArrays(1, out _id);
            if (_id == 0) {
                throw new System.Exception("Couldn't create VAO. ID is 0.");
            }

            _buffers = new Collection<VertexBufferObject>();
            _attributes = new Collection<int>();
            _indicesAmount = indices.Length;
            Bind();
            GenerateDrawVbo(vertices);
            GenerateEbo(indices);
            Unbind();
        }

        public VertexArrayObject() {
            GL.GenVertexArrays(1, out _id);
            _buffers = new Collection<VertexBufferObject>();
            _attributes = new Collection<int>();
        }

        public int Id => _id;

        public int IndicesAmount => _indicesAmount;

        public void Bind() {
            if(_currentBindVao == _id) return;
            GL.BindVertexArray(_id);
            _currentBindVao = _id;
        }

        public void AttributePointer(int index, int size, int stride, int offset, bool elementAttribute) {
            if (_currentBindVao != _id) {
                Bind();
            }

            GL.VertexAttribPointer(index, size, VertexAttribPointerType.Float, false, stride, offset);
            GL.VertexAttribDivisor(index, elementAttribute ? 1 : 0);
            GL.EnableVertexAttribArray(index);
            _attributes.Add(index);
        }

        public void CleanUp() {
            foreach (var buffer in _buffers) {
                buffer.CleanUp();
            }

            GL.DeleteVertexArray(_id);
        }

        public void LinkBuffer(VertexBufferObject vbo) {
            _buffers.Add(vbo);
        }

        public void UnlinkBuffer(VertexBufferObject vbo) {
            _buffers.Remove(vbo);
        }

        public void Draw() {
            GL.DrawElements(PrimitiveType.Triangles, _indicesAmount,
                DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        public void DrawnInstanced(int amount) {
            GL.DrawElementsInstanced(PrimitiveType.Triangles, _indicesAmount,
                DrawElementsType.UnsignedInt, IntPtr.Zero, amount);
        }

        public void DrawArrays(int first, int count) {
            GL.DrawArrays(PrimitiveType.Triangles, first, count);
        }
        
        public void DrawnArraysInstanced(int first, int count, int amount) {
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, count, amount);
        }

        #region private methods

        private void GenerateDrawVbo(Vertex[] vertices) {
            var vbo = new VertexBufferObject();
            LinkBuffer(vbo);
            var data = ToFloatArray(vertices);
            vbo.Bind(BufferTarget.ArrayBuffer);
            vbo.SetData(BufferTarget.ArrayBuffer, data, BufferUsageHint.StaticDraw);

            var builder = new AttributePointerBuilder(this, Vertex.Size, 0);
            builder.AddPointer(3, false);
            builder.AddPointer(3, false);
            builder.AddPointer(2, false);
            VertexBufferObject.Unbind(BufferTarget.ArrayBuffer);
        }

        private void GenerateEbo(int[] indices) {
            var vbo = new VertexBufferObject();
            LinkBuffer(vbo);
            vbo.Bind(BufferTarget.ElementArrayBuffer);
            vbo.SetData(BufferTarget.ElementArrayBuffer, indices, BufferUsageHint.StaticDraw);
        }

        //private void EnableAttributes() {
        //    foreach (var i in _attributes) {
        //     
        //        GL.EnableVertexAttribArray(i);
        //    }
        //}

        //private void DisableAttributes() {
        //    foreach (var i in _attributes) {
        //        GL.DisableVertexAttribArray(i);
        //    }
        //}

        private static float[] ToFloatArray(Vertex[] vertices) {
            var array = new float[vertices.Length * Vertex.Size];
            var current = 0;
            for (uint index = 0; index < vertices.Length; index++) {
                array[current++] = vertices[index]._position.X;
                array[current++] = vertices[index]._position.Y;
                array[current++] = vertices[index]._position.Z;
                array[current++] = vertices[index]._normal.X;
                array[current++] = vertices[index]._normal.Y;
                array[current++] = vertices[index]._normal.Z;
                array[current++] = vertices[index]._textureCoords.X;
                array[current++] = vertices[index]._textureCoords.Y;
            }

            return array;
        }

        #endregion
    }

    public class AttributePointerBuilder{
        private readonly VertexArrayObject _vao;
        private readonly int _elementSize;

        private int _currentOffset;
        private int _currentIndex;

        public AttributePointerBuilder(VertexArrayObject vao, int elementSize, int indexOffset) {
            _vao = vao;
            _elementSize = elementSize * sizeof(float);
            _currentOffset = 0;
            _currentIndex = indexOffset;
        }

        public void AddPointer(int size, bool elementAttribute) {
            _vao.AttributePointer(_currentIndex, size, _elementSize, _currentOffset, elementAttribute);
            _currentIndex++;
            _currentOffset += size * sizeof(float);
        }
    }
}