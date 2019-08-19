using System;
using System.Drawing;
using CMine.Texture;
using CMineNew.Geometry;
using CMineNew.Map;
using CMineNew.Map.BlockData.Type;
using CMineNew.Render;
using CMineNew.Test;
using CMineNew.Text;
using OpenTK;

namespace CMineNew{
    public class CMine{
        public const int TicksPerSecond = 10000000;
        public const float TicksPerSecondF = 10000000f;

        public static Window Window;
        public static TextureMap Textures;

        public static void Load() {
            Window = new Window(1920, 1080,
                GameWindowFlags.Fullscreen, false, (window, args) => {
                    Textures = new TextureMap();
                    var world = new World("test");

                    for (var x = 0; x < 100; x++) {
                        Console.WriteLine();
                        for (var z = 0; z < 100; z++) {
                            for (var y = 80; y < 90 + x / 2; y++) {
                                world.SetBlock(new BlockStone(null, Vector3i.Zero), new Vector3i(x, y, z));
                            }
                        }
                    }

                    var ttf = new TrueTypeFont(new Font(new FontFamily("Arial"), 10));
                    world.StaticTexts.Add(new PositionViewer(ttf));

                    world.StaticTexts.Add(new FPSViewer(ttf));

                    Window.Room = world;
                });
            Window.Run();
        }
    }
}