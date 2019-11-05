using CMineNew.Geometry;
using CMineNew.Map.BlockData.Snapshot;

namespace CMineNew.Inventory {
    public class AbstractInventory : IInventory {
        private readonly Vector2i _size;
        private readonly BlockSnapshot[,] _elements;

        public AbstractInventory(Vector2i size) {
            _size = size;
            _elements = new BlockSnapshot[size.X, size.Y];
        }

        public BlockSnapshot this[int x, int y] {
            get => _elements[x, y];
            set => _elements[x, y] = value;
        }

        public Vector2i Size => _size;
    }
}