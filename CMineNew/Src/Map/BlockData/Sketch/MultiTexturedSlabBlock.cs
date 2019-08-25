using System.Collections.Generic;
using CMineNew.Geometry;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Sketch{
    public class MultiTexturedSlabBlock : SlabBlock{
        protected readonly Area2d[] _textureAreas;

        public MultiTexturedSlabBlock(string id, Chunk chunk, Vector3i position, Area2d[] textureAreas,
            Color4 textureFilter, bool upside, bool passable = false)
            : base(id, chunk, position, textureFilter, upside, passable) {
            _textureAreas = textureAreas;
        }

        public MultiTexturedSlabBlock(string id, Chunk chunk, Vector3i position, IReadOnlyList<string> textureAreas,
            Color4 textureFilter, bool upside, bool passable = false)
            : base(id, chunk, position, textureFilter, upside, passable) {
            _textureAreas = new Area2d[textureAreas.Count];
            for (var i = 0; i < textureAreas.Count; i++) {
                _textureAreas[i] = CMine.Textures.Areas[textureAreas[i]];
            }
        }

        public Area2d[] TextureAreas => _textureAreas;

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new MultiTexturedCubicBlock(_id, _chunk, _position, _textureAreas, _textureFilter);
        }

        public override Area2d GetTextureArea(BlockFace face) {
            return _textureAreas[(int) face];
        }
    }
}