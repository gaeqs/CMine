using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

namespace CMineNew.Render{
    public class Room{
        protected readonly string _name;
        protected Color4 _background;

        public Room(string name) {
            _name = name;
            _background = Color4.Black;
        }

        public string Name => _name;

        public Color4 Background {
            get => _background;
            set => _background = value;
        }

        public virtual void Tick(long delay) {
        }

        public virtual void Draw() {
        }

        public virtual void KeyPress(KeyPressEventArgs args) {
        }

        public virtual void KeyPush(KeyboardKeyEventArgs args) {
        }

        public virtual void KeyRelease(KeyboardKeyEventArgs args) {
        }

        public virtual void MousePush(MouseButtonEventArgs args) {
        }

        public virtual void MouseRelease(MouseButtonEventArgs args) {
        }

        public virtual void MouseMove(MouseMoveEventArgs args) {
        }

        public virtual void MouseWheel(MouseWheelEventArgs args) {
        }

        public virtual void MouseEnter(EventArgs args) {
        }

        public virtual void MouseExit(EventArgs args) {
        }

        public virtual void Close() {
        }

        public override string ToString() {
            return _name;
        }
    }
}