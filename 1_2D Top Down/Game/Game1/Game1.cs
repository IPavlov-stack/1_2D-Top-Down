using _2D_Top_Down;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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
        private Color BackgroundColor = new Color(141, 142, 143); 
                                        //new Color(119, 167, 255); // sky blue-ish
        private Texture2D pixelTexture;
        private bool IsGameplayActive => gameFlowState == GameFlowState.Playing;
        private const int WindowSizeX = 1920;
        private const int WindowSizeY = 1080;
        private readonly GameplaySession gameplaySession =
            new GameplaySession(CampaignMissions.ForestOutskirts);
        private readonly PlayerProfileStore playerProfileStore = new();
        private PlayerProfile activePlayerProfile;
        private bool hasPersistentActiveProfile;
        private GameFlowState gameFlowState => gameplaySession.FlowState;
        private GameFlowState nextGameFlowState;
        private readonly ScreenManager screenManager = new();
        private readonly OverlayManager overlayManager = new();
        private readonly WorldRenderQueue worldRenderQueue = new();
        private readonly ScreenTransitionManager transitionManager = new(0.8f);
        private PauseOverlay pauseOverlay;

        //campaign info
        private MissionRuntime missionRuntime => gameplaySession.Mission;
        private Texture2D campaignMapTexture;
        private Texture2D missionNodeTexture;


        //input info
        private MouseState previousMouseState;
        private KeyboardState previousKeyboard;

        //player info
        private Player player => gameplaySession.Player;
        private Vector2 playerStartPosition;
        private GameWorld gameWorld => gameplaySession.World;
        private ProjectileManager projectileManager => gameWorld.Projectiles;
        private WorldEffectManager worldEffectManager => gameWorld.WorldEffects;

        //collectables info
        private const int CoinDropChancePercent = 35;
        private Texture2D coinTexture;
        private CollectibleManager collectibleManager => gameWorld.Collectibles;
        private IReadOnlyList<Coin> coins => collectibleManager.Coins;
        private const int ManaCrystalDropChancePercent = 11;
        private const float ManaCrystalRestoreAmount = 25f;
        private Texture2D manaCrystalTexture;
        private IReadOnlyList<ManaCrystal> manaCrystals => collectibleManager.ManaCrystals;
        private readonly List<InventoryResource> inventoryResources = new();

        //enemy info
        private EnemyManager enemyManager => gameWorld.Enemies;
        private EnemyFactory enemyFactory;
        private EnemyActionProcessor enemyActionProcessor;
        private IReadOnlyList<Enemy> enemies => enemyManager.Enemies;
        private List<DeathAnimation> deathAnimations => enemyManager.DeathAnimations;

        private IReadOnlyList<EnemyProjectile> enemyProjectiles => projectileManager.EnemyProjectiles;


        //camera info
        private Camera2D camera;
        private CameraController cameraController;
        private DialogueOverlay dialogueOverlay;
        private CutsceneDirector cutsceneDirector;

        //world map info
        private const int WorldWidth = 3000;
        private const int WorldHeight = 2000;
        private const int TileSize = 64;
        private GameMap gameMap => gameplaySession.Map;

        //ui info
        private Texture2D inventoryPanelTexture;
        private Texture2D questPanelTexture;
        private Texture2D spellsPanelTexture;
        private readonly Dictionary<string, Texture2D> shopUpgradeIcons = new();

        private Texture2D resourceBarsTexture;

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
        private Texture2D wavePreviewPanelTexture;

        private Texture2D startNextWaveButtonTexture;
        private Rectangle startNextWaveButtonBounds;
        private const float StartNextWaveButtonScale = 0.45f;

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
        private SoundEffect[] lichDeathSounds;
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
        //fonts info
        private SpriteFont boldpixels;

        //others
        private Random random = new Random();
        private readonly DeveloperModeController developerMode = new();
        private WorldDebugRenderer worldDebugRenderer;
        private DeveloperHudOverlay developerHudOverlay;

        public Game1()
        {
            Exiting += HandleGameExiting;
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
            Window.TextInput += HandleProfileTextInput;
            camera = new Camera2D();
            cameraController = new CameraController(
                camera,
                () => player == null ? playerStartPosition : player.Center);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            pixelTexture.SetData(new[] { Color.White });
            enemyFactory = new EnemyFactory(assetName => Content.Load<Texture2D>(assetName));
            enemyActionProcessor = new EnemyActionProcessor(enemyManager,projectileManager, worldEffectManager,enemyFactory);
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
            lichDeathSounds = new[]
            {
                Content.Load<SoundEffect>("Sounds/Enemies/Lich/lich_death1"),
                Content.Load<SoundEffect>("Sounds/Enemies/Lich/lich_death2"),
                Content.Load<SoundEffect>("Sounds/Enemies/Lich/lich_death3"),
                Content.Load<SoundEffect>("Sounds/Enemies/Lich/lich_death4")
            };
            manaCrystalCollectSound = Content.Load<SoundEffect>("Sounds/Mana/mana_collect");
            backgroundMusic = Content.Load<Song>("Music/ambient_forest");
            mainMenuMusic = Content.Load<Song>("Music/Main Menu/main_menu");
            campaignMapTexture = Content.Load<Texture2D>("Campaign/campaign_map1");
            missionNodeTexture = CreateCircleTexture(72);
            boldpixels = Content.Load<SpriteFont>("Sprite fonts/boldpixels");
            inventoryPanelTexture = Content.Load<Texture2D>("UI/UI_InventoryPanel");
            questPanelTexture = Content.Load<Texture2D>("UI/UI_QuestPanel");
            spellsPanelTexture = Content.Load<Texture2D>("UI/UI_SpellsPanel");
            bottomHudPanelTexture = Content.Load<Texture2D>("UI/bottom_hud_panel");
            resourceBarsTexture = Content.Load<Texture2D>("UI/resource_bars");
            panel9SliceTexture = Content.Load<Texture2D>("UI/nine slice 256x256 17gap/panel_9slice");
            inventorySlotTexture = Content.Load<Texture2D>("UI/panel_inventory_slot");
            uiCoinTexture = Content.Load<Texture2D>("UI/inventory icons/UI_coin");
            inventoryButtonTexture = Content.Load<Texture2D>("UI/ingame buttons/inventory-button");
            statsButtonTexture = Content.Load<Texture2D>("UI/ingame buttons/stats-button");
            shopButtonTexture = Content.Load<Texture2D>("UI/ingame buttons/shop-button");
            mapButtonTexture = Content.Load<Texture2D>("UI/ingame buttons/map-button");
            skillTreeButtonTexture = Content.Load<Texture2D>("UI/ingame buttons/skill-tree-button");
            settingsButtonTexture = Content.Load<Texture2D>("UI/ingame buttons/settings-button");
            soundVolumeButtonTexture = Content.Load<Texture2D>("UI/ingame buttons/sound-volume-button");
            startNextWaveButtonTexture = Content.Load<Texture2D>("UI/ingame buttons/start_next_wave");
            wavePreviewPanelTexture = Content.Load<Texture2D>("UI/wave_preview_panel");
            int buttonWidth = (int)(startNextWaveButtonTexture.Width * StartNextWaveButtonScale);
            int buttonHeight = (int)(startNextWaveButtonTexture.Height * StartNextWaveButtonScale);
            const int rightMargin = 20;
            const int bottomMargin = 20;
            startNextWaveButtonBounds = new Rectangle(
                GraphicsDevice.Viewport.Width - buttonWidth - rightMargin,
                GraphicsDevice.Viewport.Height - buttonHeight - bottomMargin,
                buttonWidth,
                buttonHeight);
            MissionDefinition initialMission = missionRuntime.Definition;
            LoadMissionMap(
                initialMission.MapFileName ?? DefaultMapFileName,
                MapThemes.Get(initialMission.MapThemeId),
                loadEnemySpawners: initialMission.Type == MissionType.Survival,
                loadMissionData: initialMission.Type == MissionType.Adventure);
            playerStartPosition = gameMap.PlayerSpawnPosition;
            activePlayerProfile = LoadOrCreateActivePlayerProfile();
            gameplaySession.SetPlayer(
                CreatePlayerForProfile(activePlayerProfile));
            AttachActivePlayerProfile();
            RestoreProfileInventoryResources();
            LoadShopUpgradeIcons();
            InitializeShopItems();
            InitializeGameplayPanels();
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = MusicVolume;

            if (MediaPlayer.State != MediaState.Stopped)
            {
                MediaPlayer.Stop();
            }

            PlayMusic(mainMenuMusic);

            screenManager.Register(new MainMenuScreen(this));
            screenManager.Register(new ProfileSelectionScreen(this));
            screenManager.Register(new OptionsScreen(this));
            screenManager.Register(new CampaignMapScreen(this));
            screenManager.Register(new GameplayScreen(this));
            screenManager.ChangeScreen(ScreenIds.MainMenu);
            pauseOverlay = new PauseOverlay(this);
            dialogueOverlay = new DialogueOverlay(this);
            cutsceneDirector = new CutsceneDirector(
                new CutsceneContext(cameraController, overlayManager, dialogueOverlay));
            worldDebugRenderer = new WorldDebugRenderer(pixelTexture);
            developerHudOverlay = new DeveloperHudOverlay(
                developerMode,
                gameplaySession,
                camera,
                cameraController,
                pixelTexture,
                boldpixels);
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

        private void UpdateMusicForgameFlowState()
        {
            if (gameFlowState == GameFlowState.Playing ||
                gameFlowState == GameFlowState.WaveIntermission)
            {
                PlayMusic(backgroundMusic);
            }
            else
            {
                // Main Menu & Options
                PlayMusic(mainMenuMusic);
            }
        }
        protected override void Update(GameTime gameTime)
        {
            UpdateProfileAutosave(gameTime);
            gameMap.Update(gameTime);

            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            UpdateSceneTransition(gameTime);
            UpdateMusicForgameFlowState();

            if (!transitionManager.IsActive)
                screenManager.Update(gameTime);

            previousKeyboard = keyboard;
            previousMouseState = mouse;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            screenManager.Draw(gameTime, _spriteBatch);
            overlayManager.Draw(gameTime, _spriteBatch);
            DrawSceneTransition();
            base.Draw(gameTime);
        }
        private static int ScaleUi(int value, float scale)
        {
            return Math.Max(1, (int)MathF.Round(value * scale));
        }
    }
}
