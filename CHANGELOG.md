## 1.3
- Added configuration presets
- Changed the minimum stage count for red printers to spawn from 5 to 2 (configurable)
- Every interactable that previously cost Lunar coins now costs white items (configurable)
- Replaced Lunar coin drops to white scrap (configurable)
- Added a visual indicator of the highest eclipse level completed for each character with the mod enabled on the character select screen.
- The difficulty icons now contain a scrap icon when the mod is enabled.
- Fixes a bug where all 3 yellow cauldrons would always spawn on the moon.
- New mod icon

## 1.2.2
- Fixed a bug where additional printers would spawn at coordinates (0, 0, 0) when void seeds were present on the map.

## 1.2.1
- Fixed yellow cauldron cost
- Better yellow cauldron cost label

## 1.2
- Added void printers to void seeds (configurable)
    - Void seeds have a ~50% chance to include void printers (configurable)
    - Void printers have a 60% chance to be white, 36% to be green, and 4% to be red (configurable)
    - The credit cost is also configurable
- Added white Cauldrons to the Bazaar (configurable).
- Added yellow Cauldrons
    - They can be found in the Bazaar (configurable).
    - They can be found on the moon (configurable).
    - They cost 2 white items, 1 green item, and 1 yellow item to activate (configurable) 
- Encrusted Key caches now drop scrap by default (configurable).
- Newt Altar now uses white items as the activation cost instead of lunar coins by default (configurable).
- Lunar Seer (dream) now uses white items as the activation cost instead of lunar coins (configurable).

## 1.1.2
- Updated documentation
- Bug fixes

## 1.1.1
- Tweaked the `Speed items multiplier` rule
- Printer spawn rates are now configurable independently for each item tier

## 1.1.0
**Added replacement rules:**

|Category|Name|Default Value|Description|
|---|---|---|---|
|Chests|Void Cradle|Enabled|Void Cradle will drop scrap instead of items|
|Printers|Printer spawn multiplier|1.5x|Controls the spawn rate of printers. 0.0x = never. 1.0x = default spawn rate. 2.0x = 2 times more likely to spawn printers.|
|Printers|Add void items to Printers|Enabled|Add void items to Printers|
|Printers|Add void items to Cauldrons|Enabled|Add void items to Cauldrons|
|Items|Speed items multiplier|1.25x|Controls the spawn rate of speed items. 0.0x = never. 1.0x = default spawn rate. 2.0x = 2 times more likely to spawn speed items.|

## 1.0.0

- First release