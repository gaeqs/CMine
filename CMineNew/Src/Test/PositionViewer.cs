using CMineNew.Map;
using CMineNew.Render;
using CMineNew.Text;
using OpenTK;
using OpenTK.Graphics;

namespace CMineNew.Test{
    public class PositionViewer : StaticText{
        public PositionViewer(TrueTypeFont font) : base(font, "INIT", new Vector2(-1, -1f), Color4.White) {
        }

        public override void Tick(long dif, Room room) {
            if (room is World dRoom) {
                Text = dRoom.Player.Position + " [" + dRoom.Camera.Rotation + "]";
            }
        }
    }
}