using _2D_Top_Down;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGameLibrary.Graphics;
using System;
using System.Collections.Generic;
using Tiled;


namespace _1_2D_Top_Down
{
    public partial class Game1 : Game
    {
        //game info
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Color BackgroundColor = new Color(119, 167, 255); // sky blue-ish
        private Texture2D pixelTexture;
        private bool isGameOver;
        private bool isEnemySpawningEnabled = true;
        private bool IsGameplayActive => gameFlowState == GameFlowState.Playing;
        private const int WindowSizeX = 1920;
        private const int WindowSizeY = 1080;
        private readonly StaticCollisionGrid mapCollisionGrid = new StaticCollisionGrid(128);
        private GameFlowState gameFlowState = GameFlowState.MainMenu;


        //input info
        private MouseState previousMouseState;
        private KeyboardState previousKeyboard;

        //player info
        private Player player;
        private Vector2 playerStartPosition = new Vector2(2150, 1850);
        private Texture2D playerProjectileTexture;
        private List<PlayerProjectile> projectiles = new List<PlayerProjectile>();
        private Texture2D playerShadowTexture;
        private PlayerProfile playerProfile;

        //collectables info
        private const int CoinDropChancePercent = 35;
        private Texture2D coinTexture;
        private List<Coin> coins = new List<Coin>();
        private const int ManaCrystalDropChancePercent = 12;
        private const float ManaCrystalRestoreAmount = 25f;
        private Texture2D manaCrystalTexture;
        private List<ManaCrystal> manaCrystals = new List<ManaCrystal>();
        private readonly List<InventoryResource> inventoryResources = new();

        //enemy info
        private const int MaxActiveEnemies = 30;
        private int ActiveEnemyCount => demons.Count + evilEyes.Count;
        private const int EnemySpatialCellSize = 128;
        private readonly SpatialGrid<Demon> demonSpatialGrid = new SpatialGrid<Demon>(EnemySpatialCellSize);
        private readonly SpatialGrid<Evil_Eye> evilEyeSpatialGrid = new SpatialGrid<Evil_Eye>(EnemySpatialCellSize);
        private readonly List<Demon> nearbyDemons = new();
        private readonly List<Evil_Eye> nearbyEvilEyes = new();

        //demon info
        private Texture2D demonTexture;
        private List<Demon> demons = new List<Demon>();
        private Texture2D demonDeathTexture;
        private List<DeathAnimation> demonDeathAnimations = new List<DeathAnimation>();
        private Texture2D demonShadowTexture;

        //evil eye info
        private Texture2D evilEyeProjectileTexture;
        private Texture2D evilEyeTexture;
        private List<Evil_Eye> evilEyes = new List<Evil_Eye>();
        private List<EnemyProjectile> enemyProjectiles = new List<EnemyProjectile>();
        private Texture2D evilEyeShadowTexture;

        //spawner info
        private float spawnTimer;
        private const float SpawnInterval = 1.5f;

        //camera info
        private Camera camera;

        //world map info
        private const int WorldWidth = 3000;
        private const int WorldHeight = 2000;
        private const int TileSize = 64;
        private const float EnvironmentScale = 0.25f;
        private Texture2D forestTileset;
        private TextureAtlas environmentGroundAtlas;
        private TextureAtlas environmentPropsAtlas;
        private TiledTileLayer waterMap;
        private TiledTileLayer worldMap;
        private TiledPropsLayer propsLayer;
        private TiledCollisionLayer collisionLayer;
        private List<Rectangle> solidCollisionRectangles;

        //ui info
        private Texture2D inventoryPanelTexture;
        private Texture2D questPanelTexture;
        private Texture2D spellsPanelTexture;
        private readonly Dictionary<string, Texture2D>shopUpgradeIcons = new();

        private const int ResourceFrameCount = 9;
        private const int ResourceFrameWidth = 63;
        private const int ResourceFrameHeight = 10;
        private const float ResourceFrameDuration = 0.08f;

        private int displayedHealthFrame;
        private int displayedManaFrame;

        private float healthFrameTimer;
        private float manaFrameTimer;

        private Texture2D healthMeterFrameTexture;
        private Texture2D healthMeterFillTexture;

        private Texture2D manaMeterFrameTexture;
        private Texture2D manaMeterFillTexture;
        private Texture2D bottomHudPanelTexture;
        private Texture2D panel9SliceTexture;
        private Texture2D inventorySlotTexture;
        private Texture2D uiCoinTexture;

