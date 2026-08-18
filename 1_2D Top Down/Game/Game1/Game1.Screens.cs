using Microsoft.Xna.Framework.Input;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        private enum ScreenNavigation
        {
            Direct,
            Forward,
            Back
        }

        private ScreenNavigation pendingScreenNavigation;

        private static bool IsScreenFlowState(GameFlowState state)
        {
            return state == GameFlowState.MainMenu ||
                   state == GameFlowState.Options ||
                   state == GameFlowState.Campaign;
        }

        private static string GetScreenId(GameFlowState state)
        {
            return state switch
            {
                GameFlowState.MainMenu => ScreenIds.MainMenu,
                GameFlowState.Options => ScreenIds.Options,
                GameFlowState.Campaign => ScreenIds.CampaignMap,
                _ => throw new System.ArgumentOutOfRangeException(nameof(state), state, "The state is not a UI screen.")
            };
        }

        private static GameFlowState GetFlowState(string screenId)
        {
            return screenId switch
            {
                ScreenIds.MainMenu => GameFlowState.MainMenu,
                ScreenIds.Options => GameFlowState.Options,
                ScreenIds.CampaignMap => GameFlowState.Campaign,
                _ => throw new System.ArgumentOutOfRangeException(nameof(screenId), screenId, "Unknown screen ID.")
            };
        }

        private void StartScreenTransition(GameFlowState state)
        {
            if (transitionManager.IsActive)
                return;

            StartSceneTransition(state);
            pendingScreenNavigation = ScreenNavigation.Forward;
        }

        private void StartScreenBackTransition()
        {
            if (!screenManager.TryGetPreviousScreenId(out string previousScreenId))
            {
                StartSceneTransition(GameFlowState.MainMenu);
                return;
            }

            StartSceneTransition(GetFlowState(previousScreenId));
            pendingScreenNavigation = ScreenNavigation.Back;
        }

        private void ActivateScreenForFlowState(GameFlowState state)
        {
            if (!IsScreenFlowState(state))
            {
                screenManager.ChangeScreen(ScreenIds.Gameplay);
                return;
            }

            string screenId = GetScreenId(state);

            if (pendingScreenNavigation == ScreenNavigation.Forward)
                screenManager.NavigateTo(screenId);
            else if (pendingScreenNavigation == ScreenNavigation.Back)
                screenManager.GoBack();
            else if (state == GameFlowState.MainMenu)
                screenManager.SetRoot(screenId);
            else
                screenManager.ChangeScreen(screenId);

            pendingScreenNavigation = ScreenNavigation.Direct;
        }

        internal void OnMenuScreenEntered()
        {
            PlayMusic(mainMenuMusic);
        }

        internal void OnOptionsScreenExited()
        {
            isMusicSliderDragging = false;
            isSoundEffectsSliderDragging = false;
        }

        internal void UpdateMainMenuScreen()
        {
            HandleMainMenuInput(Mouse.GetState());
        }

        internal void UpdateOptionsScreen()
        {
            HandleOptionsInput(Mouse.GetState());
        }

        internal void UpdateCampaignMapScreen()
        {
            HandleCampaignInput(Keyboard.GetState(), Mouse.GetState());
        }

        internal void DrawMainMenuScreen() => DrawMainMenu();
        internal void DrawOptionsScreen() => DrawOptions();
        internal void DrawCampaignMapScreen() => DrawCampaign();
    }
}
