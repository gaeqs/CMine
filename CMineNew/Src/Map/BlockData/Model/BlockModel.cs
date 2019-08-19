using System.Collections.ObjectModel;
using CMine.Collision;
using CMineNew.Geometry;
using CMineNew.Map.BlockData.Render;
using CMineNew.Render;
using CMineNew.Render.Object;
using CMineNew.Resources.Shaders;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Map.BlockData.Model{
    public abstract class BlockModel{
        public static readonly ShaderProgram BlockLinesShaderProgram =
            new ShaderProgram(Shaders.block_lines_vertex, Shaders.block_lines_fragment);

        private string _id;
        private readonly Aabb _blockCollision;

        public BlockModel(string id, Aabb blockCollision) {
            _id = id;
            _blockCollision = blockCollision;
        }

        public Aabb BlockCollision => _blockCollision;

        public string Id => _id;

        public abstract BlockRender CreateBlockRender(ChunkRegion chunkRegion);

        public abstract void DrawLines(Camera camera, Vector3i blockPosition);
    }
}