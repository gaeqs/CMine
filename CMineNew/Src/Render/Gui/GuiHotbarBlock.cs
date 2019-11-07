using CMineNew.Inventory;
using CMineNew.Map;
using CMineNew.Render.Object;
using OpenTK;

namespace CMineNew.Render.Gui{
    public class GuiHotbarBlock : GuiBlockElement{
        private readonly InventoryHotbar _hotbar;
        private readonly Gui2dElement _background;
        private readonly int _index;
        private bool _selected;

        public GuiHotbarBlock(int index, InventoryHotbar hotbar, Gui2dElement background)
            : base("hotbar_" + index, Vector2.Zero, Vector3.Zero, null, AspectRatioMode.Modify) {
            _hotbar = hotbar;
            _background = background;
            _index = index;
            _position = background.Position + new Vector2(background.Size.X * (index) / 5, 0.05f);
            Snapshot = _hotbar[index, 0];
            _selected = false;
        }

        public InventoryHotbar Hotbar => _hotbar;

        public int Index => _index;

        public override void Draw(World world, ShaderProgram shader, VertexArrayObject vao) {
            Snapshot = _hotbar[_index, 0];

            if (_hotbar.Selected == _index) {
                if (!_selected) {
                    _selected = true;
                    _position.Y += 0.05f;
                }
            }
            else {
                if (_selected) {
                    _selected = false;
                    _position.Y -= 0.05f;
                }
            }

            var ratio = _background.Size.X * 1.2f / 5f;
            Size = new Vector3(ratio);

            base.Draw(world, shader, vao);
        }
    }
}