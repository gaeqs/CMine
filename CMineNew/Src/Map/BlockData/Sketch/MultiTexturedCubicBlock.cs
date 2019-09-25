using System.Collections.Generic;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Static;
using OpenTK.Graphics;

namespace CMineNew.Map.BlockData.Sketch{
    public class MultiTexturedCubicBlock : CubicBlock{
        protected readonly Area2d[] _textureAreas;

        public MultiTexturedCubicBlock(BlockStaticDataMultiTexturedCubic staticData, Chunk chunk, Vector3i position, 
            Area2d[] textureAreas, Color4 textureFilter)
            : base(staticData, chunk, position, textureFilter) {
            _textureAreas = textureAreas;
        }

        public MultiTexturedCubicBlock(BlockStaticDataMultiTexturedCubic staticData, Chunk chunk, Vector3i position, 
            IReadOnlyList<string> textureAreas, Color4 textureFilter)
            : base(staticData, chunk, position, textureFilter) {
            _textureAreas = new Area2d[textureAreas.Count];
            for (var i = 0; i < textureAreas.Count; i++) {
                _textureAreas[i] = CMine.Textures.Areas[textureAreas[i]];
            }
        }

        public Area2d[] TextureAreas => _textureAreas;

        public override Block Clone(Chunk chunk, Vector3i position) {
            return new MultiTexturedCubicBlock((BlockStaticDataMultiTexturedCubic) _staticData, _chunk, _position, _textureAreas, _textureFilter);
        }

        public override Area2d GetTextureArea(BlockFace face) {
            return _textureAreas[(int) face];
        }
    }
}