using OpenTK.Graphics.OpenGL;

namespace CMineNew.Render.Object{
    public class VertexBufferObject{
        private int _id;

        public static void Unbind(BufferTarget target) {
            GL.BindBuffer(target, 0);
        }

        public VertexBufferObject() {
            GL.GenBuffers(1, out _id);
        }

        public void Bind(BufferTarget target) {
            GL.BindBuffer(target, _id);
        }

        public void SetData(BufferTarget target, float[] data, BufferUsageHint usageHint) {
            GL.BufferData(target, data.Length * sizeof(float), data, usageHint);
        }
        
        public void SetData(BufferTarget target, int[] data, BufferUsageHint usageHint) {
            GL.BufferData(target, data.Length * sizeof(int), data, usageHint);
        }

        public void CleanUp() {
            GL.DeleteBuffer(_id);
        }
    }
}