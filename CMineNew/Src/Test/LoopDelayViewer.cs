using System;
using System.Diagnostics;
using CMineNew.Render.Object;
using CMineNew.Resources.Shaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Test{
    public static class LoopDelayViewer{
        private static readonly ShaderProgram Shader = new ShaderProgram(Shaders.loop_delay_viewer_vertex, Shaders.loop_delay_viewer_fragment);
        private static readonly float[] Delays = new float[1000];
        private static int _amount;
        private static LineVertexArrayObject _vao;
        private static bool _loaded;

        public static void Add(long delay) {
            var millis = delay / CMine.TicksPerSecondF * 1000;
            if (_amount == Delays.Length) {
                Shift();
                Delays[Delays.Length-1] = millis;
            }
            else {
                Delays[_amount] = millis;
                _amount++;
            }
        }

        private static void Shift() {
            for (var i = 0; i < Delays.Length - 1; i++) {
                Delays[i] = Delays[i + 1];
            }
        }

        public static void Draw() {
            if (!_loaded) {
                Load();
            }
            AddToVbo();
            
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);
            Shader.Use();
            GL.LineWidth(2);
            _vao.Bind();
            _vao.DrawArrays(0, 2 + _amount * 2);
        }

        public static void Load() {
            _vao = new LineVertexArrayObject(new Vector3[0], new int[0]);
            _loaded = true;
        }

        private static void AddToVbo() {
            var vbo = _vao.VerticesVbo;
            vbo.Bind(BufferTarget.ArrayBuffer);
            vbo.SetData(BufferTarget.ArrayBuffer, GenerateFloatArray(), BufferUsageHint.StreamDraw);
            VertexBufferObject.Unbind(BufferTarget.ArrayBuffer);
        }

        private static float[] GenerateFloatArray() {
            var array = new float[6 + _amount * 6];
            const float minX = 0f;
            const float maxX = 1f;
            const float minY = -0.8f;
            const float maxY = -0.3f;
            const float maxDelay = 16f;
            var index = 0;
            for (var i = 0; i < _amount; i++) {
                //MIN
                var x =  i / (float) _amount * (maxX - minX) + minX;
                array[index++] = x;
                array[index++] = minY;
                array[index++] = 1;

                //MAX
                array[index++] = x;

                var delay = Delays[i];
                array[index++] = delay / maxDelay * (maxY - minY) + minY;
                array[index++] = 1;
            }

            array[index++] = minX;
            array[index++] = maxY;
            array[index++] = 1;
            
            array[index++] = maxX;
            array[index++] = maxY;
            array[index] = 1;

            return array;
        }
    }
}