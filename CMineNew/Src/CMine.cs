using System;
using System.Drawing;
using System.IO;
using CMineNew.Map;
using CMineNew.Map.BlockData;
using CMineNew.Map.BlockData.Model;
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

        public static readonly string MainFolder =
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
            Path.DirectorySeparatorChar + "CMine";

        //Temporary constant.
        public const int ChunkRadius = 4;

        public static Window Window;
        public static TextureMap Textures;

        public static void Load() {
            Window = new Window(1920 / 2, 1080 / 2,
                GameWindowFlags.Default, true, (window, args) => {
                    Directory.CreateDirectory(MainFolder);
                    Textures = new TextureMap();
                    BlockManager.Load();
                    BlockModelManager.Load();
                    Pointer.Load();

                    var ttf = new TrueTypeFont(new Font(new FontFamily("Arial"), 17));

                    var world = new World("test", ttf);

                    world.StaticTexts.Add(new PositionViewer(ttf));
                    world.StaticTexts.Add(new FpsViewer(ttf));

                    Window.Room = world;
                });
            Window.Run();
        }

        public static int Sum(int a, int b) {
            return a + b;
        }
    }
}