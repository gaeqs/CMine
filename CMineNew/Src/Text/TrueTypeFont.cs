using CMineNew.Geometry;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using StbTrueTypeSharp;

namespace CMineNew.Text
{
    public unsafe class TrueTypeFont
    {
        private int _textureId;
        private readonly Dictionary<char, Area2d> _characterMap;
        private readonly Dictionary<char, Vector2> _charactersSize;
        private readonly float _fontSize;
        private readonly byte[] _fontData;

        public TrueTypeFont(string ttfFilePath, float fontSize)
        {
            if (!File.Exists(ttfFilePath))
                throw new FileNotFoundException("Font file not found", ttfFilePath);

            _fontData = File.ReadAllBytes(ttfFilePath);
            _fontSize = fontSize;
            _characterMap = new Dictionary<char, Area2d>();
            _charactersSize = new Dictionary<char, Vector2>();

            BakeFontBitmap();
        }

        private void BakeFontBitmap()
        {
            Console.WriteLine("Parsing characters with StbTrueType (Unsafe)...");

            const int atlasWidth = 1024;

            fixed (byte* fontDataPtr = _fontData)
            {
                var fontInfo = new StbTrueType.stbtt_fontinfo();
                if (StbTrueType.stbtt_InitFont(fontInfo, fontDataPtr, 0) == 0)
                    throw new System.Exception("Failed to init font");

                float scale = StbTrueType.stbtt_ScaleForPixelHeight(fontInfo, _fontSize);

                var charData = new Dictionary<char, CharRawData>();
                int rowHeight = 0;

                for (int i = 32; i < 127; i++)
                {
                    char c = (char)i;

                    int width, height, xoff, yoff;

                    byte* bitmapPtr =
                        StbTrueType.stbtt_GetCodepointBitmap(fontInfo, 0, scale, c, &width, &height, &xoff, &yoff);

                    int advanceWidth, leftSideBearing;
                    StbTrueType.stbtt_GetCodepointHMetrics(fontInfo, c, &advanceWidth, &leftSideBearing);

                    _charactersSize[c] = new Vector2(advanceWidth * scale, height);
                    rowHeight = Math.Max(rowHeight, height);

                    byte[] managedBitmap = null;
                    if (bitmapPtr != null && width > 0 && height > 0)
                    {
                        managedBitmap = new byte[width * height];
                        for (int j = 0; j < width * height; j++)
                        {
                            managedBitmap[j] = bitmapPtr[j];
                        }

                        StbTrueType.stbtt_FreeBitmap(bitmapPtr, null);
                    }

                    charData[c] = new CharRawData(managedBitmap, width, height);
                }

                int totalHeight = EstimateHeight(charData, atlasWidth, rowHeight);
                byte[] atlasPixels = new byte[atlasWidth * totalHeight * 4]; // RGBA

                int currentX = 0;
                int currentY = 0;

                for (int i = 32; i < 127; i++)
                {
                    char c = (char)i;
                    var data = charData[c];

                    if (currentX + data.Width > atlasWidth)
                    {
                        currentX = 0;
                        currentY += rowHeight;
                    }

                    if (data.Pixels != null)
                    {
                        for (int y = 0; y < data.Height; y++)
                        {
                            for (int x = 0; x < data.Width; x++)
                            {
                                byte alpha = data.Pixels[y * data.Width + x];
                                if (alpha == 0) continue;

                                int destIndex = ((currentY + y) * atlasWidth + (currentX + x)) * 4;

                                atlasPixels[destIndex + 0] = 255;
                                atlasPixels[destIndex + 1] = 255;
                                atlasPixels[destIndex + 2] = 255;
                                atlasPixels[destIndex + 3] = alpha;
                            }
                        }
                    }

                    float u0 = currentX / (float)atlasWidth;
                    float v0 = currentY / (float)totalHeight;
                    float u1 = (currentX + data.Width) / (float)atlasWidth;
                    float v1 = (currentY + data.Height) / (float)totalHeight;
                    _characterMap[c] = new Area2d(u0, v0, u1, v1);

                    currentX += data.Width + 1;
                }

                _textureId = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, _textureId);
                GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                    (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                    (int)TextureMagFilter.Linear);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, atlasWidth, totalHeight, 0,
                    PixelFormat.Rgba, PixelType.UnsignedByte, atlasPixels);
            }
        }

        private int EstimateHeight(Dictionary<char, CharRawData> chars, int width, int lineHeight)
        {
            int x = 0;
            int y = 0;
            foreach (var kvp in chars)
            {
                if (x + kvp.Value.Width > width)
                {
                    x = 0;
                    y += lineHeight;
                }

                x += kvp.Value.Width + 1;
            }

            return y + lineHeight;
        }

        private struct CharRawData
        {
            public byte[] Pixels;
            public int Width;
            public int Height;

            public CharRawData(byte[] pixels, int width, int height)
            {
                Pixels = pixels;
                Width = width;
                Height = height;
            }
        }

        public int TextureId => _textureId;
        public Dictionary<char, Area2d> CharacterMap => _characterMap;
        public Dictionary<char, Vector2> CharactersSize => _charactersSize;
    }
}