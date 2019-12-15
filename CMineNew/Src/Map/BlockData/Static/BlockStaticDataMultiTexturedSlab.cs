using CMineNew.Geometry;

namespace CMineNew.Map.BlockData.Static{
    public class BlockStaticDataMultiTexturedSlab : BlockStaticDataSlab{
        private readonly int[] _textureIndices;

        public BlockStaticDataMultiTexturedSlab(string id, bool passable, bool lightSource, sbyte lightSourceLight,
            sbyte blockLightPassReduction, sbyte sunlightPassReduction, string[] texture)
            : base(id, passable, lightSource, lightSourceLight, blockLightPassReduction, sunlightPassReduction) {
            _textureIndices = new int[texture.Length];
            var indices = CMine.TextureMap.Indices;
            for (var i = 0; i < texture.Length; i++) {
                _textureIndices[i] = indices[texture[i]];
            }
        }

        public BlockStaticDataMultiTexturedSlab(string id, bool passable, bool lightSource, sbyte lightSourceLight,
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