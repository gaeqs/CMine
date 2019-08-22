using System;
using System.Drawing;
using CMineNew.Geometry;
using CMineNew.Map;
using CMineNew.Map.BlockData.Model;
using CMineNew.Map.BlockData.Snapshot;
using CMineNew.Map.Generator;
using CMineNew.RayTrace;
using CMineNew.Render;
using CMineNew.Test;
using CMineNew.Text;
using CMineNew.Texture;
using OpenTK;

namespace CMineNew{
    public class CMine{
        public const int TicksPerSecond = 10000000;
        public const float TicksPerSecondF = 10000000f;

        //Temporary constant.
        public const int ChunkRadius = 8;

        public static Window Window;
        public static TextureMap Textures;

        public static void Load() {
            Window = new Window(1920, 1080,
                GameWindowFlags.Fullscreen, false, (window, args) => {
                    Textures = new TextureMap();
                    BlockModelManager.Load();
                    Pointer.Load();
                    var world = new World("test");

                    //var generator = new DefaultWorldGenerator(world, new Random().Next());
                    //var i = 0;
                    //var now = DateTime.Now.Ticks;
                    //for (var x = -2; x < 2; x++) {
                    //    for (var z = -2; z < 2; z++) {
                    //        for (var y = -3; y < 7; y++) {
                    //            var chunk = world.CreateChunk(new Vector3i(x, y, z));
                    //            generator.GenerateChunkData(chunk);
                    //            i++;
                    //        }
                    //    }
                    //}

                    //var delay = (DateTime.Now.Ticks - now) / TicksPerSecondF;
                    //Console.WriteLine(i + " chunks created in " + delay + " seconds. (" + i * Chunk.ChunkVolume +
                    //                  " blocks updated.)");

                    var ttf = new TrueTypeFont(new Font(new FontFamily("Arial"), 10));
                    world.StaticTexts.Add(new PositionViewer(ttf));

                    world.StaticTexts.Add(new FpsViewer(ttf));

                    Window.Room = world;
                });
            Window.Run();
        }
    }
}