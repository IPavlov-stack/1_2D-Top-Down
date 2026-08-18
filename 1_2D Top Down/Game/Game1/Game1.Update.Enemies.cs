using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        private void HandleEnemyDeath(Enemy enemy)
        {
            player.GainExperience(enemy.ExperienceReward);

            Vector2 deathPosition = enemy.Bounds.Center.ToVector2();
            SpawnEnemyDrops(deathPosition);
        }
        private void TryFinishCurrentWave()
        {
            if (missionRuntime.TryCompleteWave(
                    enemyManager.HasFinishedSpawningWave,
                    enemies.Count))
            {
                gameFlowState = missionRuntime.IsCompleted
                    ? GameFlowState.MissionComplete
                    : GameFlowState.WaveIntermission;
            }
        }
        private void SpawnEnemyDrops(Vector2 enemyCenter)
        {
            TryDropCoin(enemyCenter);
            TryDropManaCrystal(enemyCenter);
        }

    }

}
