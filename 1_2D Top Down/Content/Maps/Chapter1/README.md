# Chapter 1 maps

Open `Mission01.tmx` and `Mission02.tmx` directly in Tiled. Both maps are
finite orthogonal maps, use CSV layer data, and use 16x16 source tiles. The
game renders them at scale 4, so one tile occupies 64x64 world pixels.

## Required layer contract

- `Water`: base water tile layer. It may use the solid water tiles from
  `Chapter1WaterDetails.tsx`.
- `WaterDetails`: tile layer using `Chapter1WaterDetails.tsx`.
- `WaterDetails2`: animated tile layer using `Chapter1WaterDetails2.tsx`.
- `Ground`: base walkable-space layer. Like the original showcase map, it may
  mix `Chapter1Ground.tsx` with bridge/coast tiles from `Chapter1Water.tsx`.
- `Spots1`, `Spots2`: lower ground-detail layers.
- `MainSpace`, `MainSpace2`: coast, bridge, ground-transition, and main-space
  composition layers; these may mix ground and water-coast tilesets.
- `WaterLilies`, `Shadow`, `ObjectsUnderElevatedSpace`, `ElevatedSpace`:
  showcase-compatible water and elevated-terrain composition layers.
- `Spots3`, `Stairs`: upper ground-detail and stair layers.
- `GrassElements`, `GrassElements2`, `GrassElements3`: stacked grass-detail
  layers.
- `Lianas5`, `Lianas`, `Lianas2`, `Lianas3`, `Lianas4`: stacked liana layers.
- `Objects2`: lower scenery tile layer using `Chapter1Objects.tsx`.
- `Objects1`: main scenery tile layer using `Chapter1Objects.tsx`.
- `Objects3`, `Objects4`, `Objects5`: stacked foreground scenery tile layers
  using `Chapter1Objects.tsx`, drawn in that order over actors.
- `Reeds`: final foreground vegetation layer.
- `Props`: object layer. Place tiles from `Chapter1Objects.tsx` here.
- `Collisions`: rectangle object layer for solid obstacles.
- `EnemySpawners`: required by survival missions.
- `MissionObjects`: required by adventure missions.
- `MissionTriggers`: required by adventure missions.

Do not rename these layers. Keep the maps finite and the tile layers encoded
as CSV because the runtime Tiled loaders currently depend on that format.

## Props

Place decorative tiles from `Chapter1Objects.tsx` as tile objects in `Props`.
Optional string property `DrawMode` supports `Behind`, `YSort` (default), and
`Front`. Y-sorted props are drawn behind or in front of the player based on
their bottom edge.

Add matching rectangle objects to `Collisions` for trees, rocks, ruins, and
other solid decorations. Visual props do not create collision automatically.

Use `Objects2`, `Objects1`, and `Objects3`-`Objects5` when assembling multi-tile
scenery like the source showcase map. `Objects2` is the lowest scenery layer,
`Objects1` is the main layer, and `Objects3`, `Objects4`, and `Objects5` are
drawn in order over players and enemies
for canopies and other foreground pieces. Keep `Props` for independent tile
objects that need `DrawMode`, Y-sorting, or future interaction data.

## Optional decoration tilesets

Both templates also reference these Chapter 1 tilesets:

- `Chapter1Lianas`
- `Chapter1Spots`
- `Chapter1RockSpots`
- `Chapter1GrassStairs`
- `Chapter1WaterDetails`
- `Chapter1WaterDetails2`

The visual layers follow the original `Forest.tmx` showcase order. The runtime
resolves the correct tileset separately for every tile GID. A
single layer may therefore mix tilesets exactly like the original `Forest.tmx`
showcase; bridges and water coasts no longer need an artificial dedicated
runtime layer. Layers are drawn in the documented showcase order through
Objects1, then Props and actors, followed by foreground Objects3, Objects4,
Objects5, and Reeds. The animation
definitions supplied with `Chapter1WaterDetails2` are supported at runtime.

## Mission 1 (survival)

`Mission01.tmx` contains four placeholder point objects named `EnemySpawner`
in the `EnemySpawners` object layer. Move, add, or remove these points as the
map layout requires, but keep at least one. Wave definitions determine enemy
types and counts; the runtime distributes spawned enemies across these points.
Enemy spawners have no required visual tile and do not belong in `Props`.

## Mission 2 (adventure)

`Mission02.tmx` contains a placeholder point named `PlayerSpawn` in
`MissionObjects`. Move it to the desired start location, but do not remove it.

Additional preplaced enemies are points named `EnemySpawn` with a string
property named `EnemyType`. Mission trigger rectangles go in
`MissionTriggers`; their object name is the trigger ID consumed by mission
orchestration.
