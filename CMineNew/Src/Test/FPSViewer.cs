using System;
using System.Globalization;
using CMineNew.Map;
using CMineNew.Render;
using CMineNew.Text;
using OpenTK;
using OpenTK.Graphics;

namespace CMineNew.Test{
    public class FPSViewer : StaticText{
        private static readonly long MaxMemory = (long) Math.Pow(2, 32);

        private long ticks;
        private int lastChunks;

        public FPSViewer(TrueTypeFont font) : base(font, "INIT", new Vector2(-1, 0.9f), Color4.White) {
        }

        public override void Tick(long dif, Room room) {
            ticks += dif;
            if (!(ticks > CMine.TicksPerSecond * 0.1f)) return;

            var memory = GC.GetTotalMemory(false) * 100 / MaxMemory;
            var chunks = 0;
            var unloadedChunks = 0;
            //if (CMine.Window.Room is World world) {
            //    chunks = world.AsyncChunkGenerator.ChunksToGenerate.Size();
            //    unloadedChunks = world.AsyncChunkTrashCan.Queue.Size();
            //}


            var chunkVelocity = (lastChunks - chunks) * 10;
            lastChunks = chunks;


            var fps = Math.Ceiling(CMine.TicksPerSecond / (float) dif).ToString(CultureInfo.InvariantCulture);

            Text = "(" + memory + "%) (" + chunks + ") [ " + chunkVelocity + "] (" + unloadedChunks + ")" + fps;
            ticks = 0;
        }
    }
}