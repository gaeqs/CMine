using CMineNew.Render;
using OpenTK;

namespace CMineNew{
    public class CMine{
        public const int TicksPerSecond = 10000000;
        public const float TicksPerSecondF = 10000000f;

        public static Window _window;

        public static void Load() {
            _window = new Window(1920 / 2, 1080 / 2, 
                GameWindowFlags.Default, false, (sender, args) => { });
        }
    }
}