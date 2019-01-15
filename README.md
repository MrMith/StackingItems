# StackingItems

## Install Instructions.
Put StackingItems.dll into sm_plugins folder.


## Config Options.
| Config Option              | Value Type      | Default Value | Description |
|   :---:                    |     :---:       |    :---:      |    :---:    |
| si_globaldict           | Dictionary      | Empty Dictionary           | Dictionary that keeps custom stacksizes for items. |
| si_override_keycard     | Integer         | -1       | Override all keycards to stack to this. Keep this to -1 to disable |
| si_disable              | Boolean         | false    | Disable the entire StackingItems plugin. |
| si_extract              | Boolean         | true     | Should players keep their items when they extract as D-Class or Scientist. |
| si_globalstacksize      | Integer         | 1        | If this is set to anything over 1 this will change every stacksize for every item to this. si_globaldict values will still be set|

All weapons (MicroHID,COM15,ect...) have their stacksize force set to 1 till I get around to fixing them.

If you set stacksize to anything below 1 it will act like its 1 aka normal game behavior.

Example for si_globaldict:25:5,14:4 would be frag grenade at 5 and medkit at 4 for stacksize.

https://github.com/Grover-c13/Smod2/wiki/Enum-Lists#itemtype


| Command(s)                 | Value Type      | Description                              |
|   :---:                    |     :---:       |    :---:                                 |
| si_version              | N/A             | Get the version of this plugin           |
| si_disable              | N/A             | Disables the entire StackingItems plugin.    |

## Known issues
8th slot doesn't stack.

If there is any issues please message me on discord or submit a issue.
