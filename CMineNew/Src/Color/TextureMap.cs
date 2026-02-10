using System;
using System.Collections.Generic;
using CMineNew.Resources.Textures;
using OpenTK.Graphics.OpenGL;
using StbImageSharp;

namespace CMineNew.Color
{
    public class TextureMap
    {
        public const int SpriteSize = 16;
        private float _spriteSizeNormalized;
        private int _textureLength;

        private int _textureId;
        private readonly Dictionary<string, int> _indices;

        public TextureMap()
        {
            _indices = new Dictionary<string, int>();
            
            var images = new Dictionary<string, ImageResult>();

            images.Add("default:stone", ImageResult.FromMemory(Textures.stone, ColorComponents.RedGreenBlueAlpha));
            images.Add("default:dirt", ImageResult.FromMemory(Textures.dirt, ColorComponents.RedGreenBlueAlpha));
            images.Add("default:grass_side", ImageResult.FromMemory(Textures.grass_side, ColorComponents.RedGreenBlueAlpha));
            images.Add("default:grass_top", ImageResult.FromMemory(Textures.grass_top, ColorComponents.RedGreenBlueAlpha));
            images.Add("default:water", ImageResult.FromMemory(Textures.water, ColorComponents.RedGreenBlueAlpha));
            images.Add("default:tall_grass", ImageResult.FromMemory(Textures.tall_grass, ColorComponents.RedGreenBlueAlpha));
            images.Add("default:bricks", ImageResult.FromMemory(Textures.bricks, ColorComponents.RedGreenBlueAlpha));
            images.Add("default:oak_log_side", ImageResult.FromMemory(Textures.oak_log_side, ColorComponents.RedGreenBlueAlpha));
            images.Add("default:oak_log_top", ImageResult.FromMemory(Textures.oak_log_top, ColorComponents.RedGreenBlueAlpha));
            images.Add("default:oak_leaves", ImageResult.FromMemory(Textures.oak_leaves, ColorComponents.RedGreenBlueAlpha));
            images.Add("default:sand", ImageResult.FromMemory(Textures.sand, ColorComponents.RedGreenBlueAlpha));
            images.Add("default:torch", ImageResult.FromMemory(Textures.torch, ColorComponents.RedGreenBlueAlpha));

            CreateTextureMap(images);
        }

        public float SpriteSizeNormalized => _spriteSizeNormalized;
        public int TextureLength => _textureLength;
        public int Texture => _textureId;
        public Dictionary<string, int> Indices => _indices;

        private void CreateTextureMap(Dictionary<string, ImageResult> images)
        {
            _textureLength = (int) Math.Ceiling(Math.Sqrt(images.Count));
            _spriteSizeNormalized = 1f / _textureLength;

            int atlasWidth = _textureLength * SpriteSize;
            int atlasHeight = _textureLength * SpriteSize;
            
            byte[] atlasData = new byte[atlasWidth * atlasHeight * 4];

            var x = 0;
            var y = 0;
            
            foreach (var kvp in images)
            {
                ImageResult sprite = kvp.Value;
                
                for (int row = 0; row < SpriteSize; row++)
                {
                    if (row >= sprite.Height) break;

                    int sourceIndex = row * sprite.Width * 4;

                    int destRow = (y * SpriteSize) + row;
                    int destCol = (x * SpriteSize);
                    int destIndex = (destRow * atlasWidth + destCol) * 4;

                    int bytesToCopy = Math.Min(sprite.Width, SpriteSize) * 4;

                    Array.Copy(sprite.Data, sourceIndex, atlasData, destIndex, bytesToCopy);
                }

                _indices.Add(kvp.Key, x * _textureLength + y);
                y++;
                if (y == _textureLength)
                {
                    y = 0;
                    x++;
                }
            }

            _textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _textureId);
            
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 
                atlasWidth, atlasHeight, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, atlasData);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
    }
}