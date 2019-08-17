using System;
using CMineNew.Geometry;

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
}