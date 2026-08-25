using System;
using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    /// <summary>
    /// Owns the mutable runtime state shared by the active gameplay session.
    /// </summary>
    public sealed class GameplaySession
    {
        public GameplaySession(MissionDefinition initialMission)
        {
            ArgumentNullException.ThrowIfNull(initialMission);

            Mission = new MissionRuntime(initialMission);
        }

        public GameWorld World { get; } = new();

        public GameMap Map { get; } = new();

        public Player Player { get; private set; }

        public MissionRuntime Mission { get; }

        public GameFlowState FlowState { get; private set; } = GameFlowState.MainMenu;

        public void SetPlayer(Player player)
        {
            ArgumentNullException.ThrowIfNull(player);

            if (Player != null)
                throw new InvalidOperationException("The gameplay player is already initialized.");

            Player = player;
        }

        public void PreparePlayerForMission(Vector2 spawnPosition)
        {
            EnsurePlayerInitialized();

            Player.Health.Reset();
            Player.ResetDamageEffects();
            Player.Position = spawnPosition;
        }

        public void UpdatePlayer(GameTime gameTime, bool allowInput)
        {
            EnsurePlayerInitialized();

            Player.Update(
                gameTime,
                Map.WorldBounds,
                Map.IntersectsCollision,
                allowInput);
        }

        public void StartMission(MissionDefinition mission)
        {
            ArgumentNullException.ThrowIfNull(mission);

            Mission.Start(mission);
            World.ClearMissionObjects();
        }

        public void ChangeFlowState(GameFlowState state)
        {
            FlowState = state;
        }

        public bool PublishMissionEvent(MissionEvent missionEvent)
        {
            ArgumentNullException.ThrowIfNull(missionEvent);

            Mission.Publish(missionEvent);

            if (!Mission.IsCompleted)
                return false;

            ChangeFlowState(GameFlowState.MissionComplete);
            
            return true;
        }

        public bool TryFinishCurrentWave(bool hasFinishedSpawningWave, int activeEnemyCount)
        {
            if (!Mission.TryCompleteWave( hasFinishedSpawningWave, activeEnemyCount))
            {
                return false;
            }

            ChangeFlowState( Mission.IsCompleted
                    ? GameFlowState.MissionComplete
                    : GameFlowState.WaveIntermission);
            return true;
        }

        public bool TryStartNextWave()
        {
            int waveIndex = Mission.Waves.CurrentWave;

            if (waveIndex >= Mission.Definition.Waves.Count)
                return false;

            WaveDefinition wave = Mission.Definition.Waves[waveIndex];

            Mission.Waves.StartNextWave(wave.TotalEnemyCount);
            World.Enemies.StartSpawningWave(wave);
            ChangeFlowState(GameFlowState.Playing);
            return true;
        }

        private void EnsurePlayerInitialized()
        {
            if (Player == null)
            {
                throw new InvalidOperationException(
                    "The gameplay player has not been initialized.");
            }
        }
    }
}
