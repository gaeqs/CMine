using System;
using System.Collections.Generic;
using CMineNew.Geometry;
using CMineNew.Render.Object;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Map {
    public enum BlockFace {
        Up = 0,
        Down = 1,
        West = 2,
        East = 3,
        North = 4,
        South = 5
    }

    public static class BlockFaceMethods {
        public static readonly BlockFace[] All =
            {BlockFace.Up, BlockFace.Down, BlockFace.West, BlockFace.East, BlockFace.North, BlockFace.South};

        private static readonly BlockFace[] Opposites =
            {BlockFace.Down, BlockFace.Up, BlockFace.East, BlockFace.West, BlockFace.South, BlockFace.North};

        private static readonly Vector3i[] Relatives = {
            new Vector3i(0, 1, 0), new Vector3i(0, -1, 0),
            new Vector3i(-1, 0, 0), new Vector3i(1, 0, 0),
            new Vector3i(0, 0, -1), new Vector3i(0, 0, 1),
        };

        public static BlockFace GetOpposite(BlockFace face) {
            return Opposites[(int) face];
        }


        public static Vector3i GetRelative(BlockFace face) {
            return Relatives[(int) face];
        }

        public static bool IsSide(BlockFace face) {
            return face != BlockFace.Down && face != BlockFace.Up;
        }
    }

    public static class BlockFaceVertices {
        private static Vector3[] _north = {
            new Vector3(1, 0, 0),
            new Vector3(0, 0, 0),
            new Vector3(1, 1, 0),
            new Vector3(0, 1, 0), new Vector3(0, 0, -1)
        };

        private static Vector3[] _south = {
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 1),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1), new Vector3(0, 0, 1)
        };

        private static Vector3[] _west = {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 1), new Vector3(1, 0, 0)
        };

        private static Vector3[] _east = {
            new Vector3(1, 0, 1),
            new Vector3(1, 0, 0),
            new Vector3(1, 1, 1),
            new Vector3(1, 1, 0), new Vector3(-1, 0, 0)
        };

        private static Vector3[] _up = {
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0), new Vector3(0, 1, 0)
        };

        private static Vector3[] _down = {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 1), new Vector3(0, -1, 0)
        };

        public static VertexArrayObject CreateVao(BlockFace face, float yHeight = 1) {
            switch (face) {
                case BlockFace.Up:
                    return CreateVao(_up, yHeight, 1);
                case BlockFace.Down:
                    return CreateVao(_down, yHeight, 1);
                case BlockFace.East:
                    return CreateVao(_east, yHeight, yHeight);
                case BlockFace.West:
                    return CreateVao(_west, yHeight, yHeight);
                case BlockFace.North:
                    return CreateVao(_north, yHeight, yHeight);
                case BlockFace.South:
                    return CreateVao(_south, yHeight, yHeight);
                default:
                    throw new ArgumentOutOfRangeException(nameof(face), face, null);
            }
        }

        public static VertexArrayObject CreateCubeVao() {
            var indices = new int[6 * 6];
            var vertices = new Vertex[4 * 6];
            var indicesOffset = 0;
            var indicesDataOffset = 0;
            var verticesOffset = 0;

            for (var i = 0; i < 6; i++) {
                var faceVertices = GetVertices((BlockFace) i);
                vertices[verticesOffset++] = new Vertex(faceVertices[0], faceVertices[4], new Vector2(0, 1));
                vertices[verticesOffset++] = new Vertex(faceVertices[1], faceVertices[4], new Vector2(1, 1));
                vertices[verticesOffset++] = new Vertex(faceVertices[2], faceVertices[4], new Vector2(0, 0));
                vertices[verticesOffset++] = new Vertex(faceVertices[3], faceVertices[4], new Vector2(1, 0));

                indices[indicesOffset++] = indicesDataOffset;
                indices[indicesOffset++] = 1 + indicesDataOffset;
                indices[indicesOffset++] = 3 + indicesDataOffset;
                indices[indicesOffset++] = indicesDataOffset;
                indices[indicesOffset++] = 3 + indicesDataOffset;
                indices[indicesOffset++] = 2 + indicesDataOffset;
                indicesDataOffset += 4;
            }

            var vao =  new VertexArrayObject(vertices, indices);
            //vao.Bind();
            //var vboData = new VertexBufferObject();
            //vao.LinkBuffer(vboData);
            //vboData.Bind(BufferTarget.ArrayBuffer);
//
            //vboData.SetData(BufferTarget.ArrayBuffer, new[] {
            //    0, 0, 0, 0, 
            //    1, 1, 1, 1,  
            //    2, 2, 2, 2, 
            //    3, 3, 3, 3, 
            //    4, 4, 4, 4,
            //    5, 5, 5, 5}, BufferUsageHint.StaticDraw);
            //var builder = new AttributePointerBuilder(vao, 1, 3);
            //builder.AddPointer(1, false);
            //VertexBufferObject.Unbind(BufferTarget.ArrayBuffer);
            //VertexArrayObject.Unbind(); 

            return vao;
        }

        private static Vector3[] GetVertices(BlockFace face) {
            return face switch {
                BlockFace.Up => _up,
                BlockFace.Down => _down,
                BlockFace.East => _east,
                BlockFace.West => _west,
                BlockFace.North => _north,
                BlockFace.South => _south,
                _ => throw new ArgumentOutOfRangeException(nameof(face), face, null)
            };
        }

        private static VertexArrayObject CreateVao(IReadOnlyList<Vector3> positions, float yHeight,
            float textureYHeight) {
            var vertices = new Vertex[4];
            var mul = new Vector3(1, yHeight, 1);
            vertices[0] = new Vertex(positions[0] * mul, positions[4], new Vector2(0, 1 * textureYHeight));
            vertices[1] = new Vertex(positions[1] * mul, positions[4], new Vector2(1, 1 * textureYHeight));
            vertices[2] = new Vertex(positions[2] * mul, positions[4], new Vector2(0, 0));
            vertices[3] = new Vertex(positions[3] * mul, positions[4], new Vector2(1, 0));
            return new VertexArrayObject(vertices, new[] {0, 1, 3, 0, 3, 2});
        }
    }
}