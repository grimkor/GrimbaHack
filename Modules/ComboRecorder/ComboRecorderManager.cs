using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using BepInEx;
using GrimbaHack.Modules.Combo;
using GrimbaHack.Modules.ComboTrial;
using GrimbaHack.Modules.ComboTrial.UI;
using GrimbaHack.UI.TrainingMode;
using GrimbaHack.Utility;
using HarmonyLib;
using nway.gameplay;
using nway.gameplay.ai;
using nway.gameplay.match;
using UnityEngine;

namespace GrimbaHack.Modules.ComboRecorder;

public class ComboRecorderManager
{
    public static ComboRecorderManager Instance = new();

    private ComboRecorderManager()
    {
    }

    public bool Enabled
    {
        get => Instance._enabled;
        set
        {
            Instance._enabled = value;
            Instance._statusOverlay.Enabled = value;
            if (!value)
            {
                Instance.SetState(ComboRecorderState.Idle);
                ComboTrialManager.Instance.Teardown();
            }
        }
    }

    private InputRecorderBehaviour _recorder;
    private PlayerInputPlaybackController _playbackController;
    private ComboRecorderState _state = ComboRecorderState.Idle;
    private readonly List<string> _comboParts = new();
    private Character _player;
    private Character _dummy;
    private Vector3F _playerPosition;
    private Vector3F _dummyPosition;
    private readonly LabelValueOverlayText _statusOverlay = new("Status", "Idle", new Vector3(240, 240, 1));
    private bool _enabled;
    private bool _IsPaused;

    static ComboRecorderManager()
    {
        Instance._recorder = InputRecorderBehaviour.Instance;
        Instance._recorder.SetEnabled(false);
        Instance._statusOverlay.Enabled = false;
        Instance._playbackController = PlayerInputPlaybackController.Instance;

        OnEnterMainMenuActionHandler.Instance.AddCallback(() => Instance.Enabled = false);
        OnUITrainingOptionsOnShowActionHandler.Instance.AddPostfix((_) => Instance._IsPaused = true);
        OnEnterTrainingMatchActionHandler.Instance.AddPostfix(() =>
        {
            if (!Instance.Enabled) return;
            if (Instance._IsPaused)
            {
                Instance._IsPaused = false;
                return;
            }

            Instance.GetActiveCharacters();
            switch (Instance._state)
            {
                case ComboRecorderState.RecordingStart:
                    Instance.SetState(ComboRecorderState.RecordingRunning);
                    break;
                case ComboRecorderState.RecordingRunning:
                    Instance.SetState(ComboRecorderState.RecordingRunning);
                    break;
                case ComboRecorderState.PlaybackStart:
                    Instance.SetState(ComboRecorderState.PlaybackRunning);
                    break;
                default:
                    Instance.SetState(ComboRecorderState.Idle);
                    break;
            }
        });
        OnArmorTakeDamageCallbackHandler.Instance.AddPostfix(info =>
        {
            if (!Instance._enabled) return;
            if (Instance._state != ComboRecorderState.RecordingRunning) return;
            if (info.isBlocked) return;
            if (info.attackName is "" or null) return;
            Instance._comboParts.Add(info.attackName);
        });
    }

    private void SetState(ComboRecorderState state)
    {
        switch (state)
        {
            case ComboRecorderState.Idle:
                StopPlayback();
                StopRecording();
                Instance._statusOverlay.Enabled = Instance._enabled;
                Instance._statusOverlay.Value = "Idle";
                break;
            case ComboRecorderState.RecordingStart:
                StopPlayback();
                Instance.RecordCharacterPositions();
                Instance._statusOverlay.Value = "Starting Position Set";
                break;
            case ComboRecorderState.RecordingRunning:
                Instance.SetStartingPositions();
                StartRecording();
                Instance._statusOverlay.Value = "Recording";
                break;
            case ComboRecorderState.PlaybackStart:
                StopRecording();
                MatchManager.Get.CombatDriver.FindExtension<MatchResetDriver>().ResetTrainingBattle();
                var combo = Instance.GenerateExportCombo();
                if (combo == null) break;
                ComboTrialManager.Instance.SetupComboTrial(combo);
                break;
            case ComboRecorderState.PlaybackRunning:
                StopRecording();
                StartPlayback();
                Instance._statusOverlay.Value = "Playback";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }

        Instance._state = state;
    }

    private void ToggleRecording()
    {
        Instance.SetState(
            Instance._state == ComboRecorderState.RecordingRunning
                ? ComboRecorderState.Idle
                : ComboRecorderState.RecordingStart
        );
    }

    private void StartRecording()
    {
        Instance._comboParts.Clear();
        Instance._recorder.Clean();
        Instance._recorder.SetEnabled(true);
    }

    private void StopRecording()
    {
        Instance._recorder.SetEnabled(false);
    }

