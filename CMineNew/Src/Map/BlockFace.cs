using System;
using System.Collections.Generic;
using CMineNew.Geometry;
using CMineNew.Render.Object;
using OpenTK;

namespace CMineNew.Map{
    public enum BlockFace{
        Up = 0,
        Down = 1,
        West = 2,
        East = 3,
        North = 4,
        South = 5
    }

    public static class BlockFaceMethods{
        public static readonly BlockFace[] All =
            {BlockFace.Up, BlockFace.Down, BlockFace.West, BlockFace.East, BlockFace.North, BlockFace.South};

        public static BlockFace GetOpposite(BlockFace face) {
            switch (face) {
                case BlockFace.Up:
                    return BlockFace.Down;
                case BlockFace.Down:
                    return BlockFace.Up;
                case BlockFace.West:
                    return BlockFace.East;
                case BlockFace.East:
                    return BlockFace.West;
                case BlockFace.South:
                    return BlockFace.North;
                case BlockFace.North:
                    return BlockFace.South;
                default:
                    throw new ArgumentOutOfRangeException(nameof(face), face, null);
            }
        }


        public static Vector3i GetRelative(BlockFace face) {
            switch (face) {
                case BlockFace.Up:
                    return new Vector3i(0, 1, 0);
                case BlockFace.Down:
                    return new Vector3i(0, -1, 0);
                case BlockFace.West:
                    return new Vector3i(-1, 0, 0);
                case BlockFace.East:
                    return new Vector3i(1, 0, 0);
                case BlockFace.South:
                    return new Vector3i(0, 0, 1);
                case BlockFace.North:
                    return new Vector3i(0, 0, -1);
                default:
                    throw new ArgumentOutOfRangeException(nameof(face), face, null);
            }
        }
    }

    public static class BlockFaceVertices{
        private static Vector3[] _north = new[] {
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(1, 0, 0), new Vector3(0, 0, -1)
        };

        private static Vector3[] _south = new[] {
            new Vector3(0, 0, 1),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(1, 0, 1), new Vector3(0, 0, 1)
        };

        private static Vector3[] _west = new[] {
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 1),
            new Vector3(0, 0, 1), new Vector3(1, 0, 0)
        };

        private static Vector3[] _east = new[] {
            new Vector3(1, 0, 0),
            new Vector3(1, 1, 0),
            new Vector3(1, 1, 1),
            new Vector3(1, 0, 1), new Vector3(-1, 0, 0)
        };

        private static Vector3[] _up = new[] {
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),
            new Vector3(1, 1, 0), new Vector3(0, 1, 0)
        };

        private static Vector3[] _down = new[] {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 1),
            new Vector3(1, 0, 0),
            new Vector3(0, -1, 0)
        };

        public static VertexArrayObject CreateVao(BlockFace face) {
            switch (face) {
                case BlockFace.Up:
                    return CreateVao(_up);
                case BlockFace.Down:
                    return CreateVao(_down);
                case BlockFace.East:
                    return CreateVao(_east);
                case BlockFace.West:
                    return CreateVao(_west);
                case BlockFace.North:
                    return CreateVao(_north);
                case BlockFace.South:
                    return CreateVao(_south);
                default:
                    throw new ArgumentOutOfRangeException(nameof(face), face, null);
            }
        }

        private static VertexArrayObject CreateVao(IReadOnlyList<Vector3> positions) {
            var vertices = new Vertex[4];
            vertices[0] = new Vertex(positions[0], positions[4], new Vector2(0, 1));
            vertices[1] = new Vertex(positions[1], positions[4], new Vector2(0, 0));
            vertices[2] = new Vertex(positions[2], positions[4], new Vector2(1, 0));
            vertices[3] = new Vertex(positions[3], positions[4], new Vector2(1, 1));
            return new VertexArrayObject(vertices, new[] {0, 1, 3, 1, 2, 3});
        }
    }
}