using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    public sealed class GameplayPanelManager
    {
        private readonly Dictionary<string, IGameplayPanel> panels =
            new(StringComparer.OrdinalIgnoreCase);

        public IGameplayPanel ActivePanel { get; private set; }
        public bool HasOpenPanel => ActivePanel != null;
        public bool BlocksGameplayInput =>
            ActivePanel?.BlocksGameplayInput ?? false;

        public void Register(IGameplayPanel panel)
        {
            ArgumentNullException.ThrowIfNull(panel);

            if (!panels.TryAdd(panel.Id, panel))
            {
                throw new InvalidOperationException(
                    $"A gameplay panel with ID '{panel.Id}' is already registered.");
            }
        }

        public bool IsOpen(string panelId)
        {
            return ActivePanel != null &&
                   string.Equals(
                       ActivePanel.Id,
                       panelId,
                       StringComparison.OrdinalIgnoreCase);
        }

        public void Open(string panelId)
        {
            if (!panels.TryGetValue(panelId, out IGameplayPanel panel))
                throw new KeyNotFoundException($"Unknown gameplay panel '{panelId}'.");

            if (ReferenceEquals(ActivePanel, panel))
                return;

            ActivePanel?.Close();
            ActivePanel = panel;
            ActivePanel.Open();
        }

        public void Toggle(string panelId)
        {
            if (IsOpen(panelId))
                CloseActive();
            else
                Open(panelId);
        }

        public bool Close(string panelId)
        {
            if (!IsOpen(panelId))
                return false;

            return CloseActive();
        }

        public bool CloseActive()
        {
            if (ActivePanel == null)
                return false;

            ActivePanel.Close();
            ActivePanel = null;
            return true;
        }

        public bool HandleInput(GameplayUiInput input)
        {
            return ActivePanel?.HandleInput(input) ?? false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            ActivePanel?.Draw(spriteBatch);
        }
    }
}
