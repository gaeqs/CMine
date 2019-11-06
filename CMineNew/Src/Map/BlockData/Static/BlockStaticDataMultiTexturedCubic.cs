using CMineNew.Geometry;

namespace CMineNew.Map.BlockData.Static {
    public class BlockStaticDataMultiTexturedCubic : BlockStaticDataCubic {
        private readonly Area2d[] _textureAreas;

        public BlockStaticDataMultiTexturedCubic(string id, bool passable, bool lightSource, sbyte lightSourceLight,
            sbyte blockLightPassReduction, sbyte sunlightPassReduction, string[] texture)
            : base(id, passable, lightSource, lightSourceLight, blockLightPassReduction, sunlightPassReduction) {
            _textureAreas = new Area2d[texture.Length];
            var areas = CMine.TextureMap.Areas;
            for (var i = 0; i < texture.Length; i++) {
                _textureAreas[i] = areas[texture[i]];
            }
        }

        public BlockStaticDataMultiTexturedCubic(string id, bool passable, bool lightSource, sbyte lightSourceLight,
            sbyte blockLightPassReduction, sbyte sunlightPassReduction, Area2d[] texture)
            : base(id, passable, lightSource, lightSourceLight, blockLightPassReduction, sunlightPassReduction) {
            _textureAreas = texture;
        }

        public Area2d[] TextureAreas => _textureAreas;

        public override Area2d GetTextureArea(BlockFace face) {
            return _textureAreas[(int) face];
        }
    }
}