using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        private readonly GameplayPanelManager gameplayPanelManager = new();
        private HotbarHud hotbarHud;
        private QuickMenuHud quickMenuHud;

        private const float HealthMeterScale = 0.55f;
        private const float ManaMeterScale = 0.55f;
        // =================
        // = X:  + right,  =
        // =     - left    =
        // = Y:  + down,   =
        // =     - up      =
        // =================
        private static readonly Vector2 HealthMeterOffsetFromBottomCenter = new Vector2(-200f, -175);
        private static readonly Vector2 ManaMeterOffsetFromBottomCenter = new Vector2(200f, -175f);

        // Позиция на филл текстурата вътре в рамката
        private static readonly Vector2 HealthFillOffset = new Vector2(85f, 44f);
        private static readonly Vector2 ManaFillOffset =  new Vector2(85f, 44f);

        // bottom HUD panel
        private const float BottomHudPanelScale = 0.55f;
        private static readonly Vector2 BottomHudPanelOffsetFromBottomCenter = new Vector2(0f, -280f);

        private void InitializeGameplayPanels()
        {
            gameplayPanelManager.Register(
                new InventoryPanel(
                    () => GraphicsDevice.Viewport.Bounds,
                    inventoryResources,
                    panel9SliceTexture,
                    inventorySlotTexture,
                    pixelTexture,
                    boldpixels));

            gameplayPanelManager.Register(
                new StatsPanel(
                    () => GraphicsDevice.Viewport.Bounds,
                    CreateStatsSections,
                    panel9SliceTexture,
                    pixelTexture,
                    boldpixels));

            gameplayPanelManager.Register(
                new QuestLogPanel(
                    () => GraphicsDevice.Viewport.Bounds,
                    questPanelTexture,
                    boldpixels));

            gameplayPanelManager.Register(
                new ShopPanel(
                    () => GraphicsDevice.Viewport.Bounds,
                    shopItems,
                    () => GetInventoryResourceAmount("coin"),
                    TryPurchaseShopItem,
                    panel9SliceTexture,
                    pixelTexture,
                    boldpixels));

            gameplayPanelManager.Register(
                new SoundOptionsPanel(
                    () => GraphicsDevice.Viewport.Bounds,
                    () => MusicVolume,
                    value => MusicVolume = value,
                    () => SoundEffectsVolume,
                    value => SoundEffectsVolume = value,
                    () => gameplayPanelManager.Close(GameplayPanelIds.SoundOptions),
                    panel9SliceTexture,
                    pixelTexture,
                    boldpixels));

            hotbarHud = new HotbarHud(
                () => GraphicsDevice.Viewport.Bounds,
                spellsPanelTexture,
                pixelTexture,
                boldpixels);

            quickMenuHud = new QuickMenuHud(
                () => GraphicsDevice.Viewport.Bounds,
                pixelTexture,
                inventoryButtonTexture,
                statsButtonTexture,
                shopButtonTexture,
                mapButtonTexture,
                skillTreeButtonTexture,
                settingsButtonTexture,
                soundVolumeButtonTexture,
                () => gameplayPanelManager.Toggle(GameplayPanelIds.Inventory),
                () => gameplayPanelManager.Toggle(GameplayPanelIds.Stats),
                () => gameplayPanelManager.Toggle(GameplayPanelIds.Shop),
                () => gameplayPanelManager.Toggle(GameplayPanelIds.SoundOptions));
        }

        private IReadOnlyList<StatSection> CreateStatsSections() =>
            new[]
            {
                new StatSection("RESOURCES", new[]
                {
                    new StatRow("Max Health", player.Health.MaxHealth.ToString()),
                    new StatRow("Health Regen", $"{player.Health.RegenPerSecond:0.##} / sec"),
                    new StatRow("Max Mana", $"{player.Mana.MaxMana:0}"),
                    new StatRow("Mana Regen", $"{player.Mana.RegenPerSecond:0.##} / sec")
                }),
                new StatSection("COMBAT", new[]
                {
                    new StatRow("Damage", player.Stats.Damage.ToString()),
                    new StatRow("Knockback", $"{player.Stats.Knockback:0}"),
                    new StatRow("Projectile Speed", $"{player.Stats.ProjectileSpeed:0}"),
                    new StatRow("Projectile Count", player.Stats.ProjectileCount.ToString())
                }),
                new StatSection("MOVEMENT", new[]
                {
                    new StatRow("Move Speed", $"{player.MoveSpeed:0}")
                })
            };

        private bool HandleGameplayUIInput( KeyboardState keyboard, MouseState mouse)
        {
            GameplayUiInput input = new(
                keyboard,
                previousKeyboard,
                mouse,
                previousMouseState);

            hotbarHud.HandleInput(input);

            if (gameplayPanelManager.IsOpen(GameplayPanelIds.SoundOptions))
                return gameplayPanelManager.HandleInput(input);

            if (keyboard.IsKeyDown(Keys.I) &&
                previousKeyboard.IsKeyUp(Keys.I))
            {
                gameplayPanelManager.Toggle(GameplayPanelIds.Inventory);
            }
            if (keyboard.IsKeyDown(Keys.C) &&
                previousKeyboard.IsKeyUp(Keys.C))
            {
                gameplayPanelManager.Toggle(GameplayPanelIds.Stats);
            }

            if (keyboard.IsKeyDown(Keys.Q) &&
                previousKeyboard.IsKeyUp(Keys.Q))
            {
                gameplayPanelManager.Toggle(GameplayPanelIds.QuestLog);
            }

            if (quickMenuHud.HandleInput(input))
                return true;

            if (gameplayPanelManager.HandleInput(input))
                return true;

            return gameplayPanelManager.BlocksGameplayInput;
        }

        private void DrawGameplayUI()
        {
            hotbarHud.Draw(_spriteBatch);
            DrawPlayerResourceUi();

            gameplayPanelManager.Draw(_spriteBatch);
            quickMenuHud.Draw(_spriteBatch, Mouse.GetState().Position);
        }

        private void DrawCenteredPanelText(
                string text,
                Rectangle panelBounds,
                int yOffset,
                Color color)
        {
            Vector2 textSize = boldpixels.MeasureString(text);

            _spriteBatch.DrawString(
                boldpixels,
                text,
                new Vector2(
                    panelBounds.Center.X - textSize.X / 2f,
                    panelBounds.Y + yOffset),
                color);
        }

        private void DrawPanel(Rectangle bounds, string title)
        {
            _spriteBatch.Draw(pixelTexture, bounds, Color.Black * 0.85f);

            Rectangle innerBounds = new Rectangle(
                bounds.X + 3,
                bounds.Y + 3,
                bounds.Width - 6,
                bounds.Height - 6);

            _spriteBatch.Draw(
                pixelTexture,
                innerBounds,
                Color.DarkSlateGray * 0.95f);

            _spriteBatch.DrawString(
                boldpixels,
                title,
                new Vector2(bounds.X + 20, bounds.Y + 20),
                Color.Gold);
        }
        private void DrawBottomHudPanel()
        {
            float scale = BottomHudPanelScale;

            Vector2 panelPosition = new Vector2(
                GraphicsDevice.Viewport.Width / 2f -
                bottomHudPanelTexture.Width * scale / 2f +
                BottomHudPanelOffsetFromBottomCenter.X,

                GraphicsDevice.Viewport.Height +
                BottomHudPanelOffsetFromBottomCenter.Y);

            _spriteBatch.Draw(
                bottomHudPanelTexture,
                panelPosition,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f);
        }
        private void AddInventoryResource( string resourceId, Texture2D icon,  int amount)
        {
            if (amount <= 0)
                return;

            foreach (InventoryResource resource in inventoryResources)
            {
                if (resource.Id == resourceId)
                {
                    resource.Add(amount);
                    return;
                }
            }

            inventoryResources.Add(
                new InventoryResource(resourceId, icon, amount));
        }
        private int GetInventoryResourceAmount(string resourceId)
        {
            foreach (InventoryResource resource in inventoryResources)
            {
                if (resource.Id == resourceId)
                    return resource.Amount;
            }

            return 0;
        }
        private bool TrySpendInventoryResource(string resourceId, int amount)
        {
            for (int i = 0; i < inventoryResources.Count; i++)
            {
                InventoryResource resource = inventoryResources[i];

                if (resource.Id != resourceId)
                    continue;

                if (!resource.TryRemove(amount))
                    return false;

                // При количество 0 слотът се освобождава.
                if (resource.Amount == 0)
                    inventoryResources.RemoveAt(i);

                return true;
            }

            return false;
        }

        private bool CloseOpenGameplayPanels()
        {
            return gameplayPanelManager.CloseActive();
        }

    }
}
