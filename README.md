# ItemStack

## Install Instructions.
Put ItemStack.dll into sm_plugins folder.


## Config Options.
| Config Option              | Value Type      | Default Value | Description |
|   :---:                    |     :---:       |    :---:      |    :---:    |
| itemstack_globaldict       | Dictionary      | 3 for every item            | Dictionary that keeps stacksizes of all items in the game. |
| stack_override_keycard     | Integer         | -1       | Override all keycards to stack to this. Keep this to -1 to disable |
| stack_override_weapons     | Integer         | -1       | Override all keycards to stack to this. Keep this to -1 to disable |
| stack_disable              | Boolean         | false    | Disable the entire ItemStack plugin. |
| stack_extract              | Boolean         | true     | Should players keep their items when they extract. |

Stacksize for Tablet and MicroHID are at 1 because tablet doesn't play well with 079 and MicroHID can be abused by dropping and picking them up to recharge for free.

Example for itemstack_globaldict:25:5,14:4 would be frag grenade at 5 and medkit at 4 for stacksize.

https://github.com/Grover-c13/Smod2/wiki/Enum-Lists#itemtype


| Command(s)                 | Value Type      | Description                              |
|   :---:                    |     :---:       |    :---:                                 |
| stack_version              | N/A             | Get the version of this plugin           |
| stack_disable              | N/A             | Disables the entire ItemStack plugin.    |

## Known issues
8th slot doesn't stack.

If there is any issues please message me on discord or submit a issue.
