using System;
using epoch.db;
using GrimbaHack.Data;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem.Collections.Generic;
using nway.gameplay.ai;
using nway.gameplay.match;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;

namespace GrimbaHack.Modules;

public sealed class MultipleRecordingSlots : ModuleBase
{
    private MultipleRecordingSlots() {}
    
    public static MultipleRecordingSlots Instance { get; private set; }
    
    // public MultipleRecordingSlotsBehaviour Behaviour { get; set; }
    private static CommandRecordingDriver.RecordingState DummyRecorder;
    private List<List<CommandRecordingDriver.InputChange>> recordingSlots = new List<List<CommandRecordingDriver.InputChange>>();
    
    static MultipleRecordingSlots()
    {
        Instance = new MultipleRecordingSlots();
        var i = 0;
        while (i ++ < 5)
        {
            Instance.recordingSlots.Add(new List<CommandRecordingDriver.InputChange>());
        }
    }
    
    public static void CreateUIControls(GameObject contentRoot)
    {
        var multipleRecordingSlotsGroup = UIFactory.CreateUIObject("MultipleRecordingSlotsGroup", contentRoot);
        UIFactory.SetLayoutGroup<VerticalLayoutGroup>(multipleRecordingSlotsGroup, false, false, true, true, padLeft: 25,
            spacing: 10, childAlignment: TextAnchor.MiddleLeft);
        UIFactory.SetLayoutElement(multipleRecordingSlotsGroup);

        
        var slotNumber = 0;
        foreach (var recordingSlot in Instance.recordingSlots)
        {
            var slotNo = slotNumber;
            var slotLabel = slotNo + 1;
            var horizontalRecordingSlotGroup = UIFactory.CreateUIObject($"HorizontalRecordingSlotsGroup{slotLabel}", multipleRecordingSlotsGroup);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(horizontalRecordingSlotGroup, false, false, true, true, padLeft: 25,
                spacing: 10, childAlignment: TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(horizontalRecordingSlotGroup);

            var loadOne = UIFactory.CreateButton(horizontalRecordingSlotGroup, $"multipleRecordingsLoad{slotLabel}", $"Load Slot {slotLabel}");
            loadOne.OnClick = () =>
            {
                var dummyRecorder = SceneStartup.instance.GamePlay?.recorder?.dummyRecorder;
                if (dummyRecorder != null)
                {
                    dummyRecorder.inputs = new List<CommandRecordingDriver.InputChange>();
                    foreach (var input in Instance.recordingSlots[slotNo])
                    {
                        dummyRecorder.inputs.Add(new CommandRecordingDriver.InputChange(input.frame, input.input));
                    }
                }
            };
            var saveOne = UIFactory.CreateButton(horizontalRecordingSlotGroup, $"multipleRecordingsSave{slotLabel}", $"Save Slot {slotLabel}");
            saveOne.OnClick = () =>
            {
                var dummyRecorder = SceneStartup.instance.GamePlay?.recorder?.dummyRecorder;
                if (dummyRecorder != null)
                {
                    Instance.recordingSlots[slotNo] = new List<CommandRecordingDriver.InputChange>();
                    foreach (var input in dummyRecorder.inputs)
                    {
                        Instance.recordingSlots[slotNo].Add(new CommandRecordingDriver.InputChange(input.frame, input.input));
                    }
                }
            };
            UIFactory.SetLayoutElement(loadOne.GameObject, minHeight: 25, minWidth: 100);
            UIFactory.SetLayoutElement(saveOne.GameObject, minHeight: 25, minWidth: 100);
            slotNumber++;
        }
    }
}
