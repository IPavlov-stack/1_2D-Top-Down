using System;
using System.Collections.Generic;

namespace _1_2D_Top_Down
{
    public static class MapThemes
    {
        public const string OriginalForestId = "original-forest";
        public const string Chapter1Id = "chapter-1";

        public static MapThemeDefinition OriginalForest { get; } = new(
            OriginalForestId,
            0.25f,
            "Environment/EnvironmentGroundAtlas",
            "EnvironmentGround.tsx",
            "Environment/Water/tileset_water256x256",
            "tileset_water256x256.tsx",
            "Environment/EnvironmentPropsAtlas.xml",
            "EnvironmentProps.tsx",
            new Dictionary<int, string>
            {
                [27] = "prop_blue_banner",
                [28] = "prop_bushes_large",
                [29] = "prop_bushes_medium",
                [30] = "prop_bushes_small",
                [31] = "prop_campfire",
                [32] = "prop_castle_round",
                [33] = "prop_castle_square",
                [34] = "prop_flag",
                [35] = "prop_house",
                [36] = "prop_magic_stone_tower",
                [37] = "prop_red_banner",
                [38] = "prop_rock_01",
                [39] = "prop_rock_02",
                [40] = "prop_rock_03",
                [41] = "prop_rock_04",
                [42] = "prop_rock_05",
                [43] = "prop_tent",
                [44] = "prop_treasure_chest",
                [45] = "prop_tree_large",
                [46] = "prop_tree_medium",
                [47] = "prop_tree_small",
                [48] = "prop_tree_stump_short",
                [49] = "prop_tree_stump_tall",
                [50] = "prop_watchtower_short",
                [51] = "prop_watchtower_tall",
                [52] = "prop_well",
                [53] = "prop_windmill",
                [54] = "prop_wooden_barrel",
                [55] = "prop_wooden_bridge_horizontal",
                [56] = "prop_wooden_bridge_vertical",
                [57] = "prop_wooden_cart",
                [58] = "prop_wooden_fence_horizontal",
                [59] = "prop_wooden_fence_vertical"
            });

        public static MapThemeDefinition Chapter1 { get; } = new(
            Chapter1Id,
            2.5f,
            "Environment/Chapter1/Ground",
            "Chapter1Ground.tsx",
            "Environment/Chapter1/Water",
            "Chapter1Water.tsx",
            "Environment/Chapter1/Objects",
            "Chapter1Objects.tsx",
            16,
            16,
            underGroundLayers: new[]
            {
                new MapTileLayerDefinition(
                    "WaterDetails",
                    "Environment/Chapter1/WaterDetails",
                    "Chapter1WaterDetails.tsx"),
                new MapTileLayerDefinition(
                    "WaterDetails2",
                    "Environment/Chapter1/WaterDetails2",
                    "Chapter1WaterDetails2.tsx")
            },
            overGroundLayers: new[]
            {
                new MapTileLayerDefinition(
                    "Spots1", "Environment/Chapter1/Spots", "Chapter1Spots.tsx"),
                new MapTileLayerDefinition(
                    "Spots2", "Environment/Chapter1/Spots", "Chapter1Spots.tsx"),
                new MapTileLayerDefinition(
                    "MainSpace", "Environment/Chapter1/Ground", "Chapter1Ground.tsx"),
                new MapTileLayerDefinition(
                    "MainSpace2", "Environment/Chapter1/Ground", "Chapter1Ground.tsx"),
                new MapTileLayerDefinition(
                    "WaterLilies", "Environment/Chapter1/Objects", "Chapter1Objects.tsx"),
                new MapTileLayerDefinition(
                    "Shadow", "Environment/Chapter1/Ground", "Chapter1Ground.tsx"),
                new MapTileLayerDefinition(
                    "ObjectsUnderElevatedSpace", "Environment/Chapter1/Objects", "Chapter1Objects.tsx"),
                new MapTileLayerDefinition(
                    "ElevatedSpace", "Environment/Chapter1/Ground", "Chapter1Ground.tsx"),
                new MapTileLayerDefinition(
                    "Spots3", "Environment/Chapter1/RockSpots", "Chapter1RockSpots.tsx"),
                new MapTileLayerDefinition(
                    "Stairs", "Environment/Chapter1/GrassStairs", "Chapter1GrassStairs.tsx"),
                new MapTileLayerDefinition(
                    "GrassElements", "Environment/Chapter1/GrassStairs", "Chapter1GrassStairs.tsx"),
                new MapTileLayerDefinition(
                    "GrassElements2", "Environment/Chapter1/GrassStairs", "Chapter1GrassStairs.tsx"),
                new MapTileLayerDefinition(
                    "GrassElements3", "Environment/Chapter1/GrassStairs", "Chapter1GrassStairs.tsx"),
                new MapTileLayerDefinition(
                    "Lianas5", "Environment/Chapter1/Lianas", "Chapter1Lianas.tsx"),
                new MapTileLayerDefinition(
                    "Lianas", "Environment/Chapter1/Lianas", "Chapter1Lianas.tsx"),
                new MapTileLayerDefinition(
                    "Lianas2", "Environment/Chapter1/Lianas", "Chapter1Lianas.tsx"),
                new MapTileLayerDefinition(
                    "Lianas3", "Environment/Chapter1/Lianas", "Chapter1Lianas.tsx"),
                new MapTileLayerDefinition(
                    "Lianas4", "Environment/Chapter1/Lianas", "Chapter1Lianas.tsx"),
                new MapTileLayerDefinition(
                    "Objects2", "Environment/Chapter1/Objects", "Chapter1Objects.tsx"),
                new MapTileLayerDefinition(
                    "Objects1", "Environment/Chapter1/Objects", "Chapter1Objects.tsx")
            },
            foregroundLayers: new[]
            {
                new MapTileLayerDefinition(
                    "Objects3",
                    "Environment/Chapter1/Objects",
                    "Chapter1Objects.tsx"),
                new MapTileLayerDefinition(
                    "Objects4",
                    "Environment/Chapter1/Objects",
                    "Chapter1Objects.tsx"),
                new MapTileLayerDefinition(
                    "Objects5",
                    "Environment/Chapter1/Objects",
                    "Chapter1Objects.tsx"),
                new MapTileLayerDefinition(
                    "Reeds",
                    "Environment/Chapter1/Objects",
                    "Chapter1Objects.tsx")
            });

        private static readonly Dictionary<string, MapThemeDefinition> Definitions =
            new(StringComparer.OrdinalIgnoreCase)
            {
                [OriginalForest.Id] = OriginalForest,
                [Chapter1.Id] = Chapter1
            };

        public static MapThemeDefinition Get(string id)
        {
            if (Definitions.TryGetValue(id, out MapThemeDefinition definition))
                return definition;

            throw new KeyNotFoundException($"Unknown map theme '{id}'.");
        }
    }
}
