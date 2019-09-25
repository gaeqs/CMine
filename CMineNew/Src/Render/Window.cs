using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using CMineNew.Render.Object;
using CMineNew.Test;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace CMineNew.Render{
    public class Window : GameWindow{
        private EventHandler _onLoaded;

        private Room _room;
        private Vector2 _unitsPerPixel;
        private long _lastTick, _delay;

        private Stopwatch _stopwatch;

        public Window(int width, int height, GameWindowFlags windowMode, bool vSync, EventHandler onLoaded)
            : base(width, height, GraphicsMode.Default, "CMine", windowMode) {
            _onLoaded = onLoaded;
            _unitsPerPixel = new Vector2(2f / width, 2f / height);
            VSync = vSync ? VSyncMode.On : VSyncMode.Off;

            //X += 1920;

            _room = null;
            _stopwatch = new Stopwatch();
        }

        public EventHandler OnLoaded {
            get => _onLoaded;
            set => _onLoaded = value;
        }

        public Room Room {
            get => _room;
            set {
                _room?.Close();
                _room = value;
            }
        }

        public long LastTick => _lastTick;

        public long Delay => _delay;

        public Vector2 UnitsPerPixel => _unitsPerPixel;

        protected override void OnLoad(EventArgs e) {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            Thread.CurrentThread.Name = "Render thread";
            Console.WriteLine("OpenGL Version: " + GL.GetString(StringName.Version));
            Console.WriteLine("Graphic Card: " + GL.GetString(StringName.Vendor) + " - " +
                              GL.GetString(StringName.Renderer));

            //Enable static capabilities.
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.ProgramPointSize);
            CursorVisible = false;
            _lastTick = DateTime.Now.Ticks;


            //GL.Enable(EnableCap.DebugOutput);
            //GL.DebugMessageCallback(Debug, IntPtr.Zero);

            _onLoaded?.Invoke(this, EventArgs.Empty);
        }

        private void Debug(DebugSource source, DebugType type, int id, DebugSeverity severity,
            int length, IntPtr message, IntPtr userParam) {
            if(type != DebugType.DebugTypeError) return;
            unsafe {
                Console.WriteLine("--- DEBUG ---");
                Console.WriteLine(new string((sbyte*) message.ToPointer(), 0, length, Encoding.ASCII));
                Console.WriteLine("Source: " + source);
                Console.WriteLine("Type: " + type);
                Console.WriteLine("Id: " + id);
                Console.WriteLine("Severity: " + severity);
                Console.WriteLine("-------------");
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            var now = DateTime.Now.Ticks;
            _stopwatch.Restart();
            _delay = Math.Min(now - _lastTick, CMine.TicksPerSecond / 30);
            _lastTick = now;

            if (_room != null) {
                _room.Tick(_delay);
                _room.Draw();
                _stopwatch.Stop();
                LoopDelayViewer.Add(_stopwatch.ElapsedTicks);
                LoopDelayViewer.Draw();
            }

            SwapBuffers();
        }

        #region Room events

        protected override void OnKeyPress(KeyPressEventArgs args) {
            base.OnKeyPress(args);
            _room?.KeyPress(args);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs args) {
            base.OnKeyDown(args);
            _room?.KeyPush(args);
            if (args.Key == Key.Escape)
                Exit();
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs args) {
            base.OnKeyUp(args);
            _room?.KeyRelease(args);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e) {
            base.OnMouseUp(e);
            _room?.MousePush(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e) {
            base.OnMouseDown(e);
            _room?.MouseRelease(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e) {
            base.OnMouseMove(e);
            _room?.MouseMove(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e) {
            base.OnMouseWheel(e);
            _room?.MouseWheel(e);
        }

        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);
            _room?.MouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            _room?.MouseExit(e);
        }

        protected override void OnClosing(CancelEventArgs e) {
            _room.Close();
            ShaderManager.CleanUp();
        }

        #endregion
    }
}