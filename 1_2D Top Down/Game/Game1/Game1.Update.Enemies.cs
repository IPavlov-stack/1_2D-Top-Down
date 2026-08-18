using Microsoft.Xna.Framework;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        private void HandleEnemyDeath(Enemy enemy)
        {
            player.GainExperience(enemy.ExperienceReward);
            gameplaySession.PublishMissionEvent(
                new EnemyDefeatedMissionEvent(enemy.Definition.Id));

            Vector2 deathPosition = enemy.Bounds.Center.ToVector2();
            SpawnEnemyDrops(deathPosition);
        }
        private void TryFinishCurrentWave()
        {
            gameplaySession.TryFinishCurrentWave(
                enemyManager.HasFinishedSpawningWave,
                enemies.Count);
        }
        private void SpawnEnemyDrops(Vector2 enemyCenter)
        {
            TryDropCoin(enemyCenter);
            TryDropManaCrystal(enemyCenter);
        }

    }

}
