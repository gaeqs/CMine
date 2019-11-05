using CMineNew.Geometry;
using CMineNew.Map.BlockData;
using CMineNew.Map.BlockData.Snapshot;

namespace CMineNew.Inventory {
    public interface IInventory {
        BlockSnapshot this[int x, int y] { get; set; }

        Vector2i Size { get; }
    }
}