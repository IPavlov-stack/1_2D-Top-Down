using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    public sealed class ScreenManager
    {
        private readonly Dictionary<string, IGameScreen> screens =
            new(StringComparer.OrdinalIgnoreCase);

        public IGameScreen CurrentScreen { get; private set; }

        public void Register(IGameScreen screen)
        {
            ArgumentNullException.ThrowIfNull(screen);

            if (string.IsNullOrWhiteSpace(screen.Id))
                throw new ArgumentException("A screen must have a non-empty ID.", nameof(screen));

            if (!screens.TryAdd(screen.Id, screen))
                throw new InvalidOperationException($"A screen with ID '{screen.Id}' is already registered.");
        }

        public bool Contains(string screenId)
        {
            return screens.ContainsKey(screenId);
        }

        public void ChangeScreen(string screenId)
        {
            if (!screens.TryGetValue(screenId, out IGameScreen nextScreen))
                throw new KeyNotFoundException($"No screen with ID '{screenId}' is registered.");

            if (ReferenceEquals(CurrentScreen, nextScreen))
                return;

            CurrentScreen?.Exit();
            CurrentScreen = nextScreen;
            CurrentScreen.Enter();
        }

        public void Update(GameTime gameTime)
        {
            CurrentScreen?.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            CurrentScreen?.Draw(gameTime, spriteBatch);
        }
    }
}
