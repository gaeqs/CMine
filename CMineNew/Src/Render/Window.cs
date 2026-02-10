using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using CMineNew.Render.Object;
using CMineNew.Test;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace CMineNew.Render
{
    public class Window : GameWindow
    {
        private EventHandler _onLoaded;

        private Room _room;
        private Vector2 _unitsPerPixel;
        private long _delay;
        private bool _vsync;

        private Stopwatch _stopwatch, _loopStopwatch;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings,
            EventHandler onLoaded)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            _onLoaded = onLoaded;
            _unitsPerPixel = new Vector2(2f / nativeWindowSettings.ClientSize.X,
                2f / nativeWindowSettings.ClientSize.Y);
            VSync = nativeWindowSettings.Vsync == VSyncMode.On ? VSyncMode.On : VSyncMode.Off;
            CursorState = CursorState.Grabbed;

            //X += 1920;

            _room = null;
            _stopwatch = new Stopwatch();
            _loopStopwatch = new Stopwatch();
            _vsync = nativeWindowSettings.Vsync == VSyncMode.On;
        }

        public EventHandler OnLoaded
        {
            get => _onLoaded;
            set => _onLoaded = value;
        }

        public Room Room
        {
            get => _room;
            set
            {
                _room?.Close();
                _room = value;
            }
        }

        public long Delay => _delay;

        public Vector2 UnitsPerPixel => _unitsPerPixel;

        protected override void OnLoad()
        {
            var thread = Thread.CurrentThread;
            thread.Priority = ThreadPriority.Normal;
            thread.Name = "Render thread";
            Console.WriteLine("OpenGL Version: " + GL.GetString(StringName.Version));
            Console.WriteLine("Graphic Card: " + GL.GetString(StringName.Vendor) + " - " +
                              GL.GetString(StringName.Renderer));

            //Enable static capabilities.
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.ProgramPointSize);
            GL.Enable(EnableCap.CullFace);
            //TODO CursorVisible = false;

            //GL.Enable(EnableCap.DebugOutput);
            //GL.DebugMessageCallback(Debug, IntPtr.Zero);

            _onLoaded?.Invoke(this, EventArgs.Empty);
            _loopStopwatch.Start();
        }

        private void Debug(DebugSource source, DebugType type, int id, DebugSeverity severity,
            int length, IntPtr message, IntPtr userParam)
        {
            if (type != DebugType.DebugTypeError) return;
            unsafe
            {
                Console.WriteLine("--- DEBUG ---");
                Console.WriteLine(new string((sbyte*)message.ToPointer(), 0, length, Encoding.ASCII));
                Console.WriteLine("Source: " + source);
                Console.WriteLine("Type: " + type);
                Console.WriteLine("Id: " + id);
                Console.WriteLine("Severity: " + severity);
                Console.WriteLine("-------------");
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            _loopStopwatch.Stop();
            //Blue
            LoopDelayViewer.Add(_loopStopwatch.ElapsedTicks);
            _delay = _vsync ? CMine.TicksPerSecond / 60 : _loopStopwatch.ElapsedTicks;
            _delay = Math.Min(CMine.TicksPerSecond / 30, _delay);
            _loopStopwatch.Restart();
            if (_room != null)
            {
                _stopwatch.Restart();
                _room.Tick(_delay);
                _stopwatch.Stop();
                //Green
                LoopDelayViewer.Add(_stopwatch.ElapsedTicks);
                _stopwatch.Restart();
                _room.Draw(_delay);
                _stopwatch.Stop();
                //Red
                LoopDelayViewer.Add(_stopwatch.ElapsedTicks);
                //LoopDelayViewer.Draw();
            }

            Context.SwapBuffers();
        }

        #region Room events

        protected override void OnTextInput(TextInputEventArgs args)
        {
            base.OnTextInput(args);
            _room?.KeyPress(args);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs args)
        {
            base.OnKeyDown(args);
            _room?.KeyPush(args);
            if (args.Key == Keys.Escape)
            {
                Close();
            }
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs args)
        {
            base.OnKeyUp(args);
            _room?.KeyRelease(args);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            _room?.MousePush(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            _room?.MouseRelease(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            _room?.MouseMove(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            _room?.MouseWheel(e);
        }

        protected override void OnMouseEnter()
        {
            base.OnMouseEnter();
            _room?.MouseEnter();
        }

        protected override void OnMouseLeave()
        {
            base.OnMouseLeave();
            _room?.MouseExit();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _room.Close();
            ShaderManager.CleanUp();
            CMine.TextureManager.CleanUp();
        }

        #endregion
    }
}