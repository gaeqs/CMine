using CMineNew.Geometry;

namespace CMineNew.Inventory {
    public class InventoryHotbar : AbstractInventory {
        public static readonly Vector2i HotbarSize = new Vector2i(5, 1);

        public InventoryHotbar() : base(HotbarSize) {
        }
    }
}