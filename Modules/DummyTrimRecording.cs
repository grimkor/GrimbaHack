using System;
using epoch.db;
using GrimbaHack.Data;
using GrimbaHack.Utility;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem.Collections.Generic;
using nway.gameplay.ai;
using nway.gameplay.match;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;

namespace GrimbaHack.Modules;

public sealed class DummyTrimRecording : ModuleBase
{
    private DummyTrimRecording( ) {}
    
    public static DummyTrimRecording Instance { get; private set; }
    
    public DummyTrimRecordingBehaviour Behaviour { get; set; }

    
    static DummyTrimRecording()
    {
        Instance = new DummyTrimRecording();
        ClassInjector.RegisterTypeInIl2Cpp<DummyTrimRecordingBehaviour>();
        var go = new GameObject("DummyTrimRecordingBehaviour");
        go.hideFlags = HideFlags.HideAndDontSave;
        GameObject.DontDestroyOnLoad(go);
        Instance.Behaviour = go.AddComponent<DummyTrimRecordingBehaviour>();
        Instance.Behaviour.enabled = false;
        OnEnterTrainingMatchActionHandler.Instance.AddCallback(() => Instance.Enabled = Instance._enabled);
        OnEnterMainMenuActionHandler.Instance.AddCallback(() => Instance.Behaviour.enabled = false);
    }

    private bool _enabled;
    public bool Enabled
    {
        get => Behaviour.enabled;
        set
        {
            if (value)
                Behaviour.Setup();
            Behaviour.enabled = value;
            _enabled = value;
        }
    }
    
    public static void CreateUIControls(GameObject contentRoot)
    {
        var dummyTrimRecordingGroup = UIFactory.CreateUIObject("DummyTrimRecordingGroup", contentRoot);
        UIFactory.SetLayoutElement(dummyTrimRecordingGroup);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(dummyTrimRecordingGroup, false, false, true, true, padLeft: 25,
            spacing: 10, childAlignment: TextAnchor.MiddleLeft);
        UIFactory.CreateToggle(dummyTrimRecordingGroup, "DummyTrimRecordingToggle", out var dummyTrimRecordingToggle,
            out var dummyTrimRecordingToggleLabel);
        dummyTrimRecordingToggle.isOn = false;
        dummyTrimRecordingToggleLabel.text = "Trim Dummy recordings to first action";
        dummyTrimRecordingToggle.onValueChanged.AddListener(new Action<bool>((value) => { Instance.Enabled = value; }));

        UIFactory.SetLayoutElement(dummyTrimRecordingToggle.gameObject, minHeight: 25, minWidth: 50);
    }
}

public class DummyTrimRecordingBehaviour : MonoBehaviour
{
    private static CommandRecordingDriver.RecordingState DummyRecorder;
    private static bool isRecording;

    public void Setup()
    {
        var sceneStartup = FindObjectOfType<SceneStartup>();

        if (sceneStartup)
        {
            DummyRecorder = sceneStartup.GamePlay?.recorder?.dummyRecorder;
        }
    }

    private void Update()
    {
        if (DummyRecorder != null)
        {
            if (isRecording && !DummyRecorder.isRecording)
            {
                var oldInputs = DummyRecorder.inputs;
                var index = 0;
                foreach (var input in oldInputs)
                {
                    if (input.input != (uint)DUMMY_INPUTS.EMPTY && input.input != (uint)DUMMY_INPUTS.RECORD)
                    {
                        break;
                    }
                    index++;
                }
                oldInputs.RemoveRange(0,index);
                DummyRecorder.inputs = new List<CommandRecordingDriver.InputChange>();

                var startingFrame = 0;
                foreach (var oldInput in oldInputs)
                {
                    if (startingFrame == 0)
                    {
                        startingFrame = oldInput.frame;
                    }
                    DummyRecorder.inputs.Add(new CommandRecordingDriver.InputChange(oldInput.frame - startingFrame, oldInput.input));
                }
            }
            isRecording = DummyRecorder.isRecording;
        }
    }
}