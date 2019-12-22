using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CMineNew.Collision;
using CMineNew.Util;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Render.Object{
    public class LineVertexArrayObject{
        private static int _currentBindVao = 0;

        public static void Unbind() {
            GL.BindVertexArray(0);
            _currentBindVao = 0;
        }

        private readonly int _id;
        private readonly int _indicesAmount;
        private readonly Collection<VertexBufferObject> _buffers;
        private readonly Collection<int> _attributes;
        private VertexBufferObject _verticesVbo;

        public LineVertexArrayObject(Vector3[] vertices, int[] indices) {
            GL.GenVertexArrays(1, out _id);
            _buffers = new Collection<VertexBufferObject>();
            _attributes = new Collection<int>();
            _indicesAmount = indices.Length;
            Bind();
            GenerateDrawVbo(vertices);
            if (indices.Length > 0) {
                GenerateEbo(indices);
            }

            Unbind();
        }

        public LineVertexArrayObject(IReadOnlyList<Aabb> collisionBoxes) {
            var vertices = new Vector3[collisionBoxes.Count << 3];
            var indices = new int[collisionBoxes.Count * 24];
            
            for (var i = 0; i < collisionBoxes.Count; i++) {
                var boxVertices = collisionBoxes[i].GetVertices();
                var boxIndices = ModelLinesUtil.CalculateLinesIndices(boxVertices);

                for (var n = 0; n < 8; n++) {
                    vertices[(i << 3) + n] = boxVertices[n];
                }
                for (var n = 0; n < 24; n++) {
                    indices[i * 24 + n] = boxIndices[n] + (i << 3);
                }
            }

            GL.GenVertexArrays(1, out _id);
            _buffers = new Collection<VertexBufferObject>();
            _attributes = new Collection<int>();
            _indicesAmount = indices.Length;
            Bind();
            GenerateDrawVbo(vertices);
            if (indices.Length > 0) {
                GenerateEbo(indices);
            }

            Unbind();
        }

        public int Id => _id;

        public int IndicesAmount => _indicesAmount;

        public VertexBufferObject VerticesVbo => _verticesVbo;

        public void Bind() {
            GL.BindVertexArray(_id);
            _currentBindVao = _id;
        }

        public void AttributePointer(int index, int size, int stride, int offset, bool elementAttribute) {
            if (_currentBindVao != _id) {
                Bind();
            }

            GL.VertexAttribPointer(index, size, VertexAttribPointerType.Float, false, stride, offset);
            GL.VertexAttribDivisor(index, elementAttribute ? 1 : 0);
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
            GL.DrawElements(PrimitiveType.Lines, _indicesAmount,
                DrawElementsType.UnsignedInt, IntPtr.Zero);
            DisableAttributes();
        }

        public void DrawnInstanced(int amount) {
            EnableAttributes();
            GL.DrawElementsInstanced(PrimitiveType.Lines, _indicesAmount,
                DrawElementsType.UnsignedInt, IntPtr.Zero, amount);
            DisableAttributes();
        }

        public void DrawArrays(int index, int amount) {
            EnableAttributes();
            GL.DrawArrays(PrimitiveType.Lines, index, amount);
            DisableAttributes();
        }

        #region private methods

        private void GenerateDrawVbo(Vector3[] vertices) {
            var vbo = new VertexBufferObject();
            LinkBuffer(vbo);
            var data = ToFloatArray(vertices);
            vbo.Bind(BufferTarget.ArrayBuffer);
            vbo.SetData(BufferTarget.ArrayBuffer, data, BufferUsageHint.StaticDraw);
            AttributePointer(0, 3, 3 * sizeof(float), 0, false);
            VertexBufferObject.Unbind(BufferTarget.ArrayBuffer);
            _verticesVbo = vbo;
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
                GL.DisableVertexAttribArray(i);
            }
        }

        private static float[] ToFloatArray(Vector3[] vertices) {
            var array = new float[vertices.Length * 3]; //3 = Size of Vector3 in floats.
            var current = 0;
            for (uint index = 0; index < vertices.Length; index++) {
                array[current++] = vertices[index].X;
                array[current++] = vertices[index].Y;
                array[current++] = vertices[index].Z;
            }

            return array;
        }

        #endregion
    }
}