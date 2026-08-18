using Microsoft.Xna.Framework.Input;
using System;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Owns developer-mode state and dispatches developer-only input commands.
    /// </summary>
    public sealed class DeveloperModeController
    {
        public bool IsEnabled { get; private set; }

        public DeveloperViewSettings View { get; } = new();

        public void UpdateToggle(
            KeyboardState keyboard,
            KeyboardState previousKeyboard)
        {
            if (WasPressed(Keys.F3, keyboard, previousKeyboard))
                IsEnabled = !IsEnabled;
        }

        public void UpdateCommands(
            KeyboardState keyboard,
            KeyboardState previousKeyboard,
            Action playCameraTestCutscene)
        {
            ArgumentNullException.ThrowIfNull(playCameraTestCutscene);

            if (!IsEnabled)
                return;

            if (WasPressed(Keys.F4, keyboard, previousKeyboard))
                View.ShowCollisions = !View.ShowCollisions;

            if (WasPressed(Keys.F5, keyboard, previousKeyboard))
                View.ShowEntityBounds = !View.ShowEntityBounds;

            if (WasPressed(Keys.F6, keyboard, previousKeyboard))
                playCameraTestCutscene();

            if (WasPressed(Keys.F7, keyboard, previousKeyboard))
                View.ShowProjectileBounds = !View.ShowProjectileBounds;

            if (WasPressed(Keys.F8, keyboard, previousKeyboard))
                View.ShowSpawnPoints = !View.ShowSpawnPoints;

            if (WasPressed(Keys.F9, keyboard, previousKeyboard))
                View.ShowHud = !View.ShowHud;
        }

        private static bool WasPressed(
            Keys key,
            KeyboardState keyboard,
            KeyboardState previousKeyboard)
        {
            return keyboard.IsKeyDown(key) && previousKeyboard.IsKeyUp(key);
        }
    }
}
