using System.Collections.Generic;

namespace CMineNew.Color {
    public class TextureManager {
        private readonly Dictionary<string, Texture> _textures;

        public TextureManager() {
            _textures = new Dictionary<string, Texture>();
        }

        public Texture GetOrCreateTexture(string address, bool flipY = false) {
            if (_textures.TryGetValue(address, out var texture)) {
                return texture;
            }

            texture = new Texture(address, flipY);
            _textures.Add(address, texture);
            return texture;
        }

        public Texture GetOrCreateTexture(string name, byte[] data, bool flipY = false) {
            if (_textures.TryGetValue(name, out var texture)) {
                return texture;
            }

            texture = new Texture(name, data, flipY);
            _textures.Add(name, texture);
            return texture;
        }

        public void AddTexture(Texture texture) {
            _textures.Add(texture.Name, texture);
        }

        public void CleanUp() {
            foreach (var texture in _textures.Values) {
                texture.CleanUp();
            }

            _textures.Clear();
        }
    }
}