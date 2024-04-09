# Combo Trial

Currently no combos by default, sorry not had the time to properly make combos.

## Creating Combos

1. Go to Training Mode with the character and dummy character you want in the trial.
2. Go into the pause menu > Press Y/Triangle.
3. Turn on Combo Trial and exit out (yes you might attack after un-pausing I CANT FIX IT I DONT KNOW WHY)
4. It should now show "Idle" as a recorder state on screen. You might need to turn off dummy info to see it.
5. Find your starting position for you and your dummy and press your bound Record button. It will change to "Set
   Starting Position"
6. Restart training, mode will change to "Recording" and from here recording has begun. If you mess up press restart
   again to restart recording.
7. Once you have a good recording press "Record" again to go back to idle. From here "Dummy Playback" button will
   playback the combo.
8. To save and export the combo to `output/` go back into the Combo Recorder menu and press the "Export" button.
9. From there you can do another recording by toggling the recorder off and back on.

## Adding Combos

Created combos need to be added to the character-specific folder in `BepInEx/plugins/combos/<character>`. These combos
will need to be ones created by the combo recorder.
Once they are added to that folder going into the combo trial character select screen will update and show new trials.

## Editing Trials Overlay

When a combo is exported it will be in this format:

```json
{
  "Title": "Advanced Concept: Plinked Jab",
  "Combo": [
    [
      {
        "Items": [
          "S21_YellowRanger_combat_high_light_1_0",
          "5L"
        ],
        "Repeat": 1
      },
      {
        "Items": [
          "S21_YellowRanger_combat_high_light_1_0",
          "5L"
        ],
        "Repeat": 1
      }
    ]
  ],
  "Inputs": [
  ],
  "Character": "S21_YellowRanger",
  "Dummy": "S01_GreenRanger",
  "PlayerPosition": [],
  "CharacterId": 2,
  "DummyId": 1,
  "SuperMeter": 2,
  "MzMeter": 12
}
```

| Property    |                                                                                  |
|-------------|----------------------------------------------------------------------------------|
| Title       | Name of the trial, trials are sorted alphabetically                              |
| Combo       | Array output of the combo trial and how it will be displayed, more on this below |
| Inputs      | Inputs recorded by the Combo Recorder, used for combo playback. DO NOT EDIT!     |
| Character   | Name of the character the trial is designed for. DO NOT EDIT!                    |
| Dummy       | Name of the dummy the trial is designed for. DO NOT EDIT!                        |
| CharacterId | HeroID of the character the trial is designed for. DO NOT EDIT!                  |
| DummyId     | HeroID of the dummy the trial is designed for. DO NOT EDIT!                      |
| SuperMeter  | Super meter the trial starts with/resets to.                                     |
| MzMeter     | Zord meter the trial starts with. Zord is not implemented yet please ignore.     |

### Combo Notation

The `Combo` section is an array (`[]`) of arrays where each array is a row on the combo trial. By default the Combo
Recorder will split the combo out by 15 inputs per row.
Each `Combo Item` in a row represents a part of the combo. The definition of each part of a combo item is below:

| Property |                                                                                                                                                                                    |
|----------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Items    | List of attacks in a group, groups are useful mostly for repeats. Each `Item` has 2 parts, separated by commas.                                                                     |
| Item[0]  | Attack ID the game uses, these are what the trial reads ON HIT to progress to the next trial step.                                                                                 |
| Item[1]  | The notation the trial will convert to buttons and directions. It accepts text but be sensible. `4` will convert to a left direction icon and `L` would convert to the light icon. |
| Repeat   | The amount of times to repeat this step. Creates the repeat brackets for all Notation/Ids in the group.                                                                            |

To edit this I recommend using a text editor that can help assist with JSON files. VSCode is probably the most accessible tool for this.

#### Repeat/Loop Example

```json
{
  "Items": [
    [
      "S21_YellowRanger_combat_jump_light_0",
      "9L"
    ],
    [
      "S21_YellowRanger_combat_jump_heavy_0",
      "9H"
    ],
    [
      "S21_YellowRanger_combat_sp_airspec_0",
      "9S"
    ]
  ],
  "Repeat": 99
}
```
Above example will show in-game as `(9LMS)x99`.
