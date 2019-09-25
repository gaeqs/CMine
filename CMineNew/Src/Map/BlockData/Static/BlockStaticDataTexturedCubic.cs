using CMineNew.Geometry;

namespace CMineNew.Map.BlockData.Static{
    public class BlockStaticDataTexturedCubic : BlockStaticDataCubic{
        private readonly Area2d _textureArea;

        public BlockStaticDataTexturedCubic(string id, bool passable, bool lightSource, int lightSourceLight,
            int blockLightPassReduction, int sunlightPassReduction, string texture)
            : base(id, passable, lightSource, lightSourceLight, blockLightPassReduction, sunlightPassReduction) {
            _textureArea = CMine.Textures.Areas[texture];
        }
        
        public BlockStaticDataTexturedCubic(string id, bool passable, bool lightSource, int lightSourceLight,
            int blockLightPassReduction, int sunlightPassReduction, Area2d texture)
            : base(id, passable, lightSource, lightSourceLight, blockLightPassReduction, sunlightPassReduction) {
            _textureArea = texture;
        }

        public Area2d TextureArea => _textureArea;
    }
}