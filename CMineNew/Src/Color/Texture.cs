using System;
using CMineNew.Loader;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Color {
    public class Texture {
        private readonly string _name;
        private readonly int _id;

        public Texture(string name, int id) {
            _name = name;
            _id = id;
        }

        public Texture(string address, bool flipY = false) {
            _name = address;
            _id = ImageLoader.Load(address, true, true, flipY);
        }

        public Texture(string name, byte[] data, bool flipY = false) {
            _name = name;
            _id = ImageLoader.Load(data, true, true, flipY);
        }

        public string Name => _name;

        public int Id => _id;

        public void CleanUp() {
            GL.DeleteTexture(_id);
        }

        public void Bind(TextureTarget target = TextureTarget.Texture2D) {
            GL.BindTexture(target, _id);
        }

        public void BindAs(TextureUnit unit, TextureTarget target = TextureTarget.Texture2D) {
            GL.ActiveTexture(unit);
            Bind(target);
        }
    }
}