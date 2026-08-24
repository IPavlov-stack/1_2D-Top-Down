using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Tiled;

namespace _1_2D_Top_Down
{
    public partial class Game1
    {
        private const string DefaultMapFileName = "Maps/ForestMap.tmx";

        private void LoadMissionMap(
            string mapFileName,
            MapThemeDefinition mapTheme,
            bool loadEnemySpawners,
            bool loadMissionData = false)
        {
            gameMap.Load(
                Content,
                mapFileName,
                mapTheme,
                loadEnemySpawners,
                loadMissionData);
        }
    }
}
