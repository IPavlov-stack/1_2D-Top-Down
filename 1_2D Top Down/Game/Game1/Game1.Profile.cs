using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Text.Json;

#nullable enable

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        private const float ProfileAutosaveIntervalSeconds = 3f;
        private bool profileSavePending;
        private float profileAutosaveTimer;

        private PlayerProfile LoadOrCreateActivePlayerProfile()
        {
            try
            {
                PlayerProfile? profile = playerProfileStore.LoadActive();
                if (profile != null)
                {
                    hasPersistentActiveProfile = true;
                    return profile;
                }
            }
            catch (Exception exception) when (IsProfileStorageException(exception))
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Could not load the active player profile: " +
                    $"{exception.Message}");
            }

            hasPersistentActiveProfile = false;
            return new PlayerProfile(
                "Player",
                PlayerClass.Warrior);
        }

        private void AttachActivePlayerProfile()
        {
            activePlayerProfile.Changed += MarkProfileSavePending;
            gameplaySession.MissionCompleted += HandleMissionCompleted;
        }

        private void MarkProfileSavePending()
        {
            profileSavePending = true;
        }

        private void HandleMissionCompleted(MissionDefinition mission)
        {
            SaveActivePlayerProfile();
        }

        private void UpdateProfileAutosave(GameTime gameTime)
        {
            if (!profileSavePending || activePlayerProfile == null)
                return;

            profileAutosaveTimer +=
                (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (profileAutosaveTimer < ProfileAutosaveIntervalSeconds)
                return;

            SaveActivePlayerProfile();
        }

        private void SaveActivePlayerProfile()
        {
            if (activePlayerProfile == null || !hasPersistentActiveProfile)
                return;

            try
            {
                playerProfileStore.Save(activePlayerProfile);
                profileSavePending = false;
                profileAutosaveTimer = 0f;
            }
            catch (Exception exception) when (IsProfileStorageException(exception))
            {
                profileSavePending = true;
                profileAutosaveTimer = 0f;
                System.Diagnostics.Debug.WriteLine(
                    $"Could not save player profile '{activePlayerProfile.Name}': " +
                    $"{exception.Message}");
            }
        }

        private void RestoreProfileInventoryResources()
        {
            inventoryResources.Clear();

            int coins = activePlayerProfile.GetResourceAmount("coin");
            if (coins > 0)
            {
                inventoryResources.Add(
                    new InventoryResource("coin", uiCoinTexture, coins));
            }
        }

        private Player CreatePlayerForProfile(PlayerProfile profile)
        {
            return new Player(
                PlayerVisualDefinitions.MeleeLevel1,
                assetName => Content.Load<
                    Microsoft.Xna.Framework.Graphics.Texture2D>(assetName),
                playerStartPosition,
                profile);
        }

        private void ActivatePlayerProfile(PlayerProfile profile)
        {
            ArgumentNullException.ThrowIfNull(profile);

            if (hasPersistentActiveProfile)
                SaveActivePlayerProfile();

            activePlayerProfile.Changed -= MarkProfileSavePending;
            activePlayerProfile = profile;
            hasPersistentActiveProfile = true;
            activePlayerProfile.Changed += MarkProfileSavePending;

            gameplaySession.ReplacePlayer(
                CreatePlayerForProfile(activePlayerProfile));
            RestoreProfileInventoryResources();
            InitializeShopItems();

            try
            {
                playerProfileStore.Save(activePlayerProfile);
                profileSavePending = false;
                profileAutosaveTimer = 0f;
            }
            catch (Exception exception) when (IsProfileStorageException(exception))
            {
                profileSavePending = true;
                System.Diagnostics.Debug.WriteLine(
                    $"Could not activate player profile " +
                    $"'{activePlayerProfile.Name}': {exception.Message}");
            }
        }

        private void ResetRuntimeToTemporaryProfile()
        {
            activePlayerProfile.Changed -= MarkProfileSavePending;
            activePlayerProfile = new PlayerProfile(
                "Player",
                PlayerClass.Warrior);
            hasPersistentActiveProfile = false;
            profileSavePending = false;
            profileAutosaveTimer = 0f;
            activePlayerProfile.Changed += MarkProfileSavePending;

            gameplaySession.ReplacePlayer(
                CreatePlayerForProfile(activePlayerProfile));
            RestoreProfileInventoryResources();
            InitializeShopItems();
        }

        private void HandleGameExiting(object sender, EventArgs args)
        {
            SaveActivePlayerProfile();
        }

        private static bool IsProfileStorageException(Exception exception) =>
            exception is IOException or
            UnauthorizedAccessException or
            JsonException or
            InvalidDataException or
            ArgumentException;
    }
}
