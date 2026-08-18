using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace _1_2D_Top_Down
{
    public sealed class SoundOptionsPanel : IGameplayPanel
    {
        private const int PanelWidth = 700;
        private const int PanelHeight = 430;
        private const int SliderWidth = 460;
        private const int SliderHeight = 14;
        private readonly Func<Rectangle> viewport;
        private readonly Action closePanel;
        private readonly Texture2D panelTexture;
        private readonly Texture2D pixelTexture;
        private readonly SpriteFont font;
        private readonly UiSlider musicSlider;
        private readonly UiSlider soundEffectsSlider;

        public SoundOptionsPanel(
            Func<Rectangle> viewport,
            Func<float> musicVolume,
            Action<float> setMusicVolume,
            Func<float> soundEffectsVolume,
            Action<float> setSoundEffectsVolume,
            Action closePanel,
            Texture2D panelTexture,
            Texture2D pixelTexture,
            SpriteFont font)
        {
            this.viewport = viewport;
            musicSlider = new UiSlider("MUSIC", musicVolume, setMusicVolume);
            soundEffectsSlider = new UiSlider(
                "SOUND EFFECTS", soundEffectsVolume, setSoundEffectsVolume);
            this.closePanel = closePanel;
            this.panelTexture = panelTexture;
            this.pixelTexture = pixelTexture;
            this.font = font;
        }

        public string Id => GameplayPanelIds.SoundOptions;
        public bool BlocksGameplayInput => true;
        public void Open() => ResetSliders();
        public void Close() => ResetSliders();

        public bool HandleInput(GameplayUiInput input)
        {
            bool escapePressed = input.Keyboard.IsKeyDown(Keys.Escape) &&
                input.PreviousKeyboard.IsKeyUp(Keys.Escape);
            bool clicked = input.Mouse.LeftButton == ButtonState.Pressed &&
                input.PreviousMouse.LeftButton == ButtonState.Released;

            if (escapePressed || (clicked && CloseButtonBounds.Contains(input.Mouse.Position)))
            {
                closePanel();
                return true;
            }

            if (!musicSlider.HandleInput(input.Mouse, MusicSliderBounds))
                soundEffectsSlider.HandleInput(input.Mouse, SoundEffectsSliderBounds);
            return true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            UiDrawing.DrawNineSlicePanel(spriteBatch, panelTexture, PanelBounds);
            PanelTitle.Draw(spriteBatch, font, "SOUND OPTIONS", PanelBounds, 38, Color.Gold);
            musicSlider.Draw(spriteBatch, font, pixelTexture, MusicSliderBounds, Color.DodgerBlue);
            soundEffectsSlider.Draw(
                spriteBatch, font, pixelTexture, SoundEffectsSliderBounds, Color.DodgerBlue);

            Rectangle close = CloseButtonBounds;
            spriteBatch.Draw(pixelTexture, close, Color.Black);
            Rectangle inner = new(close.X + 3, close.Y + 3, close.Width - 6, close.Height - 6);
            spriteBatch.Draw(pixelTexture, inner,
                close.Contains(Mouse.GetState().Position) ? Color.SlateGray : Color.DimGray);
            PanelTitle.Draw(spriteBatch, font, "CLOSE", close, 12, Color.White);
        }

        private Rectangle PanelBounds
        {
            get
            {
                Rectangle view = viewport();
                return new Rectangle(view.Center.X - PanelWidth / 2,
                    view.Center.Y - PanelHeight / 2, PanelWidth, PanelHeight);
            }
        }
        private Rectangle MusicSliderBounds => new(
            PanelBounds.Center.X - SliderWidth / 2,
            PanelBounds.Top + 165, SliderWidth, SliderHeight);
        private Rectangle SoundEffectsSliderBounds => new(
            MusicSliderBounds.X, MusicSliderBounds.Y + 100, SliderWidth, SliderHeight);
        private Rectangle CloseButtonBounds => new(
            PanelBounds.Center.X - 90, PanelBounds.Bottom - 90, 180, 52);

        private void ResetSliders()
        {
            musicSlider.Reset();
            soundEffectsSlider.Reset();
        }
    }
}
