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
        private readonly Stack<string> navigationHistory = new();

        public IGameScreen CurrentScreen { get; private set; }

        public bool CanGoBack => navigationHistory.Count > 0;

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
            Activate(screenId);
        }

        public void SetRoot(string screenId)
        {
            navigationHistory.Clear();
            Activate(screenId);
        }

        public void NavigateTo(string screenId)
        {
            if (CurrentScreen != null &&
                !string.Equals(CurrentScreen.Id, screenId, StringComparison.OrdinalIgnoreCase))
            {
                navigationHistory.Push(CurrentScreen.Id);
            }

            Activate(screenId);
        }

        public bool TryGetPreviousScreenId(out string screenId)
        {
            return navigationHistory.TryPeek(out screenId);
        }

        public bool GoBack()
        {
            if (!navigationHistory.TryPop(out string previousScreenId))
                return false;

            Activate(previousScreenId);
            return true;
        }

        public void Deactivate(bool clearHistory = false)
        {
            CurrentScreen?.Exit();
            CurrentScreen = null;

            if (clearHistory)
                navigationHistory.Clear();
        }

        private void Activate(string screenId)
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
