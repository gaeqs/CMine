using System.Collections.Generic;
using CMineNew.Geometry;

namespace CMineNew.Map.BlockData.Sketch{
    public class MultiTexturedCubicBlock : CubicBlock{
        protected readonly Area2d[] _textureAreas;

        public MultiTexturedCubicBlock(string id, Chunk chunk, Vector3i position, Area2d[] textureAreas, bool passable = false)
            : base(id, chunk, position, passable) {
            _textureAreas = textureAreas;
        }

        public MultiTexturedCubicBlock(string id, Chunk chunk, Vector3i position, IReadOnlyList<string> textureAreas, bool passable = false)
            : base(id, chunk, position, passable) {
            _textureAreas = new Area2d[textureAreas.Count];
            for (var i = 0; i < textureAreas.Count; i++) {
                _textureAreas[i] = CMine.Textures.Areas[textureAreas[i]];
            }
        }

        public Area2d[] TextureAreas => _textureAreas;

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new MultiTexturedCubicBlock(_id, _chunk, _position, _textureAreas);
        }

        public override Area2d GetTextureArea(BlockFace face) {
            return _textureAreas[(int) face];
        }
    }
}