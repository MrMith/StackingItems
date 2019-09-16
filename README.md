# StackingItems
This is a server plugin for [SCP:SL.](https://store.steampowered.com/app/700330/SCP_Secret_Laboratory)

This plugin allows multiples of the same item to occupy the same slot.

This plugin makes it to when you extract you keep your items and ammo (anything over the 8th slot is spawned on the ground) 

This plugin also has its own 914 system that IS NOT compatible with 914-held-items.
## Install Instructions.
Put StackingItems.dll into sm_plugins folder.


## Config Options.
| Config Option              | Value Type      | Default Value | Description |
|   :---:                    |     :---:       |    :---:      |    :---:    |
| si_globaldict           | Dictionary      | Empty Dictionary           | A dictionary that stores custom stack sizes for items.|
| si_override_keycard     | Integer         | -1       | Override all stack sizes of keycards. Leave this default at -1 to disable. |
| si_disable              | Boolean         | false    | Disable the entire Stacking Items plugin. |
| si_extract              | Boolean         | true     | Should a player save items when they are rescued as D-class or Scientist.|
| si_globalstacksize      | Integer         | 1        | If this is set to anything over 1 this will change every stacksize for every item to this. si_globaldict values will still be set|
| si_914enable            | Boolean         | true     | Should player's inventory interact with 914. Set to false to disable any interaction with 914 (Items will be removed if disabled and generally be glitchy) |
| si_914handorinv         | Integer         | 0        | Integer value that decides whether or not to just pass items along or upgrade item in hand or the whole user's inventory. 0 = Transfer items no upgrades, 1 = just hand gets upgraded., 2 = Entire inventory gets upgraded |
| si_914itemstosave       | Integer Dictionary | 12,14,15,17,18,19   | Any itemtype in this list will NOT be consumed by 914 while in inventory or hand. |

All weapons (MicroHID, COM15, etc.) have a stack size of 1. After trying to design a system for keeping track of attachments and ammo I couldn't find anything was intuitive to use so this plugins will NEVER support weapons.

If you set the stack size below 1, it will behave like normal game behavior.

Example for si_globaldict: 25:5,14:4
a stack-size of 5 for fragmentation grenades and a stack-size of 4 for medkits.

https://github.com/Grover-c13/Smod2/wiki/Enum-Lists#itemtype

| Command(s)                 | Value Type      | Description                              |
|   :---:                    |     :---:       |    :---:                                 |
| si_version              | N/A             | Get the version of this plugin           |
| si_disable              | N/A             | Disables the entire StackingItems plugin.    |

## Known issues
8th slot doesn't stack.

If there are any problems, please private message me on Discord or submit a Github issue.
