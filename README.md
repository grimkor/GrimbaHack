# GrimbaHack - Power Rangers Battle for the Grid Mod

## !!!Warning!!!
Using this mod may result in a ban from the game. Use at your own risk. Mods that can affects online play for other people have been restricted to:
* Training Mode
* Local Versus Mode
* Arcade Mode
* Story Mode(?)

I have also allowed these mods for passworded lobbies and direct matches however I am not sure if this is allowed by the game's terms of service. I am not responsible for any bans that may occur from using this mod.

## What is this?
This is a mod for Power Rangers Battle for the Grid on PC. This mod is a work in progress and is not yet complete. It is currently in a playable state, but there are still many things to be added and fixed.

## How do I install this?
1. Download the latest release from the [releases page](https://github.com/grimkor/GrimbaHack/releases) and keep aside for a later step.
2. Download the linked version of BepInEx for your operating system. BepInEx is a mod loader for Unity games. It is required for this mod to work:
    * [Windows](https://builds.bepinex.dev/projects/bepinex_be/665/BepInEx-Unity.IL2CPP-win-x64-6.0.0-be.665%2B6aabdb5.zip)
    * [Linux](https://builds.bepinex.dev/projects/bepinex_be/665/BepInEx-Unity.IL2CPP-linux-x64-6.0.0-be.665%2B6aabdb5.zip)
3. Extract the BepInEx zip file into the game's install directory. This is usually `C:\Program Files (x86)\Steam\steamapps\common\Power Rangers Battle for the Grid` on Windows or `~/.steam/steam/steamapps/common/Power Rangers Battle for the Grid` on Linux.
4. Extract the GrimbaHack zip file into the extracted BepInEx/plugins folder.
5. Run the game, the game will take a while when first running BepInEx to generate some files. Once the game is running, you can press F1 to open and close the mod menu.

## How do I use this?
The mod menu can be opened by pressing F1. The mod menu is a work in progress and is not yet complete but on the menu there are 3 options:

### Global
  #### Stage Select Override
Allows you to override the selected stage, selecting "Random" will select a random stage from the list which you can filter with the stage select button.
#### Pick Same Character Multiple Times
Allows you to select the same character multiple times in a row. **USING THIS ONLINE IS AGAINST THE TOS AND WILL GET YOU BANNED**.
#### Enable Camera Control
Allows you to enable a free moving camera that you can control with the keyboard. Press the `?` button to see the controls.

### BGM Player
Allows you to play the BGM from any stage in the game. Player will automatically progress to the next track when the current track ends. 
* **Play** - Plays the selected track.
* **Pause** - Pauses the currently playing track.
* **Song Select** - Selects the track to play.
* **<<** - Rewinds the currently playing track. or plays the previous track if the current track is less than 5 seconds in.
* **>>** - Plays the next track.

### Training Mode
These settings will only work in training mode currently:
#### Show Frame Data
Shows the frame data for the current character, please note that this calculation is still in testing and may not be accurate. To get the most accurate results you must be holding a movement button at the end of the move as the calculation is tied to the animation system.
### Collision Box Viewer
Allows you to see the following collision boxes in real-time:
* Hit
* Hurt
* Projectile
* Proximity
* Physics/Collision

To enable select the option in the training menu from the mod and you may need to quick reset the training to get it started. From there simple toggle it again to turn off.
### Simulation Speed Modifier
Modify the speed of the game, use the input field or the +/- buttons to change to the desired % and then press the "Set Speed" button to apply that speed. To turn it off either quick reset in training or delete the number in the field and it will auto-fill back to "100" for you to apply.

## To Fix
- [ ] Frame advantage is not resetting per hit when hit/blockstun doesn't expire.
- [ ] anti-cheat measure can lock a user from logging into lobbies after trying to connect to the game with a "cheat"-like mod enabled.
- [ ] Fix the frame data calculation to be more accurate.
- [ ] Sort options to have the right default state, some more should probably be off by default.
