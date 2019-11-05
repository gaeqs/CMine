using CMineNew.Geometry;

namespace CMineNew.Map.BlockData.Static{
    public class BlockStaticDataTexturedCubic : BlockStaticDataCubic{
        private readonly Area2d _textureArea;

        public BlockStaticDataTexturedCubic(string id, bool passable, bool lightSource, sbyte lightSourceLight,
            sbyte blockLightPassReduction, sbyte sunlightPassReduction, string texture)
            : base(id, passable, lightSource, lightSourceLight, blockLightPassReduction, sunlightPassReduction) {
            _textureArea = CMine.TextureMap.Areas[texture];
        }
        
        public BlockStaticDataTexturedCubic(string id, bool passable, bool lightSource, sbyte lightSourceLight,
            sbyte blockLightPassReduction, sbyte sunlightPassReduction, Area2d texture)
            : base(id, passable, lightSource, lightSourceLight, blockLightPassReduction, sunlightPassReduction) {
            _textureArea = texture;
        }

        public Area2d TextureArea => _textureArea;
    }
}