using System;
using CMineNew.Collision;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Render;
using CMineNew.Render;
using CMineNew.Render.Object;
using CMineNew.Util;
using OpenTK;
using OpenTK.Graphics.ES11;

namespace CMineNew.Map.BlockData.Model{
    public class TorchBlockModel : BlockModel{
        public const string Key = "default:torch";
        
        public TorchBlockModel() : base(Key, new Aabb(0.4f, 0, 0.4f, 0.2f, 0.6f, 0.2f)) {
        }

        public override BlockRender CreateBlockRender(ChunkRegion chunkRegion) {
            return new TorchBlockRender(chunkRegion);
        }
    }
}