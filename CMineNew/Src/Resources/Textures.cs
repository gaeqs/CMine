using System;
using System.IO;

namespace CMineNew.Resources.Textures
{
    public static class Textures
    {
        private static readonly string BasePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Textures");

        private static byte[] Load(string relativePath)
        {
            var fullPath = Path.Combine(BasePath, relativePath);

            if (!File.Exists(fullPath))
            {
                fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            }

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Texture file not found: {fullPath}");
            }

            return File.ReadAllBytes(fullPath);
        }

        public static byte[] bricks => Load("bricks.png");
        public static byte[] dirt => Load("dirt.png");
        public static byte[] grass_side => Load("grass_side.png");
        public static byte[] grass_top => Load("grass_top.png");
        public static byte[] oak_leaves => Load("oak_leaves.png");
        public static byte[] oak_log_side => Load("oak_log_side.png");
        public static byte[] oak_log_top => Load("oak_log_top.png");
        public static byte[] sand => Load("sand.png");
        public static byte[] stone => Load("stone.png");
        public static byte[] tall_grass => Load("tall_grass.png");
        public static byte[] torch => Load("torch.png");
        public static byte[] water => Load("water.png");
        public static byte[] pointer => Load("pointer.png");
        public static byte[] gui_hotbar => Load("Gui/hotbar.png");
        public static byte[] sky_box_back => Load("SkyBox/back.png");
        public static byte[] sky_box_bottom => Load("SkyBox/bottom.png");
        public static byte[] sky_box_front => Load("SkyBox/front.png");
        public static byte[] sky_box_left => Load("SkyBox/left.png");
        public static byte[] sky_box_right => Load("SkyBox/right.png");
        public static byte[] sky_box_top => Load("SkyBox/top.png");
    }
}