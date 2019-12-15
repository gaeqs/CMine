using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using CMineNew.Geometry;
using CMineNew.Loader;
using CMineNew.Resources.Textures;
using OpenTK;

namespace CMineNew.Color{
    public class TextureMap{
        public const int SpriteSize = 16;
        private float _spriteSizeNormalized;
        private int _textureLength;

        private int _texture;
        private readonly Dictionary<string, int> _indices;

        public TextureMap() {
            _indices = new Dictionary<string, int>();
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
            bitmaps.Add("default:torch", ToBitmap(Textures.torch));


            CreateTextureMap(bitmaps);
        }

        public float SpriteSizeNormalized => _spriteSizeNormalized;
        public int TextureLength => _textureLength;

        private void CreateTextureMap(Dictionary<string, Bitmap> bitmaps) {
            _textureLength = (int) Math.Ceiling(Math.Sqrt(bitmaps.Count));

            _spriteSizeNormalized = 1f /  _textureLength;
            
            var map = new Bitmap(_textureLength * SpriteSize, _textureLength * SpriteSize);
            var graphics = Graphics.FromImage(map);
            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;


            var x = 0;
            var y = 0;
            foreach (var bitmap in bitmaps) {
                graphics.DrawImage(bitmap.Value,
                    new Rectangle(x * SpriteSize, y * SpriteSize, SpriteSize, SpriteSize),
                    new Rectangle(0, 0, SpriteSize, SpriteSize), GraphicsUnit.Pixel);
                _indices.Add(bitmap.Key, x * _textureLength + y);
                y++;
                if (y != _textureLength) continue;
                y = 0;
                x++;
            }

            graphics.Save();
            graphics.Dispose();
            //map.Save(@"D:/test.png", ImageFormat.Png);
            _texture = ImageLoader.Load(map, true, true, false);
        }

        public int Texture => _texture;

        public Dictionary<string, int> Indices => _indices;

        private static Bitmap ToBitmap(byte[] bytes) {
            return (Bitmap) Image.FromStream(new MemoryStream(bytes));
        }
    }
}