        private Texture2D inventoryButtonTexture;
        private Texture2D statsButtonTexture;
        private Texture2D shopButtonTexture;
        private Texture2D mapButtonTexture;
        private Texture2D skillTreeButtonTexture;
        private Texture2D settingsButtonTexture;
        private Texture2D soundVolumeButtonTexture;

        //sound effects
        private const float SoundEffectsVolumeStep = 0.05f;
        private float soundEffectsVolume = 0.25f;
        private const float CoinPickupVolumeMultiplier = 1.8f;
        private float SoundEffectsVolume
        {
            get => soundEffectsVolume;
            set => soundEffectsVolume =
                MathHelper.Clamp(value, 0f, 1f);
        }
        private SoundEffect[] coinPickupSounds;
        private SoundEffect[] basicAttackSounds;
        private SoundEffect[] demonDeathSounds;
        private SoundEffect[] evilEyeDeathSounds;
        private SoundEffect manaCrystalCollectSound;

        //music
        private Song currentMusic;
        private Song backgroundMusic;
        private Song mainMenuMusic;
        private float musicVolume = 0.3f;
        private float MusicVolume
        {
            get => musicVolume;
            set
            {
                musicVolume = MathHelper.Clamp(value, 0f, 1f);
                MediaPlayer.Volume = musicVolume;
            }
        }
        //scene info
        private GameScene currentScene = GameScene.MainMenu;
        private const float SceneTransitionDuration = 0.8f;
        private bool isSceneTransitioning;
        private bool sceneChangedDuringTransition;
        private float sceneTransitionTimer;
        private GameScene nextScene;

        //fonts info
        private SpriteFont boldpixels;

        //others
        private Random random = new Random();
        private bool isDeveloperMode;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = WindowSizeX;
            _graphics.PreferredBackBufferHeight = WindowSizeY;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;

