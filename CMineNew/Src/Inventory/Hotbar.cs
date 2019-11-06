using CMineNew.Geometry;
using CMineNew.Map.BlockData.Snapshot;
using CMineNew.Util;

namespace CMineNew.Inventory {
    public class InventoryHotbar : AbstractInventory {
        public static readonly Vector2i HotbarSize = new Vector2i(5, 1);

        private int _selected;

        public InventoryHotbar() : base(HotbarSize) {
            _selected = 0;
        }

        public int Selected {
            get => _selected;
            set => _selected = MathUtils.FloorMod(value, HotbarSize.X);
        }


        public BlockSnapshot SelectedBlock {
            get => this[_selected, 0];
            set => this[_selected, 0] = value;
        }
    }
}