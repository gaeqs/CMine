using OpenTK.Input;

namespace CMineNew.Entities.Controller{
    
    /// <summary>
    /// A PlayerController manages the movement of a player.
    /// Currently the only implementation is LocalPlayerController, which translates
    /// mouse and keyboard inputs into player's actions.
    /// </summary>
    public abstract class PlayerController{
        private readonly Player _player;

        /// <summary>
        /// Creates a player controller.
        /// </summary>
        /// <param name="player">The player.</param>
        public PlayerController(Player player) {
            _player = player;
        }

        public Player Player => _player;

        /// <summary>
        /// Method called every game loop.
        /// </summary>
        /// <param name="dif">The delay between the last tick and the current one in ticks.</param>
        public abstract void Tick(long dif);

        /// <summary>
        /// Method called when a key is pushed.
        /// </summary>
        /// <param name="eventArgs">The event.</param>
        public abstract void HandleKeyPush(KeyboardKeyEventArgs eventArgs);

        /// <summary>
        /// Method called when a key is released.
        /// </summary>
        /// <param name="eventArgs">The event.</param>
        public abstract void HandleKeyRelease(KeyboardKeyEventArgs eventArgs);

        /// <summary>
        /// Method called when a mouse button is pushed.
        /// </summary>
        /// <param name="eventArgs">The event.</param>
        public abstract void HandleMousePush(MouseButtonEventArgs eventArgs);

        /// <summary>
        /// Method called when a mouse button is released.
        /// </summary>
        /// <param name="eventArgs">The event.</param>
        public abstract void HandleMouseRelease(MouseButtonEventArgs eventArgs);
    }
}