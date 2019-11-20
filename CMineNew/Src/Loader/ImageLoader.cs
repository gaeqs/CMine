using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK.Graphics.OpenGL;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace CMineNew.Loader{
    public static class ImageLoader{

        public static int Load(byte[] bytes, bool nearest = true, bool repeat = true, bool flipY = true) {
            var bitmap = (Bitmap)Image.FromStream(new MemoryStream(bytes));
            return Load(bitmap, nearest, repeat, flipY);
        }
        
        
        public static int Load(string url, bool nearest = true, bool repeat = true, bool flipY = true) {
            var bitmap = new Bitmap(url);
            return Load(bitmap, nearest, repeat, flipY);
        }
        
        public static int Load(Bitmap bitmap, bool nearest = true, bool repeat = true, bool flipY = true) {
            if (flipY)
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            GL.GenTextures(1, out int id);
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.PixelStore(PixelStoreParameter.PackAlignment, 1);
            var quality = nearest ? (int) All.Nearest : (int) All.Linear;
            var wrap = repeat ? (int) All.Repeat : (int) All.ClampToEdge;
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, quality);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, quality);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, wrap);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, wrap);

            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            GL.GenerateMipmap(TextureTarget.Texture2D);
            bitmap.UnlockBits(data);
            return id;
        }
    }
}
