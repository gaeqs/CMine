using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using CMineNew.Geometry;
using CMineNew.Loader;

namespace CMineNew.Text{
    public class TrueTypeFont{
        private readonly Font _font;
        private int _textureId;
        private readonly Dictionary<char, Area2d> _characterMap;
        private readonly Dictionary<char, SizeF> _charactersSize;

        private int _maxHeight;

        public TrueTypeFont(Font font) {
            _font = font;
            _characterMap = new Dictionary<char, Area2d>();
            _charactersSize = new Dictionary<char, SizeF>();
            ParseCharacters();
        }

        private void ParseCharacters() {
            Console.WriteLine("Parsing characters...");
            var images = new Image[256 - 32];

            var imageWidth = 0;
            _maxHeight = 0;
            for (var i = 32; i < 256; i++) {
                if (i == 127) continue;
                var c = (char) i;
                var current = ToImage(c);
                images[i - 32] = current;
                if (current == null) continue;
                imageWidth += current.Width;
                _maxHeight = Math.Max(_maxHeight, current.Height);
            }

            var final = new Bitmap(1000, _maxHeight * (imageWidth / 1000));
            var imageHeight = final.Height;
            imageWidth = final.Width;
            
            var graphics = Graphics.FromImage(final);
            graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            var x = 0;
            var y = 0;
            foreach (var image in images) {
                if (image == null) continue;
                if (x + image.Width > 1000) {
                    x = 0;
                    y += _maxHeight;
                }
                graphics.DrawImage(image, new PointF(x, y));
                x += image.Width;
            }

            x = 0;
            y = 0;
            for (var i = 32; i < 256; i++) {
                if (i == 127) continue;
                var c = (char) i;
                var current = images[i - 32];
                if (current == null) continue;
                if (x + current.Width > 1000) {
                    x = 0;
                    y += _maxHeight;
                }
                _characterMap.Add(c, new Area2d(x/(float)imageWidth,   y /(float)imageHeight, 
                    (x + current.Width)/(float)imageWidth,  (y + current.Height)/(float)imageHeight));
                x += current.Width;
            }

            graphics.Save();
            _textureId = ImageLoader.Load(final, true, false, false);
        }

        private Image ToImage(char c) {
            var image = new Bitmap(1, 1);
            var graphics = Graphics.FromImage(image);
            graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            graphics.PageUnit = GraphicsUnit.Pixel;
            var size = graphics.MeasureString(c.ToString(), _font);
            _charactersSize.Add(c, size);
            image = new Bitmap((int) Math.Ceiling(size.Width),
                (int) Math.Ceiling(size.Height));
            graphics = Graphics.FromImage(image);
            graphics.DrawString(c.ToString(), _font, Brushes.White,
                new PointF(0, 0));
            graphics.Save();
            return image;
        }

        public int TextureId => _textureId;

        public Font Font => _font;

        public Dictionary<char, Area2d> CharacterMap => _characterMap;

        public Dictionary<char, SizeF> CharactersSize => _charactersSize;
    }
}