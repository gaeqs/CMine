using System;
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
    public class CubicBlockModel : BlockModel{
        public const string Key = "default:cubic";

        private static readonly Vector3[] LineVertices = {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 1),
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 1),
            new Vector3(1, 1, 0),
            new Vector3(1, 1, 1),
        };

        private readonly LineVertexArrayObject _lineVao;

        public CubicBlockModel() : base(Key, new Aabb(0, 0, 0, 1, 1, 1)) {
            _lineVao = new LineVertexArrayObject(LineVertices, ModelLinesUtil.CalculateLinesIndices(LineVertices));
            _vertexArrayObject = BlockFaceVertices.CreateVao(BlockFace.South);
            _shader = ShaderManager.GetOrCreateShader("cubic_block", Shaders.block_vertex, Shaders.block_fragment);

            _shader.Use();
            var matrices = BlockFaceVertices.GenerateRotationMatrices();
            for (var i = 0; i < matrices.Length; i++) {
                _shader.SetUMatrix("rotationMatrices[" + i + "]", matrices[i]);
            }
        }

        public override uint FloatsPerBlock => 60;
        public override uint DefaultVboBlocks => 20000;

        public override BlockRender CreateBlockRender(ChunkRegion chunkRegion) {
            return new CubicBlockRender(chunkRegion);
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
            if (!(block is CubicBlock cubic)) return new float[0];
            var data = new float[FloatsPerBlock];

            var pointer = 0;

            for (var i = 0; i < BlockFaceMethods.All.Length; i++) {
                if (!cubic.IsFaceVisible(i)) {
                    for (var j = 0; j < FloatsPerBlock / 6; j++)
                        data[pointer++] = -1;
                    continue;
                }

                var pos = block.Position;
                var area = cubic.GetTextureArea((BlockFace) i);
                var filter = block.TextureFilter;

                data[pointer++] = pos.X;
                data[pointer++] = pos.Y;
                data[pointer++] = pos.Z;
                data[pointer++] = area.MinX;
                data[pointer++] = area.MinY;
                data[pointer++] = area.MaxX;
                data[pointer++] = area.MaxY;
                data[pointer++] = filter.ValueF;
                data[pointer++] = block.BlockLight.Light / Block.MaxBlockLightF;
                data[pointer++] = block.BlockLight.Sunlight / Block.MaxBlockLightF;
            }

            return data;
        }

        public override void SetupVbo(VertexBufferObject vbo) {
            Console.WriteLine("Cubic VBO: "+vbo.Id);
            vbo.Bind(BufferTarget.ArrayBuffer);

            var builder = new AttributePointerBuilder(_vertexArrayObject, (int) FloatsPerBlock / 6, 3);
            builder.AddPointer(3, true);
            builder.AddPointer(4, true);
            builder.AddPointer(1, true);
            builder.AddPointer(1, true);
            builder.AddPointer(1, true);
        }

        public override void Draw(int amount) {
            _shader.Use();
            GL.Disable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            _vertexArrayObject.Bind();
            _vertexArrayObject.DrawnInstanced(amount * 6);
        }

        public override void DrawAfterPostRender(int amount) {
        }
    }
}