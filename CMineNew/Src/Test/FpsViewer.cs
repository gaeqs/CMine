using System;
using System.Globalization;
using CMineNew.Map;
using CMineNew.Render;
using CMineNew.Render.Object;
using CMineNew.Text;
using OpenTK;
using OpenTK.Graphics;

namespace CMineNew.Test{
    public class FpsViewer : StaticText{
        private static readonly long MaxMemory = (long) Math.Pow(2, 32);

        private long _ticks;
        private int _loops;
        private int _lastChunks;

        public FpsViewer(TrueTypeFont font) : base(font, "INIT", new Vector2(-1, 0.9f), Color4.White) {
        }

        public override void Tick(long dif, Room room) {
            _ticks += dif;
            _loops++;
            if (_ticks < CMine.TicksPerSecond) return;

            var memory = GC.GetTotalMemory(false) * 100 / MaxMemory;
            var chunks = 0;
            var lights = 0;
            if (CMine.Window.Room is World world) {
                chunks = world.AsyncChunkGenerator.ChunksToGenerateSize;
                lights = world.RenderData.LightManager.PointLights.Count;
            }

            var chunkVelocity = _lastChunks - chunks;
            _lastChunks = chunks;


            Text = "(" + memory + "%) (" + chunks + ") [ " + chunkVelocity + "] " +
                   "(" + VertexBufferObject.Buffers + ") {" + BuffersSize() + "} "+ "("+lights+") " + _loops;
            _ticks = 0;
            _loops = 0;
        }

        private static string BuffersSize() {
            var memory = VertexBufferObject.BuffersMemory;
            string st;
            if (memory > 1000000) {
                st = Math.Round(memory / 1000000f, 3) + " MB";
            }
            else if (memory > 1000) {
                st = Math.Round(memory / 1000f, 3) + " KB";
            }
            else {
                st = memory + " B";
            }

            return st;
        }
    }
}