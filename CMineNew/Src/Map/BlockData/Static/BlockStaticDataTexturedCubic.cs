namespace CMineNew.Map.BlockData.Static{
    public class BlockStaticDataTexturedCubic : BlockStaticDataCubic{
        private readonly int _textureIndex;

        public BlockStaticDataTexturedCubic(string id, bool passable, bool lightSource, sbyte lightSourceLight,
            sbyte blockLightPassReduction, sbyte sunlightPassReduction, string texture)
            : base(id, passable, lightSource, lightSourceLight, blockLightPassReduction, sunlightPassReduction) {
            _textureIndex = CMine.TextureMap.Indices[texture];
        }

        public BlockStaticDataTexturedCubic(string id, bool passable, bool lightSource, sbyte lightSourceLight,
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