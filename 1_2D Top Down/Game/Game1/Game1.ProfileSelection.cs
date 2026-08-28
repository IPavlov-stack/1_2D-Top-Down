using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        private const int MaximumProfileSlots = 4;
        private const int MaximumProfileNameLength = 32;

        private enum ProfileDialogMode
        {
            None,
            Create,
            ConfirmDelete
        }

        private readonly List<PlayerProfile> availablePlayerProfiles = new();
        private ProfileDialogMode profileDialogMode;
        private string profileNameInput = string.Empty;
        private PlayerClass profileClassSelection = PlayerClass.Unassigned;
        private PlayerProfile? profilePendingDeletion;
        private string profileScreenMessage = string.Empty;

        internal void OnProfileSelectionScreenEntered()
        {
            PlayMusic(mainMenuMusic);
            profileDialogMode = ProfileDialogMode.None;
            profilePendingDeletion = null;
            profileScreenMessage = string.Empty;
            RefreshAvailablePlayerProfiles();
        }

        internal void OnProfileSelectionScreenExited()
        {
            profileDialogMode = ProfileDialogMode.None;
            profilePendingDeletion = null;
            profileNameInput = string.Empty;
            profileClassSelection = PlayerClass.Unassigned;
        }

        private void HandleProfileTextInput(
            object? sender,
            TextInputEventArgs args)
        {
            if (profileDialogMode != ProfileDialogMode.Create)
                return;

            if (args.Character == '\b')
            {
                if (profileNameInput.Length > 0)
                    profileNameInput = profileNameInput[..^1];
                return;
            }

            if (char.IsControl(args.Character) ||
                profileNameInput.Length >= MaximumProfileNameLength)
            {
                return;
            }

            profileNameInput += args.Character;
        }

        private void HandleProfileSelectionInput(
            KeyboardState keyboard,
            MouseState mouse)
        {
            bool pressedEscape =
                keyboard.IsKeyDown(Keys.Escape) &&
                previousKeyboard.IsKeyUp(Keys.Escape);
            bool clicked =
                mouse.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released;

            if (profileDialogMode == ProfileDialogMode.Create)
            {
                HandleCreateProfileDialogInput(
                    keyboard,
                    mouse,
                    clicked,
                    pressedEscape);
                return;
            }

            if (profileDialogMode == ProfileDialogMode.ConfirmDelete)
            {
                HandleDeleteProfileDialogInput(
                    mouse,
                    clicked,
                    pressedEscape);
                return;
            }

            if (pressedEscape)
            {
                StartScreenBackTransition();
                return;
            }

            if (!clicked)
                return;

            for (int slotIndex = 0;
                 slotIndex < MaximumProfileSlots;
                 slotIndex++)
            {
                if (slotIndex < availablePlayerProfiles.Count)
                {
                    PlayerProfile profile =
                        availablePlayerProfiles[slotIndex];

                    if (GetProfilePlayButtonBounds(slotIndex)
                        .Contains(mouse.Position))
                    {
                        ActivatePlayerProfile(profile);
                        StartScreenTransition(GameFlowState.Campaign);
                        return;
                    }

                    if (GetProfileDeleteButtonBounds(slotIndex)
                        .Contains(mouse.Position))
                    {
                        profilePendingDeletion = profile;
                        profileDialogMode =
                            ProfileDialogMode.ConfirmDelete;
                        return;
                    }
                }
                else if (slotIndex >= availablePlayerProfiles.Count &&
                         GetProfileCreateButtonBounds(slotIndex)
                             .Contains(mouse.Position))
                {
                    OpenCreateProfileDialog();
                    return;
                }
            }

            if (GetProfileBackButtonBounds().Contains(mouse.Position))
                StartScreenBackTransition();
        }

        private void HandleCreateProfileDialogInput(
            KeyboardState keyboard,
            MouseState mouse,
            bool clicked,
            bool pressedEscape)
        {
            if (pressedEscape)
            {
                CloseProfileDialog();
                return;
            }

            bool pressedEnter =
                keyboard.IsKeyDown(Keys.Enter) &&
                previousKeyboard.IsKeyUp(Keys.Enter);

            if (clicked)
            {
                for (int classIndex = 0; classIndex < 3; classIndex++)
                {
                    if (!GetProfileClassButtonBounds(classIndex)
                        .Contains(mouse.Position))
                    {
                        continue;
                    }

                    profileClassSelection = classIndex switch
                    {
                        0 => PlayerClass.Warrior,
                        1 => PlayerClass.Mage,
                        _ => PlayerClass.Hunter
                    };
                    profileScreenMessage = string.Empty;
                    return;
                }

                if (GetCreateProfileConfirmButtonBounds()
                    .Contains(mouse.Position))
                {
                    TryCreatePlayerProfile();
                    return;
                }

                if (GetProfileDialogCancelButtonBounds()
                    .Contains(mouse.Position))
                {
                    CloseProfileDialog();
                    return;
                }
            }

            if (pressedEnter)
                TryCreatePlayerProfile();
        }

        private void HandleDeleteProfileDialogInput(
            MouseState mouse,
            bool clicked,
            bool pressedEscape)
        {
            if (pressedEscape)
            {
                CloseProfileDialog();
                return;
            }

            if (!clicked)
                return;

            if (GetDeleteProfileConfirmButtonBounds()
                .Contains(mouse.Position))
            {
                DeletePendingPlayerProfile();
                return;
            }

            if (GetProfileDialogCancelButtonBounds().Contains(mouse.Position))
                CloseProfileDialog();
        }

        private void OpenCreateProfileDialog()
        {
            profileNameInput = string.Empty;
            profileClassSelection = PlayerClass.Unassigned;
            profileScreenMessage = string.Empty;
            profileDialogMode = ProfileDialogMode.Create;
        }

        private void CloseProfileDialog()
        {
            profileDialogMode = ProfileDialogMode.None;
            profilePendingDeletion = null;
            profileNameInput = string.Empty;
            profileClassSelection = PlayerClass.Unassigned;
            profileScreenMessage = string.Empty;
        }

        private void TryCreatePlayerProfile()
        {
            string profileName = profileNameInput.Trim();

            if (profileName.Length == 0)
            {
                profileScreenMessage = "ENTER A PROFILE NAME";
                return;
            }

            if (profileClassSelection == PlayerClass.Unassigned)
            {
                profileScreenMessage = "SELECT A CLASS";
                return;
            }

            if (availablePlayerProfiles.Any(profile =>
                    string.Equals(
                        profile.Name,
                        profileName,
                        StringComparison.OrdinalIgnoreCase)))
            {
                profileScreenMessage = "THAT PROFILE NAME IS ALREADY USED";
                return;
            }

            try
            {
                PlayerProfile profile = new(
                    profileName,
                    profileClassSelection);
                playerProfileStore.Save(profile, makeActive: false);
                CloseProfileDialog();
                RefreshAvailablePlayerProfiles();
            }
            catch (Exception exception) when (
                IsProfileStorageException(exception))
            {
                profileScreenMessage = "PROFILE COULD NOT BE CREATED";
                System.Diagnostics.Debug.WriteLine(
                    $"Could not create profile '{profileName}': " +
                    $"{exception.Message}");
            }
        }

        private void DeletePendingPlayerProfile()
        {
            if (profilePendingDeletion == null)
                return;

            PlayerProfile profile = profilePendingDeletion;

            try
            {
                playerProfileStore.Delete(profile.Id);

                if (hasPersistentActiveProfile &&
                    activePlayerProfile.Id == profile.Id)
                {
                    ResetRuntimeToTemporaryProfile();
                }

                CloseProfileDialog();
                RefreshAvailablePlayerProfiles();
            }
            catch (Exception exception) when (
                IsProfileStorageException(exception))
            {
                profileScreenMessage = "PROFILE COULD NOT BE DELETED";
                System.Diagnostics.Debug.WriteLine(
                    $"Could not delete profile '{profile.Name}': " +
                    $"{exception.Message}");
            }
        }

        private void RefreshAvailablePlayerProfiles()
        {
            availablePlayerProfiles.Clear();

            try
            {
                availablePlayerProfiles.AddRange(
                    playerProfileStore
                        .LoadAll()
                        .Take(MaximumProfileSlots));
            }
            catch (Exception exception) when (
                IsProfileStorageException(exception))
            {
                profileScreenMessage = "PROFILES COULD NOT BE LOADED";
                System.Diagnostics.Debug.WriteLine(
                    $"Could not list player profiles: {exception.Message}");
            }
        }

        private void DrawProfileSelection()
        {
            GraphicsDevice.Clear(new Color(25, 30, 40));

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            DrawCenteredText("SELECT PROFILE", 70, Color.Gold, 2.2f);
            DrawCenteredText(
                "Choose a hero or create a new profile",
                145,
                Color.LightGray,
                0.85f);

            for (int slotIndex = 0;
                 slotIndex < MaximumProfileSlots;
                 slotIndex++)
            {
                if (slotIndex < availablePlayerProfiles.Count)
                {
                    DrawOccupiedProfileSlot(
                        slotIndex,
                        availablePlayerProfiles[slotIndex]);
                }
                else
                {
                    DrawEmptyProfileSlot(slotIndex);
                }
            }

            DrawProfileActionButton(
                GetProfileBackButtonBounds(),
                "BACK",
                Color.DimGray,
                enabled: true);

            if (profileDialogMode != ProfileDialogMode.None)
                DrawProfileDialog();
            else if (!string.IsNullOrWhiteSpace(profileScreenMessage))
                DrawCenteredText(
                    profileScreenMessage,
                    GraphicsDevice.Viewport.Height - 85,
                    Color.IndianRed,
                    0.8f);

            _spriteBatch.End();
        }

        private void DrawOccupiedProfileSlot(
            int slotIndex,
            PlayerProfile profile)
        {
            Rectangle cardBounds = GetProfileCardBounds(slotIndex);
            bool isActive = hasPersistentActiveProfile &&
                            activePlayerProfile.Id == profile.Id;

            DrawProfileCardBackground(
                cardBounds,
                isActive ? Color.Gold : Color.Black);

            DrawCenteredProfileText(
                profile.Name,
                new Rectangle(
                    cardBounds.X + 15,
                    cardBounds.Y + 28,
                    cardBounds.Width - 30,
                    42),
                Color.Gold,
                1.1f);

            if (isActive)
            {
                DrawCenteredProfileText(
                    "ACTIVE",
                    new Rectangle(
                        cardBounds.X + 15,
                        cardBounds.Y + 82,
                        cardBounds.Width - 30,
                        28),
                    Color.LightGreen,
                    0.65f);
            }

            int infoTop = cardBounds.Y + 140;
            DrawProfileInfoLine(
                cardBounds,
                infoTop,
                "CLASS",
                GetPlayerClassDisplayName(profile.Class));
            DrawProfileInfoLine(
                cardBounds,
                infoTop + 62,
                "LEVEL",
                profile.Experience.Level.ToString());
            DrawProfileInfoLine(
                cardBounds,
                infoTop + 124,
                "LATEST MISSION",
                GetLastUnlockedMissionName(profile));
            DrawProfileInfoLine(
                cardBounds,
                infoTop + 186,
                "MISSIONS COMPLETE",
                $"{profile.CompletedMissionIds.Count}/" +
                $"{CampaignMissions.All.Count}");

            DrawProfileActionButton(
                GetProfilePlayButtonBounds(slotIndex),
                "PLAY",
                new Color(42, 115, 70),
                enabled: true);
            DrawProfileActionButton(
                GetProfileDeleteButtonBounds(slotIndex),
                "DELETE",
                new Color(125, 45, 45),
                enabled: true);
        }

        private void DrawEmptyProfileSlot(int slotIndex)
        {
            Rectangle cardBounds = GetProfileCardBounds(slotIndex);
            DrawProfileCardBackground(cardBounds, Color.Black);

            DrawCenteredProfileText(
                $"PROFILE {slotIndex + 1}",
                new Rectangle(
                    cardBounds.X + 15,
                    cardBounds.Y + 35,
                    cardBounds.Width - 30,
                    40),
                Color.Gray,
                0.9f);

            DrawCenteredProfileText(
                "EMPTY SLOT",
                new Rectangle(
                    cardBounds.X + 20,
                    cardBounds.Center.Y - 25,
                    cardBounds.Width - 40,
                    50),
                Color.DarkGray,
                1f);

            bool canCreate =
                availablePlayerProfiles.Count < MaximumProfileSlots;
            DrawProfileActionButton(
                GetProfileCreateButtonBounds(slotIndex),
                "CREATE",
                new Color(55, 80, 115),
                canCreate);
        }

        private void DrawProfileCardBackground(
            Rectangle bounds,
            Color borderColor)
        {
            _spriteBatch.Draw(pixelTexture, bounds, borderColor);

            Rectangle inner = new(
                bounds.X + 4,
                bounds.Y + 4,
                bounds.Width - 8,
                bounds.Height - 8);
            _spriteBatch.Draw(pixelTexture, inner, new Color(43, 49, 61));
        }

        private void DrawProfileInfoLine(
            Rectangle cardBounds,
            int y,
            string label,
            string value)
        {
            DrawCenteredProfileText(
                label,
                new Rectangle(
                    cardBounds.X + 15,
                    y,
                    cardBounds.Width - 30,
                    23),
                Color.Gray,
                0.58f);
            DrawCenteredProfileText(
                value,
                new Rectangle(
                    cardBounds.X + 15,
                    y + 26,
                    cardBounds.Width - 30,
                    30),
                Color.White,
                0.75f);
        }

        private void DrawProfileDialog()
        {
            _spriteBatch.Draw(
                pixelTexture,
                GraphicsDevice.Viewport.Bounds,
                Color.Black * 0.72f);

            Rectangle dialogBounds = GetProfileDialogBounds();
            DrawProfileCardBackground(dialogBounds, Color.Gold);

            if (profileDialogMode == ProfileDialogMode.Create)
                DrawCreateProfileDialog(dialogBounds);
            else
                DrawDeleteProfileDialog(dialogBounds);
        }

        private void DrawCreateProfileDialog(Rectangle dialogBounds)
        {
            DrawCenteredProfileText(
                "CREATE PROFILE",
                new Rectangle(
                    dialogBounds.X + 30,
                    dialogBounds.Y + 32,
                    dialogBounds.Width - 60,
                    45),
                Color.Gold,
                1.2f);

            _spriteBatch.DrawString(
                boldpixels,
                "NAME",
                new Vector2(dialogBounds.X + 62, dialogBounds.Y + 112),
                Color.LightGray,
                0f,
                Vector2.Zero,
                0.7f,
                SpriteEffects.None,
                0f);

            Rectangle nameBounds = GetProfileNameInputBounds();
            _spriteBatch.Draw(pixelTexture, nameBounds, Color.Black);
            Rectangle nameInner = new(
                nameBounds.X + 3,
                nameBounds.Y + 3,
                nameBounds.Width - 6,
                nameBounds.Height - 6);
            _spriteBatch.Draw(pixelTexture, nameInner, new Color(27, 32, 42));
            _spriteBatch.DrawString(
                boldpixels,
                profileNameInput + "|",
                new Vector2(nameInner.X + 14, nameInner.Y + 14),
                Color.White);

            DrawCenteredProfileText(
                "CHOOSE CLASS",
                new Rectangle(
                    dialogBounds.X + 30,
                    dialogBounds.Y + 210,
                    dialogBounds.Width - 60,
                    35),
                Color.LightGray,
                0.8f);

            PlayerClass[] classes =
            {
                PlayerClass.Warrior,
                PlayerClass.Mage,
                PlayerClass.Hunter
            };
            for (int index = 0; index < classes.Length; index++)
            {
                PlayerClass playerClass = classes[index];
                DrawProfileActionButton(
                    GetProfileClassButtonBounds(index),
                    GetPlayerClassDisplayName(playerClass),
                    profileClassSelection == playerClass
                        ? new Color(140, 105, 35)
                        : Color.DimGray,
                    enabled: true);
            }

            bool canCreate =
                !string.IsNullOrWhiteSpace(profileNameInput) &&
                profileClassSelection != PlayerClass.Unassigned;
            DrawProfileActionButton(
                GetCreateProfileConfirmButtonBounds(),
                "CREATE",
                new Color(42, 115, 70),
                canCreate);
            DrawProfileActionButton(
                GetProfileDialogCancelButtonBounds(),
                "CANCEL",
                Color.DimGray,
                enabled: true);

            if (!string.IsNullOrWhiteSpace(profileScreenMessage))
            {
                DrawCenteredProfileText(
                    profileScreenMessage,
                    new Rectangle(
                        dialogBounds.X + 25,
                        dialogBounds.Bottom - 54,
                        dialogBounds.Width - 50,
                        25),
                    Color.IndianRed,
                    0.65f);
            }
        }

        private void DrawDeleteProfileDialog(Rectangle dialogBounds)
        {
            DrawCenteredProfileText(
                "DELETE PROFILE?",
                new Rectangle(
                    dialogBounds.X + 30,
                    dialogBounds.Y + 60,
                    dialogBounds.Width - 60,
                    50),
                Color.IndianRed,
                1.25f);

            string profileName = profilePendingDeletion?.Name ?? string.Empty;
            DrawCenteredProfileText(
                $"All progress for {profileName} will be permanently deleted.",
                new Rectangle(
                    dialogBounds.X + 45,
                    dialogBounds.Y + 160,
                    dialogBounds.Width - 90,
                    70),
                Color.White,
                0.75f);

            DrawProfileActionButton(
                GetDeleteProfileConfirmButtonBounds(),
                "DELETE",
                new Color(135, 38, 38),
                enabled: true);
            DrawProfileActionButton(
                GetProfileDialogCancelButtonBounds(),
                "CANCEL",
                Color.DimGray,
                enabled: true);

            if (!string.IsNullOrWhiteSpace(profileScreenMessage))
            {
                DrawCenteredProfileText(
                    profileScreenMessage,
                    new Rectangle(
                        dialogBounds.X + 25,
                        dialogBounds.Bottom - 54,
                        dialogBounds.Width - 50,
                        25),
                    Color.IndianRed,
                    0.65f);
            }
        }

        private void DrawProfileActionButton(
            Rectangle bounds,
            string text,
            Color color,
            bool enabled)
        {
            bool hovered = enabled &&
                           bounds.Contains(Mouse.GetState().Position);
            Color background = enabled
                ? (hovered ? Color.Lerp(color, Color.White, 0.22f) : color)
                : Color.DarkSlateGray * 0.55f;

            _spriteBatch.Draw(pixelTexture, bounds, Color.Black);
            Rectangle inner = new(
                bounds.X + 2,
                bounds.Y + 2,
                bounds.Width - 4,
                bounds.Height - 4);
            _spriteBatch.Draw(pixelTexture, inner, background);

            DrawCenteredProfileText(
                text,
                bounds,
                enabled ? Color.White : Color.Gray,
                0.82f);
        }

        private void DrawCenteredProfileText(
            string text,
            Rectangle bounds,
            Color color,
            float maximumScale)
        {
            Vector2 unscaledSize = boldpixels.MeasureString(text);
            float scale = maximumScale;
            if (unscaledSize.X * scale > bounds.Width - 8)
                scale = Math.Max(0.35f, (bounds.Width - 8) / unscaledSize.X);

            Vector2 size = unscaledSize * scale;
            Vector2 position = new(
                bounds.Center.X - size.X / 2f,
                bounds.Center.Y - size.Y / 2f);

            _spriteBatch.DrawString(
                boldpixels,
                text,
                position,
                color,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f);
        }

        private string GetLastUnlockedMissionName(PlayerProfile profile)
        {
            int unlockedMissionIndex = 0;
            for (int index = 0;
                 index < CampaignMissions.All.Count - 1;
                 index++)
            {
                if (!profile.HasCompletedMission(
                        CampaignMissions.All[index].Id))
                {
                    break;
                }

                unlockedMissionIndex = index + 1;
            }

            return CampaignMissions.All[unlockedMissionIndex].Name;
        }

        private static string GetPlayerClassDisplayName(PlayerClass playerClass)
        {
            return playerClass switch
            {
                PlayerClass.Warrior => "WARRIOR",
                PlayerClass.Mage => "MAGE",
                PlayerClass.Hunter => "HUNTER",
                _ => "UNASSIGNED"
            };
        }

        private Rectangle GetProfileCardBounds(int slotIndex)
        {
            const int horizontalMargin = 45;
            const int gap = 22;
            int availableWidth =
                GraphicsDevice.Viewport.Width - horizontalMargin * 2 -
                gap * (MaximumProfileSlots - 1);
            int cardWidth = Math.Min(360, availableWidth / MaximumProfileSlots);
            int rowWidth =
                cardWidth * MaximumProfileSlots +
                gap * (MaximumProfileSlots - 1);
            int startX = (GraphicsDevice.Viewport.Width - rowWidth) / 2;

            return new Rectangle(
                startX + slotIndex * (cardWidth + gap),
                215,
                cardWidth,
                430);
        }

        private Rectangle GetProfilePlayButtonBounds(int slotIndex)
        {
            Rectangle card = GetProfileCardBounds(slotIndex);
            int gap = 8;
            int deleteWidth = Math.Max(105, card.Width * 36 / 100);
            return new Rectangle(
                card.X,
                card.Bottom + 14,
                card.Width - deleteWidth - gap,
                58);
        }

        private Rectangle GetProfileDeleteButtonBounds(int slotIndex)
        {
            Rectangle card = GetProfileCardBounds(slotIndex);
            int deleteWidth = Math.Max(105, card.Width * 36 / 100);
            return new Rectangle(
                card.Right - deleteWidth,
                card.Bottom + 14,
                deleteWidth,
                58);
        }

        private Rectangle GetProfileCreateButtonBounds(int slotIndex)
        {
            Rectangle card = GetProfileCardBounds(slotIndex);
            return new Rectangle(
                card.X,
                card.Bottom + 14,
                card.Width,
                58);
        }

        private Rectangle GetProfileBackButtonBounds()
        {
            return new Rectangle(
                30,
                GraphicsDevice.Viewport.Height - 88,
                210,
                58);
        }

        private Rectangle GetProfileDialogBounds()
        {
            int width = Math.Min(760, GraphicsDevice.Viewport.Width - 80);
            int height = 540;
            return new Rectangle(
                GraphicsDevice.Viewport.Width / 2 - width / 2,
                GraphicsDevice.Viewport.Height / 2 - height / 2,
                width,
                height);
        }

        private Rectangle GetProfileNameInputBounds()
        {
            Rectangle dialog = GetProfileDialogBounds();
            return new Rectangle(
                dialog.X + 58,
                dialog.Y + 142,
                dialog.Width - 116,
                58);
        }

        private Rectangle GetProfileClassButtonBounds(int classIndex)
        {
            Rectangle dialog = GetProfileDialogBounds();
            const int gap = 12;
            int width = (dialog.Width - 116 - gap * 2) / 3;
            return new Rectangle(
                dialog.X + 58 + classIndex * (width + gap),
                dialog.Y + 260,
                width,
                62);
        }

        private Rectangle GetCreateProfileConfirmButtonBounds()
        {
            Rectangle dialog = GetProfileDialogBounds();
            return new Rectangle(
                dialog.Center.X - 224,
                dialog.Y + 372,
                210,
                62);
        }

        private Rectangle GetDeleteProfileConfirmButtonBounds()
        {
            Rectangle dialog = GetProfileDialogBounds();
            return new Rectangle(
                dialog.Center.X - 224,
                dialog.Y + 300,
                210,
                62);
        }

        private Rectangle GetProfileDialogCancelButtonBounds()
        {
            Rectangle dialog = GetProfileDialogBounds();
            int y = profileDialogMode == ProfileDialogMode.Create
                ? dialog.Y + 372
                : dialog.Y + 300;
            return new Rectangle(
                dialog.Center.X + 14,
                y,
                210,
                62);
        }
    }
}
