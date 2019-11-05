using CMineNew.Geometry;

namespace CMineNew.Inventory {
    public class PlayerInventory : AbstractInventory, IPlayerInventory {
        
        public static readonly Vector2i PlayerInventorySize = new Vector2i(8, 3);

        private readonly InventoryHotbar _hotbar;
        
        public PlayerInventory() : base(PlayerInventorySize) {
            _hotbar = new InventoryHotbar();
        }

        public InventoryHotbar Hotbar { get => _hotbar; }
    }
}