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
    private static Toggle comboTrackerToggle;

    private static void OnToggleComboTracker(bool value)
    {
        ComboTrackerController.Instance.SetEnabled(value);
    }

    private static void OnPressRecordButton()
    {
        PlayerInputController.PreRecord();
        ComboTracker.Instance.SetState(ComboTrackerState.Recording);
        UIComboTracker.Instance.SetStatusText("Recording");
    }

    private static void OnPressPlaybackButton()
    {
        PlayerInputController.Playback();
        ComboTracker.Instance.SetState(ComboTrackerState.Idle);
        UIComboTracker.Instance.SetStatusText("Playback");
    }

    private static void OnPressExportButton()
    {
        var combo = ComboTrackerController.Instance.GetCombo();
        var inputs = PlayerInputController.GetInputs();
        var character = ComboTrackerController.GetPlayerCharacter();
        if (combo == null || inputs == null || character == null)
        {
            return;
        }

        var PlayerPosition = PlayerInputController.GetPlayerStartPosition();
        var DummyPosition = PlayerInputController.GetDummyStartPosition();
        var exportClass = new ComboExport()
        {
            Combo = combo,
            Inputs = inputs,
            Character = character.GetCharacterName(),
            PlayerPosition = [PlayerPosition.x, PlayerPosition.y, PlayerPosition.z],
            DummyPosition = [DummyPosition.x, DummyPosition.y, DummyPosition.z],
        };
        var options = new JsonSerializerOptions { IncludeFields = true, WriteIndented = true };
        File.WriteAllText(Path.Join(BepInEx.Paths.GameRootPath, "output", "combo.json"),
            JsonSerializer.Serialize(exportClass, options));
    }

    private static void OnPressImportButton()
    {
        var filepath = Path.Join(BepInEx.Paths.GameRootPath, "output", "combo.json");
        if (!File.Exists(filepath))
        {
            return;
        }

        ComboTrackerController.Instance.SetEnabled(true);
        comboTrackerToggle.isOn = true;
        var fileContents = File.ReadAllText(filepath);
        var options = new JsonSerializerOptions { IncludeFields = true };
        var contents = JsonSerializer.Deserialize<ComboExport>(fileContents, options);
        if (ComboTracker.GetPlayerCharacter().GetCharacterName() != contents.Character)
        {
            Plugin.Log.LogInfo(
                $"Combo is not for this character. ({ComboTracker.GetPlayerCharacter().GetCharacterName()} vs {contents.Character})");
        }

        ComboTracker.Instance.SetCombo(contents.Combo);
        PlayerInputController.SetInputs(contents.Inputs);
        var playerPosition = new Vector3F()
            { x = contents.PlayerPosition[0], y = contents.PlayerPosition[1], z = contents.PlayerPosition[2] };
        var dummyPosition = new Vector3F()
            { x = contents.DummyPosition[0], y = contents.DummyPosition[1], z = contents.DummyPosition[2] };
        PlayerInputController.SetCharacterPositions(playerPosition, dummyPosition);
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
        CreateUIImportButton(buttonGroup);

        UIFactory.SetLayoutElement(buttonGroup.gameObject, minHeight: 25, minWidth: 50);
    }

    private static void CreateUIImportButton(GameObject contentRoot)
    {
        var exportButton = UIFactory.CreateButton(contentRoot, "ImportComboButton", "Import");
        exportButton.OnClick += OnPressImportButton;
        UIFactory.SetLayoutElement(exportButton.GameObject, minHeight: 25, minWidth: 50);
    }

    private static void CreateUIExportButton(GameObject buttonGroup)
    {
        var exportButton = UIFactory.CreateButton(buttonGroup, "ExportComboButton", "Export");
        exportButton.OnClick += OnPressExportButton;
        UIFactory.SetLayoutElement(exportButton.GameObject, minHeight: 25, minWidth: 50);
    }

    private static void CreateUIPlaybackButton(GameObject contentRoot)
    {
        var playbackButton = UIFactory.CreateButton(contentRoot, "PlaybackComboButton", "Playback");
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
            out comboTrackerToggle,
            out var comboTrackerToggleLabel);
        comboTrackerToggle.isOn = false;
        comboTrackerToggleLabel.text = "Enable Combo Tracker";
        comboTrackerToggle.onValueChanged.AddListener(new Action<bool>(OnToggleComboTracker));
        UIFactory.SetLayoutElement(comboTrackerToggle.gameObject, minHeight: 25, minWidth: 50);
    }

    private static void CreateUIRecordButton(GameObject contentRoot)
    {
        var recordButton = UIFactory.CreateButton(contentRoot, "RecordComboButton", "Record");
        recordButton.OnClick += OnPressRecordButton;
        UIFactory.SetLayoutElement(recordButton.GameObject, minHeight: 25, minWidth: 75);
    }
}

public class ComboExport
{
    public List<string> Combo;
    public List<uint> Inputs;
    public string Character;
    public List<FixedPoint> PlayerPosition;
    public List<FixedPoint> DummyPosition;
}