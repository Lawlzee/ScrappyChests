# ScrappyChests

ScrappyChests is a Risk of Rain 2 mod that substitutes the loot from chests and mobs with scrap rather than standard items, requiring players to utilize printers to obtain standard items.

## Installation

[https://thunderstore.io/package/Lawlzee/ScrappyChests/](https://thunderstore.io/package/Lawlzee/ScrappyChests/)

## Gameplay

Below are links to various videos showcasing the mod.

- [Cap_](https://www.youtube.com/watch?v=_OH1NYyIlpc)
- [RayDans](https://www.youtube.com/watch?v=QDR5_SUpFIg)

## Replacement rules

Each replacement rules can be configured in the game menu `Settings > Mod Options > ScrappyChests`.

### Global Configuration

| Name | Default Value | Description |
| --- | --- | --- |
| Mod enabled | Enabled | Mod enabled |
| Preset | Default | See [Configuration presets](#configuration-presets) |

#### Configuration presets
Scrappy chests contains the following configuration presets:

##### Default
Default configuration for the mod:
- Interactables and monsters drop scrap instead of items
- Bonus printer spawn rate
- Void items can be found in printers and cauldrons
- Extra white and yellow cauldrons in the Bazaar and on the moon
- Interactables that costs lunar coins, cost white items instead

##### Easy
Default configuration except:
- Adaptive chests, legendary chests, void potential, void cradles and bosses drop items
- Lunar interactables cost lunar coins
- Red items are never replaced with scrap

##### All the choices
Default configuration except:
- Multishops, adaptive chests and void potentials drop items

##### Default printer spawn rates
Default configuration except:
- Default printer spawn rate

##### No printers
Default configuration except:
- Printers are disabled

#### Hardcore
Default configuration except:
- Every chests and enemies drops scrap
- Default printers spawn rate
- No void printers in void seeds
- No extra cauldrons
- No speed item bonus

#### V1
Equivalent to the 1.0 version of the mod.

Default configuration except:
- Default printers spawn rate
- No void printers in void seeds
- No extra cauldrons
- No void items in printers and cauldrons
- No speed items bonus
- Lunar interactables cost lunar coins
- Void cradles drops void items

#### V1.1
Equivalent to the 1.1 version of the mod.
        
Default configuration except:
- No void printers in void seeds
- No extra cauldrons
- Lunar interactables cost lunar coins

#### V1.2
Equivalent to the 1.2 version of the mod.
        
Default configuration except:
- Lunar interactables cost lunar coins (newt altars and lunar seers) 

#### Vanilla
Every changes are disabled.

### Chests rules

| Name | Default Value | Description |
| --- | --- | --- |
| Chest | Enabled | Chest will drop scrap instead of items |
| Multishop Terminal | Enabled | Multishop Terminal will drop scrap instead of items |
| Adaptive Chest | Enabled | Adaptive Chest will drop scrap instead of items |
| Shrine of Chance | Enabled | Shrine of Chance will drop scrap instead of items |
| Legendary Chest | Enabled | Legendary Chest will drop scrap instead of items |
| Void Potential | Enabled | Void Potential will drop scrap instead of items |
| Void Cradle | Enabled | Void Cradle will drop scrap instead of items |
| Lunar Pod | Disabled | Lunar Pod will drop Beads of Fealty instead of items |
| Lunar Bud | Disabled | Lunar Bud in the Bazaar Between Time will always sell Beads of Fealty |

### Printers rules

| Name | Default Value | Description |
| --- | --- | --- |
| White printer spawn multiplier | 1.5x | Controls the spawn rate of white printers. 0.0x = never. 1.0x = default spawn rate. 2.0x = 2 times more likely to spawn printers. |
| Green printer spawn multiplier | 2.5x | Controls the spawn rate of green printers. 0.0x = never. 1.0x = default spawn rate. 2.0x = 2 times more likely to spawn printers. |
| Red printer spawn multiplier | 3x | Controls the spawn rate of red printers. 0.0x = never. 1.0x = default spawn rate. 2.0x = 2 times more likely to spawn printers. |
| Yellow printer spawn multiplier | 3x | Controls the spawn rate of yellow printers. 0.0x = never. 1.0x = default spawn rate. 2.0x = 2 times more likely to spawn printers. |
| Minimum stage for red printers to spawn | 2 | Minimum stage for red printers to spawn. The vanilla value is 5. |
| Add void items to Printers | Enabled | Add void items to Printers |
| Add void printers to void seeds | Enabled | Add void printers to void seeds |
| Void seeds void printers weigth | 0.2 | Void seeds void printers weigth |
| Void seeds white void printers weigth | 60 | Void seeds white void printers weigth |
| Void seeds white void printers credit cost | 25 | Void seeds white void printers credit cost |
| Void seeds green void printers weigth | 36 | Void seeds green void printers weigth |
| Void seeds green void printers credit cost | 40 | Void seeds green void printers credit cost |
| Void seeds red void printers weigth | 4 | Void seeds red void printers weigth |
| Void seeds red void printers credit cost | 50 | Void seeds red void printers credit cost |

### Cauldrons rules

| Name | Default Value | Description |
| --- | --- | --- |
| Add void items to Cauldrons | Enabled | Add void items to Cauldrons |
| Add white Cauldrons to the Bazaar | Enabled | Add a white Cauldrons to the Bazaar |
| Add yellow Cauldrons to the Bazaar | Enabled | Add yellow Cauldrons to the Bazaar |
| Add yellow Cauldrons to the Moon | Enabled | Add a yellow Cauldrons to the Moon |
| Yellow Cauldrons Cost | 2 white items, 1 green item, 1 yellow item |  Cost to use the yellow Cauldrons |

### Items rules

| Name | Default Value | Description |
| --- | --- | --- |
| Rusted Key | Disabled | Lockboxes will drop scrap instead of items |
| Encrusted Key | Disabled | Encrusted Cache will drop scrap instead of items |
| Crashed Multishop | Disabled | Crashed Multishop will drop scrap instead of items |
| Trophy Hunters Tricorn | Disabled | Trophy Hunter's Tricorn will drop scrap instead of items |
| Speed items multiplier | 1.25x | Controls the spawn rate of speed items. 0.0x = never. 1.0x = default spawn rate. 2.0x = 2 times more likely to spawn speed items. |

### Mobs rules

| Name | Default Value | Description |
| --- | --- | --- |
| Boss | Enabled | Defeating a Boss will drop scrap instead of items |
| Alloy Worship Unit | Enabled | Alloy Worship Unit will drop scrap instead of items |
| Scavenger | Disabled | Scavenger will drop scrap instead of items |
| Elite Elder Lemurian | Disabled | The Elite Elder Lemurian in the hidden chamber of Abandoned Aqueduct will drop scrap instead of bands |
| Lunar coins drops | Enabled | Mobs will drop white scrap instead of Lunar coins |

### Costs rules

| Name | Default Value | Description |
| --- | --- | --- |
| Newt Altars uses white items | Enabled | Newt Altar uses white items as the activation cost instead of lunar coins |
| Lunar Seer uses white items | Enabled | Lunar Seer (dream) uses white items as the activation cost instead of lunar coins |
| Lunar pods uses white items | Enabled | Lunar pods uses white items as the activation cost instead of lunar coins |
| Lunar buds uses white items | Enabled | Lunar buds uses white items as the activation cost instead of lunar coins |
| Slab uses white items | Enabled | The Slab in the Bazaar Between Time uses white items as the activation cost instead of lunar coins |
| Unlocking artificier uses white items | Enabled | Unlocking artificier uses white items as the activation cost instead of lunar coins |
| Petting the frog uses white items | Enabled | Petting the frog uses white items as the activation cost instead of lunar coins |
| Shrine of order uses white items | Disabled | Shrine of order uses white items as the activation cost instead of lunar coins |

### Artifacts rules

| Name | Default Value | Description |
| --- | --- | --- |
| Relentless Doppelganger | Disabled | The Relentless Doppelganger from the Artifact of Vengeance will drop scrap instead of items |
| Artifact of Sacrifice | Enabled | When using the Artifact of Sacrifice, mobs will drop scrap instead of items |

### Waves rules

| Name | Default Value | Description |
| --- | --- | --- |
| Simulacrum | Enabled | The orb reward after each wave of Simulacrum will drop scrap instead of items |
| Void Fields | Enabled | The orb reward after each wave of the Void Fields will drop scrap instead of items |

### Tiers rules

| Name | Default Value | Description |
| --- | --- | --- |
| White item | Enabled | Replace white item drops with white scrap |
| Green item | Enabled | Replace green item drops with green scrap |
| Red item | Enabled | Replace red item drops with red scrap |
| Yellow item | Enabled | Replace yellow item drops with yellow scrap |
| Blue item | Enabled | Replace blue item drops with Beads of Fealty |
| Void tier 1 item | Enabled | Replace void tier 1 drops with white scrap |
| Void tier 2 item | Enabled | Replace void tier 2 drops with green scrap |
| Void tier 3 item | Enabled | Replace void tier 3 drops with red scrap |

### Eclipse
| Name | Default Value | Description |
| --- | --- | --- |
| Max completed Eclipse levels | 0 for all characters | Max eclipse level per survivor reached with Scappy Chests enabled |