using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using CMineNew.Geometry;
using CMineNew.Resources.Textures;
using GraphicEngine.Loader;

namespace CMineNew.Texture{
    public class TextureMap{
        private int _texture;
        private Dictionary<string, Area2d> _areas;

        public TextureMap() {
            _areas = new Dictionary<string, Area2d>();
            var bitmaps = new Dictionary<string, Bitmap>();

            bitmaps.Add("default:stone", ToBitmap(Textures.stone));
            bitmaps.Add("default:dirt", ToBitmap(Textures.dirt));
            bitmaps.Add("default:grass_side", ToBitmap(Textures.grass_side));
            bitmaps.Add("default:grass_top", ToBitmap(Textures.grass_top));
            bitmaps.Add("default:water", ToBitmap(Textures.water));
            bitmaps.Add("default:tall_grass", ToBitmap(Textures.tall_grass));
            bitmaps.Add("default:bricks", ToBitmap(Textures.bricks));
            bitmaps.Add("default:oak_log_side", ToBitmap(Textures.oak_log_side));
            bitmaps.Add("default:oak_log_top", ToBitmap(Textures.oak_log_top));
            bitmaps.Add("default:oak_leaves", ToBitmap(Textures.oak_leaves));
            bitmaps.Add("default:sand", ToBitmap(Textures.sand));

            CreateTextureMap(bitmaps);
        }


        private void CreateTextureMap(Dictionary<string, Bitmap> bitmaps) {
            var count = (int) Math.Ceiling(Math.Sqrt(bitmaps.Count));

            var map = new Bitmap(count * 16, count * 16);
            var graphics = Graphics.FromImage(map);
            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;


            var x = 0;
            var y = 0;
            foreach (var bitmap in bitmaps) {
                graphics.DrawImage(bitmap.Value,
                    new Rectangle(x * 16, y * 16, 16, 16),
                    new Rectangle(0, 0, 16, 16), GraphicsUnit.Pixel);
                _areas.Add(bitmap.Key, new Area2d(x / (float) count, y / (float) count,
                    (x + 1) / (float) count, (y + 1) / (float) count));
                y++;
                if (y != count) continue;
                y = 0;
                x++;
            }

            graphics.Save();
            graphics.Dispose();
            //map.Save(@"D:/test.png", ImageFormat.Png);
            _texture = ImageLoader.Load(map, true, true, false);
        }

        public int Texture => _texture;

        public Dictionary<string, Area2d> Areas => _areas;

        private static Bitmap ToBitmap(byte[] bytes) {
            return (Bitmap) Image.FromStream(new MemoryStream(bytes));
        }
    }
}