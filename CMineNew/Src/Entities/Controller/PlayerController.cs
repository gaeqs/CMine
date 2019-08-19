using OpenTK.Input;

namespace CMineNew.Entities.Controller{
    public abstract class PlayerController{
        private readonly Player _player;

        public PlayerController(Player player) {
            _player = player;
        }

        public Player Player => _player;

        public abstract void Tick(long dif);

        public abstract void HandleKeyPush(KeyboardKeyEventArgs eventArgs);

        public abstract void HandleKeyRelease(KeyboardKeyEventArgs eventArgs);

        public abstract void HandleMousePush(MouseButtonEventArgs args);

        public abstract void HandleMouseRelease(MouseButtonEventArgs args);

        public abstract void HandleMouseMove(MouseMoveEventArgs args);
    }
}