    private void TogglePlayback()
    {
        Instance.SetState(
            ComboRecorderState.PlaybackStart
        );
    }

    private void StartPlayback()
    {
        if (Instance.GetRecordedInputs().Count <= 0)
        {
            SetState(ComboRecorderState.Idle);
            return;
        }

        Instance.SetStartingPositions();
        Instance._playbackController.Playback(Instance.GetRecordedInputs());
    }

    private void StopPlayback()
    {
        Instance._playbackController.Stop();
    }

    public List<int> GetRecordedInputs()
    {
        return Instance._recorder.Inputs;
    }

    public List<string> GetComboParts()
    {
        return Instance._comboParts;
    }

    private ComboExport GenerateExportCombo()
    {
        var combo = Instance.GetComboParts();
        var inputs = Instance.GetRecordedInputs();
        var drivers = MatchManager.Get.CombatDriver;
        var trainingMeterDriver = drivers.FindExtension<TrainingMeterDriver>();
        if (combo == null || inputs == null || Instance._player == null || trainingMeterDriver == null)
        {
            return null;
        }

        List<List<ComboItem>> convertedCombo = ConvertToComboExport(combo);
        var comboExport = new ComboExport()
        {
            Title = $"{Instance._player.GetCharacterName()}_{Time.frameCount}",
            Combo = convertedCombo,
            Inputs = inputs,
            Character = Instance._player.GetCharacterName(),
            CharacterId = Instance._player.GetHeroIndex(),
            Dummy = Instance._dummy.GetCharacterName(),
            DummyId = Instance._dummy.GetHeroIndex(),
            PlayerPosition = new List<FixedPoint>
                { Instance._playerPosition.x, Instance._playerPosition.y, Instance._playerPosition.z },
            DummyPosition = new List<FixedPoint>
                { Instance._dummyPosition.x, Instance._dummyPosition.y, Instance._dummyPosition.z },
            SuperMeter = trainingMeterDriver.LocalPlayerSuperRefillLevel,
            MzMeter = trainingMeterDriver.LocalPlayerMZMeterLevel
        };
        return comboExport;
    }

    private List<List<ComboItem>> ConvertToComboExport(List<string> combo)
    {
        var convertedCombo = new List<List<ComboItem>>() { new() };
        for (var i = 0; i < combo.Count; i++)
        {
            int lastRowCount = convertedCombo.Last().SelectMany(x => x.GetNotation()).Where(x => x != "").ToList()
                .Count;
            if (lastRowCount != 0 && lastRowCount % 15 == 0)
            {
                convertedCombo.Add(new());
            }

            var row = convertedCombo.Last();
            row.Add(new ComboItem
            {
                Items = new()
                    { new() { combo[i], ComboQuickConverter.ConvertInput(combo[i], Instance._player.GetHeroIndex()) } },
                Repeat = 1
            });
        }

        return convertedCombo;
    }

    public void ExportCombo()
    {
        var options = new JsonSerializerOptions
            { IncludeFields = true, WriteIndented = true, AllowTrailingCommas = true };
        var combo = Instance.GenerateExportCombo();
        if (combo == null) return;
        File.WriteAllText(
            Path.Join(Path.Join(Paths.PluginPath, "combos", "__EXPORT"),
                $"{Instance._player.GetCharacterName()}_{Time.frameCount}.json"),
            JsonSerializer.Serialize(combo, options));
    }

    private void GetActiveCharacters()
    {
        foreach (var character in SceneStartup.Get.GamePlay._playerList)
        {
            if (character.IsActiveCharacter)
            {
                if (character.team == 0)
                {
                    Instance._player = character;
                }
                else
                {
                    Instance._dummy = character;
                }
            }
        }
    }

    private void RecordCharacterPositions()
    {
        Instance._playerPosition = Instance._player.GetPosition();
        Instance._dummyPosition = Instance._dummy.GetPosition();
    }

    private void SetStartingPositions()
    {
        Instance._player.SetPosition(Instance._playerPosition);
        Instance._dummy.SetPosition(Instance._dummyPosition);
    }

    [HarmonyPatch(typeof(CommandRecordingDriver), nameof(CommandRecordingDriver.ToggleRecordingState))]
    public class OverrideToggleRecording
    {
        static bool Prefix()
        {
            if (Instance.Enabled)
            {
                Instance.ToggleRecording();
            }

            return !Instance.Enabled;
        }
    }

    [HarmonyPatch(typeof(CommandRecordingDriver), nameof(CommandRecordingDriver.TogglePlaybackState))]
    public class OverrideTogglePlayback
    {
        static bool Prefix()
        {
            if (Instance.Enabled)
            {
                Instance.TogglePlayback();
            }

            return !Instance.Enabled;
        }
    }
}

public enum ComboRecorderState
{
    Idle,
    RecordingStart,
    PlaybackStart,
    PlaybackRunning,
    RecordingRunning
}
