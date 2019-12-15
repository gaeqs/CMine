namespace CMineNew.Map.BlockData.Static{
    public class BlockStaticDataMultiTexturedCubic : BlockStaticDataCubic{
        private readonly int[] _textureIndices;

        public BlockStaticDataMultiTexturedCubic(string id, bool passable, bool lightSource, sbyte lightSourceLight,
            sbyte blockLightPassReduction, sbyte sunlightPassReduction, string[] texture)
            : base(id, passable, lightSource, lightSourceLight, blockLightPassReduction, sunlightPassReduction) {
            _textureIndices = new int[texture.Length];
            var indices = CMine.TextureMap.Indices;
            for (var i = 0; i < texture.Length; i++) {
                _textureIndices[i] = indices[texture[i]];
            }
        }

        public BlockStaticDataMultiTexturedCubic(string id, bool passable, bool lightSource, sbyte lightSourceLight,
            sbyte blockLightPassReduction, sbyte sunlightPassReduction, int[] texture)
            : base(id, passable, lightSource, lightSourceLight, blockLightPassReduction, sunlightPassReduction) {
            _textureIndices = texture;
        }

        public int[] TextureIndices => _textureIndices;

        public override int GetTextureIndex(BlockFace face) {
            return _textureIndices[(int) face];
        }
    }
}