            _graphics.PreferredBackBufferWidth = displayMode.Width;
            _graphics.PreferredBackBufferHeight = displayMode.Height;
            _graphics.IsFullScreen = true;

            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            camera = new Camera();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            pixelTexture.SetData(new[] { Color.White });
            Texture2D playerTexture = Content.Load<Texture2D>("player/Character");
            playerShadowTexture = Content.Load<Texture2D>("player/shadow_player");
            demonTexture = Content.Load<Texture2D>("enemies/Demon/FLYING");
            demonDeathTexture = Content.Load<Texture2D>("enemies/Demon/DEATH");
            demonShadowTexture = Content.Load<Texture2D>("enemies/Demon/shadow_demon");
            evilEyeShadowTexture = Content.Load<Texture2D>("enemies/Evil Eye/shadow_eye");
            playerProjectileTexture =Content.Load<Texture2D>("projectiles/magic_projectile2");
            coinTexture = Content.Load<Texture2D>("Collectables/coin");
            manaCrystalTexture = Content.Load<Texture2D>("Collectables/mana_crystal_sheet");
            coinPickupSounds = new[]
            {
                Content.Load<SoundEffect>("Sounds/Coin/coin_1"),
                Content.Load<SoundEffect>("Sounds/Coin/coin_2"),
                Content.Load<SoundEffect>("Sounds/Coin/coin_3"),
            };
            basicAttackSounds = new[]
            {
                Content.Load<SoundEffect>("Sounds/Player/basic_attack1"),
                Content.Load<SoundEffect>("Sounds/Player/basic_attack2"),
                Content.Load<SoundEffect>("Sounds/Player/basic_attack3")
            };
            demonDeathSounds = new[]
            {
                Content.Load<SoundEffect>("Sounds/Enemies/Demon/demon_death1"),
                Content.Load<SoundEffect>("Sounds/Enemies/Demon/demon_death2"),
                Content.Load<SoundEffect>("Sounds/Enemies/Demon/demon_death3"),
                Content.Load<SoundEffect>("Sounds/Enemies/Demon/demon_death4")
            };
            evilEyeDeathSounds = new[]
            {
                Content.Load<SoundEffect>("Sounds/Enemies/Evil_Eye/evil_eye_death1"),
                Content.Load<SoundEffect>("Sounds/Enemies/Evil_Eye/evil_eye_death2"),
                Content.Load<SoundEffect>("Sounds/Enemies/Evil_Eye/evil_eye_death3"),
                Content.Load<SoundEffect>("Sounds/Enemies/Evil_Eye/evil_eye_death4")
            };
            manaCrystalCollectSound = Content.Load<SoundEffect>("Sounds/Mana/mana_collect");
            evilEyeProjectileTexture = Content.Load<Texture2D>("projectiles/evilEye/evilEye_projectile_sphere");
            evilEyeTexture = Content.Load<Texture2D>("enemies/Evil Eye/Evil Eye Sprite sheet");
            backgroundMusic = Content.Load<Song>("Music/ambient_forest");
            mainMenuMusic = Content.Load<Song>("Music/Main Menu/main_menu");
            boldpixels = Content.Load<SpriteFont>("Sprite fonts/boldpixels");
            inventoryPanelTexture = Content.Load<Texture2D>("UI/UI_InventoryPanel");
            questPanelTexture = Content.Load<Texture2D>("UI/UI_QuestPanel");
            spellsPanelTexture = Content.Load<Texture2D>("UI/UI_SpellsPanel");
            bottomHudPanelTexture = Content.Load<Texture2D>("UI/bottom_hud_panel");
            healthMeterFrameTexture = Content.Load<Texture2D>("UI/health_meter_frame");
            healthMeterFillTexture = Content.Load<Texture2D>("UI/health_meter_fill");
            manaMeterFrameTexture = Content.Load<Texture2D>("UI/mana_meter_frame");
            manaMeterFillTexture = Content.Load<Texture2D>("UI/mana_meter_fill");
            panel9SliceTexture = Content.Load<Texture2D>("UI/nine slice 256x256 17gap/panel_9slice");
            inventorySlotTexture = Content.Load<Texture2D>("UI/panel_inventory_slot");
            uiCoinTexture = Content.Load<Texture2D>("UI/inventory icons/UI_coin");
            inventoryButtonTexture = Content.Load<Texture2D>("UI/ingame buttons/inventory-button");
            statsButtonTexture = Content.Load<Texture2D>("UI/ingame buttons/stats-button");
            shopButtonTexture = Content.Load<Texture2D>("UI/ingame buttons/shop-button");
            mapButtonTexture =Content.Load<Texture2D>("UI/ingame buttons/map-button");
            skillTreeButtonTexture = Content.Load<Texture2D>("UI/ingame buttons/skill-tree-button");
            settingsButtonTexture = Content.Load<Texture2D>("UI/ingame buttons/settings-button");
            soundVolumeButtonTexture = Content.Load<Texture2D>("UI/ingame buttons/sound-volume-button");

            environmentGroundAtlas = TextureAtlas.FromFile(Content, "Environment/EnvironmentGroundAtlas.xml");
            environmentPropsAtlas = TextureAtlas.FromFile(Content, "Environment/EnvironmentPropsAtlas.xml");
            waterMap = TiledTileLayer.FromFile(Content, "Maps/ForestMap.tmx", "Environment/Water/tileset_water256x256", "tileset_water256x256.tsx", EnvironmentScale, "Water");
            worldMap = TiledTileLayer.FromFile(Content, "Maps/ForestMap.tmx", "Environment/EnvironmentGroundAtlas", "EnvironmentGround.tsx", EnvironmentScale, "Ground");
            propsLayer = TiledPropsLayer.FromFile(Content, "Maps/ForestMap.tmx", environmentPropsAtlas, EnvironmentScale);
            collisionLayer = TiledCollisionLayer.FromFile(Content, "Maps/ForestMap.tmx", EnvironmentScale);
            TiledWaterCollisionLayer waterCollisionLayer = TiledWaterCollisionLayer.FromFile(Content, "Maps/ForestMap.tmx", "tileset_water256x256.tsx", EnvironmentScale);

            solidCollisionRectangles = new List<Rectangle>(collisionLayer.Rectangles);
            solidCollisionRectangles.AddRange(waterCollisionLayer.Rectangles);
            mapCollisionGrid.Build(solidCollisionRectangles);
            LoadPortals();

            playerProfile = new PlayerProfile();
            player = new Player(playerTexture, playerStartPosition, playerProfile);
            LoadShopUpgradeIcons();
            InitializeShopItems();

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = MusicVolume;

            if (MediaPlayer.State != MediaState.Stopped)
            {
                MediaPlayer.Stop();
            }

