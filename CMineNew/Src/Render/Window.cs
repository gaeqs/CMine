using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace CMineNew.Render{
    public class Window : GameWindow{
        private EventHandler _onLoaded;

        private Vector2 _unitsPerPixel;

        private long _lastTick, _delay;

        public Window(int width, int height, GameWindowFlags windowMode, bool vSync, EventHandler onLoaded)
            : base(width, height, GraphicsMode.Default, "CMine", windowMode) {
            _onLoaded = onLoaded;
            _unitsPerPixel = new Vector2(2f / width, 2f / height);
            VSync = vSync ? VSyncMode.On : VSyncMode.Off;
            X += 1920;
        }

        public EventHandler OnLoaded {
            get => _onLoaded;
            set => _onLoaded = value;
        }

        public long LastTick => _lastTick;

        public long Delay => _delay;

        protected override void OnLoad(EventArgs e) {
            Console.WriteLine("OpenGL Version: " + GL.GetString(StringName.Version));
            Console.WriteLine("Graphic Card: " + GL.GetString(StringName.Vendor) + " - " +
                              GL.GetString(StringName.Renderer));

            //Enable static capabilities.
            GL.Enable(EnableCap.Texture2D);
            CursorVisible = false;
            _lastTick = DateTime.Now.Ticks;

            _onLoaded?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
        }
    }
}