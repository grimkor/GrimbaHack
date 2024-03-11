# GrimbaHack - Power Rangers Battle for the Grid Mod

Please see the [Wiki](https://github.com/grimkor/GrimbaHack/wiki) for instructions on how to install and use the mod.

## TODO:
### JSON
[x] Export character used in combo
[x] Import combo
[] Add combo notation to import/export
[] Auto-split combo based on combo states?
[] See about auto-translating combo into numpad notation

### UI
[] Display whole combo notation on screen
[] Provide feedback for completed parts of combo
[] Combo selection/import interface
[x] Restrict combos to relevant character
[] Split out from Training UI Pane to Combo Trial one
[] Disable Frame Advantage mod while using Combo Recorder

### I hate that I get ideas
[] Playback takeover by listening to controller inputs like how I do the camera control
[x] Remember restart positions for combo trials
[] Figure out how to activate mouse control without F1, could be useful for better UI options

### Notes
- EventLayerSystem controls the selected elements on a given UI Layer (such as main menu > UI Settings pop-up layer)

#### Creating a menu

UIManager > UIPopupManager >
InitLoadedPopup - Adds UIPopup to the uiPopupAssetList by name
GetUIPopupAsset - Retrieves stored popup by name

UI Pages such as UISettings extend off UIWindow
UI Pages will typically have UIMenu and properties for the fields hanging off them
UISettings uses a UIStackedMenu for managing its extra pop-up windows and has a push method on that for rendering those windows

UIWindow has as AddButtonCallback which can used by passing a `(Action<ILayeredEventData>)` to it and will respond to inputs.

LayeredSelectables are nway's way of doing Buttons/Selectables and the extent on how they work with navigation logic is unknown.
Navigation mostly uses `LayeredSelectable.OnMoveOverrides` to navigate and trying to bypass them ends in broken navigation.