            PlayMusic(mainMenuMusic);
        }

        private void PlayMusic(Song music)
        {
            if (currentMusic == music &&
                MediaPlayer.State == MediaState.Playing)
            {
                return;
            }

            if (MediaPlayer.State != MediaState.Stopped)
            {
                MediaPlayer.Stop();
            }

            MediaPlayer.Play(music);
            currentMusic = music;
        }

        private void UpdateMusicForCurrentScene()
        {
            if (currentScene == GameScene.Playing)
            {
                PlayMusic(backgroundMusic);
            }
            else
            {
                // Main Menu и Options.
                PlayMusic(mainMenuMusic);
            }
        }
        protected override void Update(GameTime gameTime)
        {
            portalLayer.Update(gameTime);

            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            UpdateSceneTransition(gameTime);

            if (isSceneTransitioning)
            {
                previousKeyboard = keyboard;
                previousMouseState = mouse;

                base.Update(gameTime);
                return;
            }

            if (currentScene == GameScene.MainMenu)
            {
                HandleMainMenuInput(mouse);

                previousKeyboard = keyboard;
                previousMouseState = mouse;

                base.Update(gameTime);
                return;
            }

            if (currentScene == GameScene.Options)
            {
                HandleOptionsInput(mouse);

                previousKeyboard = keyboard;
                previousMouseState = mouse;

                base.Update(gameTime);
                return;
            }
            if (isGameOver)
            {
                //puase менюто не може да отвори при game over screen или да остане
                isExitConfirmationOpen = false;

                bool pressedRestart =
                    keyboard.IsKeyDown(Keys.R) &&
                    previousKeyboard.IsKeyUp(Keys.R);

                if (pressedRestart)
                {
                    RestartGame();
                }

                previousKeyboard = keyboard;
                previousMouseState = mouse;

                base.Update(gameTime);
                return;
            }
            if (isExitConfirmationOpen)
            {
                HandleExitConfirmationInput(keyboard, mouse);

                previousKeyboard = keyboard;
                previousMouseState = mouse;

                base.Update(gameTime);
                return;
            }

            bool pressedEscape =
                keyboard.IsKeyDown(Keys.Escape) &&
                previousKeyboard.IsKeyUp(Keys.Escape);

            if (pressedEscape)
            {
                bool closedPanel = CloseOpenGameplayPanels();

                if (!closedPanel)
                {
                    isExitConfirmationOpen = true;
                }

                previousKeyboard = keyboard;
                previousMouseState = mouse;

                base.Update(gameTime);
                return;
            }
            HandleDeveloperMode(keyboard);
            Vector2 playerCenter = player.Position +
                       new Vector2(player.texture.Width / 2, player.texture.Height / 2);

            Vector2 screenCenter = new Vector2(
                GraphicsDevice.Viewport.Width / 2,
                GraphicsDevice.Viewport.Height / 2);

            CenterCameraOnPlayer();
            if (!isGameOver)
            {
                bool gameplayUiClickHandled =
                    HandleGameplayUIInput(keyboard, mouse);

                if (!gameplayUiClickHandled)
                {
                    HandlePlayerShooting(mouse, keyboard);
                }
                UpdateGameObjects(gameTime);
            }
            else if (keyboard.IsKeyDown(Keys.R))
            {
                RestartGame();
            }

            previousKeyboard = keyboard;
            previousMouseState = mouse;

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            if (currentScene == GameScene.MainMenu)
            {
                DrawMainMenu();
                DrawSceneTransition();
                base.Draw(gameTime);
                return;
            }

            if (currentScene == GameScene.Options)
            {
                DrawOptions();
                DrawSceneTransition();

                base.Draw(gameTime);
                return;
            }

            GraphicsDevice.Clear(
                isDeveloperMode
                    ? Color.DimGray
                    : isGameOver ? Color.Black : BackgroundColor);

            _spriteBatch.Begin(
                transformMatrix: camera.Transform,
                samplerState: SamplerState.PointClamp);

            if (isDeveloperMode)
            {
                DrawDeveloperMode();
            }
            else
            {
                DrawNormalWorld();
            }

            _spriteBatch.End();

            DrawSceneTransition();
            DrawUi();

            base.Draw(gameTime);
        }
        private static int ScaleUi(int value, float scale)
        {
            return Math.Max(1, (int)MathF.Round(value * scale));
        }
    }
}
