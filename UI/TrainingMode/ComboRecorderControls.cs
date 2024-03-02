using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using GrimbaHack.Modules;
using GrimbaHack.Modules.Combo;
using GrimbaHack.Modules.PlayerInput;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;

namespace GrimbaHack.UI.TrainingMode;

public class ComboRecorderControls : ModuleBase
{
    private static void OnToggleComboTracker(bool value)
    {
        ComboTrackerController.Instance.SetEnabled(value);
    }

    private static void OnPressRecordButton()
    {
        PlayerInputController.Record();
        ComboTracker.Instance.SetState(ComboTrackerState.Recording);
    }

    private static void OnPressPlaybackButton()
    {
        PlayerInputController.Playback();
        ComboTracker.Instance.SetState(ComboTrackerState.Idle);
    }

    private static void OnPressExportButton()
    {
        var combo = ComboTrackerController.Instance.GetCombo();
        var inputs = PlayerInputController.GetInputs();
        var exportClass = new ComboExport()
        {
            Combo = combo,
            Inputs = inputs,
        };
        var options = new JsonSerializerOptions { IncludeFields = true, WriteIndented = true};
        File.WriteAllText(Path.Join(BepInEx.Paths.GameRootPath, "output", "combo.json"),
        JsonSerializer.Serialize(exportClass, options));
    }

    public static void CreateUIControls(GameObject contentRoot)
    {
        var buttonGroup = UIFactory.CreateUIObject("PlayerInputButtonGroup", contentRoot);
        UIFactory.SetLayoutElement(buttonGroup);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(buttonGroup, false, false, true, true, padLeft: 25,
            spacing: 10, childAlignment: TextAnchor.MiddleLeft);

        CreateUIComboTrackerToggler(contentRoot);
        CreateUIRecordButton(buttonGroup);
        CreateUIPlaybackButton(buttonGroup);
        CreateUIExportButton(buttonGroup);

        UIFactory.SetLayoutElement(buttonGroup.gameObject, minHeight: 25, minWidth: 50);
    }

    private static void CreateUIExportButton(GameObject buttonGroup)
    {
        var exportButton = UIFactory.CreateButton(buttonGroup, "PlayerButtonExport", "Export");
        exportButton.OnClick += OnPressExportButton;
        UIFactory.SetLayoutElement(exportButton.GameObject, minHeight: 25, minWidth: 50);
    }

    private static void CreateUIPlaybackButton(GameObject contentRoot)
    {
        var playbackButton = UIFactory.CreateButton(contentRoot, "PlayerButtonPlaybackButton", "Playback");
        playbackButton.OnClick += OnPressPlaybackButton;
        UIFactory.SetLayoutElement(playbackButton.GameObject, minHeight: 25, minWidth: 50);
    }

    private static void CreateUIComboTrackerToggler(GameObject contentRoot)
    {
        var comboTrackerGroup = UIFactory.CreateUIObject("comboTrackerGroup", contentRoot);
        UIFactory.SetLayoutElement(comboTrackerGroup);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(comboTrackerGroup, false, false, true, true, padLeft: 25,
            spacing: 10, childAlignment: TextAnchor.MiddleLeft);
        UIFactory.CreateToggle(comboTrackerGroup, "comboTrackerToggle",
            out var comboTrackerToggle,
            out var comboTrackerToggleLabel);
        comboTrackerToggle.isOn = false;
        comboTrackerToggleLabel.text = "Enable Combo Tracker";
        comboTrackerToggle.onValueChanged.AddListener(new Action<bool>(OnToggleComboTracker));
        UIFactory.SetLayoutElement(comboTrackerToggle.gameObject, minHeight: 25, minWidth: 50);
    }

    private static void CreateUIRecordButton(GameObject contentRoot)
    {
        var recordButton = UIFactory.CreateButton(contentRoot, "PlayerButtonRecordButton", "Record");
        recordButton.OnClick += OnPressRecordButton;
        UIFactory.SetLayoutElement(recordButton.GameObject, minHeight: 25, minWidth: 75);
    }
}

public class ComboExport
{
    public List<string> Combo;
    public List<uint> Inputs;
}
