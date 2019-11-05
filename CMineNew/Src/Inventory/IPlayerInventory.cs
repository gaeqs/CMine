namespace CMineNew.Inventory {
    public interface IPlayerInventory : IInventory {
        InventoryHotbar Hotbar { get; }
    }
}