using System.Collections.ObjectModel;
using CMine.Collision;
using CMineNew.Geometry;
using CMineNew.Map.Render;
using CMineNew.Render;
using CMineNew.Render.Object;
using CMineNew.Resources.Shaders;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Map.Model{
    public abstract class BlockModel{
        public static readonly ShaderProgram BlockLinesShaderProgram = new ShaderProgram(
            new Collection<string> {Shaders.block_lines_vertex, Shaders.block_fragment},
            new Collection<ShaderType> {ShaderType.VertexShader, ShaderType.FragmentShader});

        private readonly Aabb _blockCollision;

        public BlockModel(Aabb blockCollision) {
            _blockCollision = blockCollision;
        }

        public Aabb BlockCollision => _blockCollision;

        public abstract BlockRender CreateBlockRender();

        public abstract void DrawLines(Camera camera, Vector3i blockPosition);
    }
}