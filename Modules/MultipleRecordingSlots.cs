using Il2CppSystem.Collections.Generic;
using nway.gameplay.ai;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;

namespace GrimbaHack.Modules;

public sealed class MultipleRecordingSlots : ModuleBase
{
    private MultipleRecordingSlots() {}
    
    public static MultipleRecordingSlots Instance { get; private set; }
    
    // public MultipleRecordingSlotsBehaviour Behaviour { get; set; }
    private List<List<CommandRecordingDriver.InputChange>> _recordingSlots = new List<List<CommandRecordingDriver.InputChange>>();
    
    static MultipleRecordingSlots()
    {
        Instance = new MultipleRecordingSlots();
        var i = 0;
        while (i ++ < 5)
        {
            Instance._recordingSlots.Add(new List<CommandRecordingDriver.InputChange>());
        }
    }
    
    public static void CreateUIControls(GameObject contentRoot)
    {
        var multipleRecordingSlotsGroup = UIFactory.CreateUIObject("MultipleRecordingSlotsGroup", contentRoot);
        UIFactory.SetLayoutGroup<VerticalLayoutGroup>(multipleRecordingSlotsGroup, false, false, true, true, padLeft: 25,
            spacing: 10, childAlignment: TextAnchor.MiddleLeft);
        UIFactory.SetLayoutElement(multipleRecordingSlotsGroup);

        
        var slotNumber = 0;
        foreach (var recordingSlot in Instance._recordingSlots)
        {
            var slotNo = slotNumber;
            var slotLabel = slotNo + 1;
            var horizontalRecordingSlotGroup = UIFactory.CreateUIObject($"HorizontalRecordingSlotsGroup{slotLabel}", multipleRecordingSlotsGroup);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(horizontalRecordingSlotGroup, false, false, true, true, padLeft: 25,
                spacing: 10, childAlignment: TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(horizontalRecordingSlotGroup);

            var LoadButton = UIFactory.CreateButton(horizontalRecordingSlotGroup, $"multipleRecordingsLoad{slotLabel}", $"Load Slot {slotLabel}");
            LoadButton.OnClick = () =>
            {
                var dummyRecorder = SceneStartup.instance.GamePlay?.recorder?.dummyRecorder;
                if (dummyRecorder != null)
                {
                    dummyRecorder.inputs = new List<CommandRecordingDriver.InputChange>();
                    foreach (var input in Instance._recordingSlots[slotNo])
                    {
                        dummyRecorder.inputs.Add(new CommandRecordingDriver.InputChange(input.frame, input.input));
                    }
                }
            };
            var saveButton = UIFactory.CreateButton(horizontalRecordingSlotGroup, $"multipleRecordingsSave{slotLabel}", $"Save Slot {slotLabel}");
            saveButton.OnClick = () =>
            {
                var dummyRecorder = SceneStartup.instance.GamePlay?.recorder?.dummyRecorder;
                if (dummyRecorder != null)
                {
                    Instance._recordingSlots[slotNo] = new List<CommandRecordingDriver.InputChange>();
                    foreach (var input in dummyRecorder.inputs)
                    {
                        Instance._recordingSlots[slotNo].Add(new CommandRecordingDriver.InputChange(input.frame, input.input));
                    }
                }
            };
            UIFactory.SetLayoutElement(LoadButton.GameObject, minHeight: 25, minWidth: 100);
            UIFactory.SetLayoutElement(saveButton.GameObject, minHeight: 25, minWidth: 100);
            slotNumber++;
        }
    }
}
