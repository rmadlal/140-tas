### [Download](https://github.com/rmadlal/140-tas/releases/) ###

### Instructions ###  
There needs to be an input file for each level: `tas_script_0.txt` (main hub) through `tas_script_4.txt` (level 4), which should all be in `...\Steam\steamapps\common\140`.  
Each line's structure must be: `<# of frames>[,<input>[,<input>[...]]]` where `<input>` is `Left`, `Right` or `Jump`.  
Examples:  
`5,Right`  
`45,Jump,Right`  
`30` (here the player will wait for 30 frames.)  
`5,Jump,Left`  
`100,Left`  
You may start the line with a `#` for it to be a comment and it'll be ignored. Empty lines will also be ignored.

### Behavior ###  
The game automatically starts on the first possible frame, as if by holding jump or enter.  
The appropriate input file will be automatically loaded according to the current level.  
On gate activation, execution stops and resumes from the next line upon regaining control.  
It is highly recommended to install [Dalet's speedrun mod](https://github.com/Dalet/140-speedrun-timer/releases) after installing this, for much more efficient testing.

### Features ###
| Key Combo | Effect |
| :---: | --- |
| `Shift` + `F5` | Start TAS execution |
| `Shift` + `F6` | Stop TAS execution |
| `Shift` + `F7` | Display input info and player position |
