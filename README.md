### [Download](https://github.com/rmadlal/140-tas/releases/) ###

### Instructions ###  
There needs to be an input file for each level: `tas_script_0.txt` (main hub) through `tas_script_4.txt` (level 4), which should all be in `...\Steam\steamapps\common\140`.  
Each line can be one of:  
* Inputs for some number of frames. Structure: `<# of frames>[,<input>[,<input>[...]]]` where `<input>` is `Left`, `Right` or `Jump`.  
* Some position to which the player will move towards until it is reached. Structure: `Pos,<X position>`. The player's X position can be viewed when `Shift` + `F7` is toggled.  
* A comment. If a line starts with `#` it'll be ignored.  
* An empty line.

Examples:  
`Pos,-503.4`  
`5,Jump,Right`  
`Pos,-497.7`  
`35` (here the player will wait for 35 frames.)  
`40,Left`

### Behavior ###  
The game automatically starts on the first possible frame, as if by holding jump or enter.  
The appropriate input file will be automatically loaded according to the current level.  
On gate activation, execution stops and resumes from the next line upon regaining control.  
It is highly recommended to install [Dalet's speedrun mod](https://github.com/Dalet/140-speedrun-timer/releases) after installing this, for much more efficient testing.  
It is also advised to lower the game's graphics quality by pressing `F10` (or launching the game in low quality on Legacy) to help minimize TAS desyncs.

### Features ###
| Key Combo | Effect |
| :---: | --- |
| `Shift` + `F5` | Start TAS execution |
| `Shift` + `F6` | Stop TAS execution |
| `Shift` + `F7` | Display input info and player position |
