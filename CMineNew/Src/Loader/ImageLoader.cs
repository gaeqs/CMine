using OpenTK.Graphics.OpenGL;
using StbImageSharp;

namespace CMineNew.Loader
{
    public static class ImageLoader
    {
        public static int Load(string path, bool nearest = true, bool repeat = true, bool flipY = true)
        {
            using var stream = File.OpenRead(path);
            return Load(stream, nearest, repeat, flipY);
        }

        public static int Load(byte[] bytes, bool nearest = true, bool repeat = true, bool flipY = true)
        {
            using var stream = new MemoryStream(bytes);
            return Load(stream, nearest, repeat, flipY);
        }

        public static int Load(Stream stream, bool nearest = true, bool repeat = true, bool flipY = true)
        {
            StbImage.stbi_set_flip_vertically_on_load(flipY ? 1 : 0);

            ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            int minFilter = nearest ? (int)TextureMinFilter.Nearest : (int)TextureMinFilter.Linear;
            int magFilter = nearest ? (int)TextureMagFilter.Nearest : (int)TextureMagFilter.Linear;
            int wrapMode = repeat ? (int)TextureWrapMode.Repeat : (int)TextureWrapMode.ClampToEdge;

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, magFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, wrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, wrapMode);

            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                image.Width,
                image.Height,
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                image.Data
            );

            // 6. Mipmaps
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return id;
        }
    }
};