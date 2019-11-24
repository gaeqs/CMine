using CMineNew.Geometry;

namespace CMineNew.Map.BlockData.Static{
    public class BlockStaticDataTexturedCross : BlockStaticDataCross{
        private readonly int _textureIndex;

        public BlockStaticDataTexturedCross(string id, bool passable, bool lightSource, sbyte lightSourceLight,
            sbyte blockLightPassReduction, sbyte sunlightPassReduction, string texture)
            : base(id, passable, lightSource, lightSourceLight, blockLightPassReduction, sunlightPassReduction) {
            _textureIndex = CMine.TextureMap.Indices[texture];
        }

        public BlockStaticDataTexturedCross(string id, bool passable, bool lightSource, sbyte lightSourceLight,
            sbyte blockLightPassReduction, sbyte sunlightPassReduction, int texture)
            : base(id, passable, lightSource, lightSourceLight, blockLightPassReduction, sunlightPassReduction) {
            _textureIndex = texture;
        }

        public int TextureIndex => _textureIndex;
        
        public override int GetTextureIndex(BlockFace face) {
            return _textureIndex;
        }
    }
}