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

        private readonly int _id;
        private readonly int _indicesAmount;
        private Collection<VertexBufferObject> _buffers;
        private Collection<int> _attributes;

        private VertexArrayObject(Vertex[] vertices, int[] indices) {
            GL.GenVertexArrays(1, out _id);
            _buffers = new Collection<VertexBufferObject>();
            _attributes = new Collection<int>();
            _indicesAmount = indices.Length;
            Bind();
            GenerateDrawVbo(vertices);
            GenerateEbo(indices);
            Unbind();
        }

        public void Bind() {
            GL.BindVertexArray(_id);
            _currentBindVao = _id;
        }

        public void AttributePointer(int index, int size, int stride, int offset, bool elementAttribute) {
            if (_currentBindVao != _id) {
                Bind();
            }

            GL.VertexAttribPointer(index, size, VertexAttribPointerType.Float, false, stride, offset);
            GL.VertexAttribDivisor(index, elementAttribute ? 0 : 1);
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
            EnableAttributes();
            GL.DrawElements(PrimitiveType.Triangles, _indicesAmount,
                DrawElementsType.UnsignedInt, IntPtr.Zero);
            DisableAttributes();
        }

        public void DrawnInstanced(int amount) {
            EnableAttributes();
            GL.DrawElementsInstanced(PrimitiveType.Triangles, _indicesAmount,
                DrawElementsType.UnsignedInt, IntPtr.Zero, amount);
            DisableAttributes();
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
            builder.AddPointer(4, false);
            builder.AddPointer(5, false);
            VertexBufferObject.Unbind(BufferTarget.ArrayBuffer);
        }

        private void GenerateEbo(int[] indices) {
            var vbo = new VertexBufferObject();
            LinkBuffer(vbo);
            vbo.Bind(BufferTarget.ElementArrayBuffer);
            vbo.SetData(BufferTarget.ElementArrayBuffer, indices, BufferUsageHint.StaticDraw);
        }

        private void EnableAttributes() {
            foreach (var i in _attributes) {
                GL.EnableVertexAttribArray(i);
            }
        }

        private void DisableAttributes() {
            foreach (var i in _attributes) {
                GL.EnableVertexAttribArray(i);
            }
        }

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