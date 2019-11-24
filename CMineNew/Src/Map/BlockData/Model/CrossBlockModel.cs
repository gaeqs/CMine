using CMineNew.Collision;
using CMineNew.Map.BlockData.Render;
using CMineNew.Map.BlockData.Sketch;
using CMineNew.Render;
using CMineNew.Render.Object;
using CMineNew.Resources.Shaders;
using CMineNew.Util;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Map.BlockData.Model{
    public class CrossBlockModel : BlockModel{
        public const string Key = "default:cross";

        public static readonly Vector3[] LineVertices = {
            new Vector3(0.1f, 0, 0.1f),
            new Vector3(0.1f, 0, 0.9f),
            new Vector3(0.1f, 0.7f, 0.1f),
            new Vector3(0.1f, 0.7f, 0.9f),
            new Vector3(0.9f, 0, 0.1f),
            new Vector3(0.9f, 0, 0.9f),
            new Vector3(0.9f, 0.7f, 0.1f),
            new Vector3(0.9f, 0.7f, 0.9f),
        };

        public static readonly Vertex[] Vertices = {
            new Vertex(0.1f, 0, 0.1f, -1, -1, -1, 0, 1),
            new Vertex(0.1f, 0, 0.9f, -1, -1, 1, 0, 1),
            new Vertex(0.1f, 1f, 0.1f, -1, 1, -1, 0, 0),
            new Vertex(0.1f, 1f, 0.9f, -1, 1, 1, 0, 0),
            new Vertex(0.9f, 0, 0.1f, 1, -1, -1, 1, 1),
            new Vertex(0.9f, 0, 0.9f, 1, -1, 1, 1, 1),
            new Vertex(0.9f, 1f, 0.1f, 1, 1, -1, 1, 0),
            new Vertex(0.9f, 1f, 0.9f, 1, 1, 1, 1, 0),
        };

        private static readonly int[] Indices = {0, 5, 7, 0, 7, 2, 1, 4, 6, 1, 6, 3};

        private readonly LineVertexArrayObject _lineVao;

        public CrossBlockModel() : base(Key, new Aabb(0.1f, 0, 0.1f, 0.8f, 0.7f, 0.8f)) {
            _lineVao = new LineVertexArrayObject(LineVertices, ModelLinesUtil.CalculateLinesIndices(LineVertices));
            _vertexArrayObject = new VertexArrayObject(Vertices, Indices);
            _shader = ShaderManager.GetOrCreateShader("cross_block", Shaders.cross_block_vertex, Shaders.cross_block_fragment);
        }

        public override uint FloatsPerBlock => 10;
        public override uint DefaultVboBlocks => 1000;

        public override BlockRender CreateBlockRender(ChunkRegion chunkRegion) {
            return new CrossBlockRender(chunkRegion);
        }

        public override void DrawLines(Camera camera, Block block) {
            _lineVao.Bind();
            BlockLinesShaderProgram.Use();
            BlockLinesShaderProgram.SetUMatrix("viewProjection", camera.ViewProjection);
            BlockLinesShaderProgram.SetUVector("worldPosition", block.Position);
            GL.LineWidth(2);
            _lineVao.Draw();
        }

        public override float[] GetData(Block block) {
            if (!(block is CrossBlock)) return new float[0];
            var pos = block.Position;
            var area = block.StaticData.GetTextureArea(BlockFace.Up);
            var filter = block.TextureFilter;
            var blockLight = block.BlockLight.Light;
            var sunlight = block.BlockLight.Sunlight;
            return new[] {
                pos.X, pos.Y, pos.Z, area.MinX, area.MinY, area.MaxX, area.MaxY,
                filter.ValueF, blockLight / Block.MaxBlockLightF, sunlight / Block.MaxBlockLightF
            };
        }

        public override void SetupVbo(VertexBufferObject vbo) {
            vbo.Bind(BufferTarget.ArrayBuffer);

            var builder = new AttributePointerBuilder(_vertexArrayObject, (int) FloatsPerBlock, 3);
            builder.AddPointer(3, true);
            builder.AddPointer(4, true);
            builder.AddPointer(1, true);
            builder.AddPointer(1, true);
            builder.AddPointer(1, true);
        }

        public override void Draw(int amount) {
            //_shader.Use();
            //GL.Disable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.FrontAndBack);
            //_vertexArrayObject.Bind();
            //_vertexArrayObject.DrawnInstanced(amount);
        }

        public override void DrawAfterPostRender(int amount) {
        }
    }
}