using Microsoft.Xna.Framework.Input;

namespace _1_2D_Top_Down
{
    public readonly struct GameplayUiInput
    {
        public GameplayUiInput(
            KeyboardState keyboard,
            KeyboardState previousKeyboard,
            MouseState mouse,
            MouseState previousMouse)
        {
            Keyboard = keyboard;
            PreviousKeyboard = previousKeyboard;
            Mouse = mouse;
            PreviousMouse = previousMouse;
        }

        public KeyboardState Keyboard { get; }
        public KeyboardState PreviousKeyboard { get; }
        public MouseState Mouse { get; }
        public MouseState PreviousMouse { get; }
    }
}
