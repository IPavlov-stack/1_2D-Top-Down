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
            bool loadPortals,
            bool loadMissionData = false)
        {
            gameMap.Load(
                Content,
                mapFileName,
                environmentPropsAtlas,
                Content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>(
                    "Objects/Portal_orange-sheet"),
                EnvironmentScale,
                loadPortals,
                loadMissionData);
        }
    }
}
