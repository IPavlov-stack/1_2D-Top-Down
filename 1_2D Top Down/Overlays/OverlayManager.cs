using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    public sealed class OverlayManager
    {
        private readonly List<IGameOverlay> overlays = new();

        public bool BlocksInputBelow => overlays.Count > 0 && overlays[^1].BlocksInputBelow;
        public bool BlocksUpdateBelow => overlays.Count > 0 && overlays[^1].BlocksUpdateBelow;
        public bool HasOverlays => overlays.Count > 0;

        public bool Contains(string id) => overlays.Exists(overlay => overlay.Id == id);

        public void Push(IGameOverlay overlay)
        {
            if (Contains(overlay.Id))
                return;

            overlays.Add(overlay);
            overlay.Enter();
        }

        public bool Remove(string id)
        {
            int index = overlays.FindLastIndex(overlay => overlay.Id == id);
            if (index < 0)
                return false;

            IGameOverlay overlay = overlays[index];
            overlays.RemoveAt(index);
            overlay.Exit();
            return true;
        }

        public void Clear()
        {
            for (int index = overlays.Count - 1; index >= 0; index--)
                overlays[index].Exit();

            overlays.Clear();
        }

        public void Update(GameTime gameTime)
        {
            if (overlays.Count > 0)
                overlays[^1].Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (IGameOverlay overlay in overlays)
                overlay.Draw(gameTime, spriteBatch);
        }
    }
}
