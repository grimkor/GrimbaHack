using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using GrimbaHack.Modules.Combo;
using GrimbaHack.Modules.ComboTrial;
using GrimbaHack.UI.TrainingMode;
using GrimbaHack.Utility;
using HarmonyLib;
using nway.gameplay;
using nway.gameplay.ai;
using nway.gameplay.match;
using UnityEngine;
using Object = UnityEngine.Object;

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
            Instance._statusOverlay.Enable = value;
        }
    }

    private InputRecorderBehaviour _recorder;
    private PlayerInputPlaybackBehaviour _playbackController;
    private ComboRecorderState _state = ComboRecorderState.Idle;
    private readonly List<string> _comboParts = new();
    private Character _player;
    private Character _dummy;
    private Vector3F _playerPosition;
    private Vector3F _dummyPosition;
    private InputSystem _inputSystem;
    private readonly LabelValueOverlayText _statusOverlay = new("Status", "Idle", new Vector3(240, 240, 1));
    private bool _enabled;

    static ComboRecorderManager()
    {
        Instance.Enabled = false;
        var go = new GameObject("grim_input_recorder");
        Object.DontDestroyOnLoad(go);
        Instance._recorder = go.AddComponent<InputRecorderBehaviour>();
        Instance._recorder.enabled = false;
        Instance._playbackController = go.AddComponent<PlayerInputPlaybackBehaviour>();
        Instance._playbackController.enabled = false;

        OnEnterMainMenuActionHandler.Instance.AddCallback(() => Instance.Enabled = false);
        OnEnterTrainingMatchActionHandler.Instance.AddPostfix(() =>
        {
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
            if (Instance._state != ComboRecorderState.RecordingRunning) return;
            if (info.isBlocked) return;
            Instance._comboParts.Add(info.attackName);
        });
    }

    private void SetState(ComboRecorderState state)
    {
        Plugin.Log.LogInfo($"SetState: {state}");
        switch (state)
        {
            case ComboRecorderState.Idle:
                StopPlayback();
                StopRecording();
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
        Instance._recorder.enabled = true;
    }

    private void StopRecording()
    {
        Instance._recorder.enabled = false;
    }

    private void TogglePlayback()
    {
        Instance.SetState(
            Instance._state == ComboRecorderState.PlaybackRunning
                ? ComboRecorderState.Idle
                : ComboRecorderState.PlaybackStart
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
        Instance._playbackController.Playback(Instance._inputSystem, Instance.GetRecordedInputs());
    }

    private void StopPlayback()
    {
        Instance._playbackController.enabled = false;
    }

    public List<uint> GetRecordedInputs()
    {
        return Instance._recorder.Inputs;
    }

    public List<string> GetComboParts()
    {
        return Instance._comboParts;
    }

    public void ExportCombo()
    {
        var combo = Instance.GetComboParts();
        var inputs = Instance.GetRecordedInputs();
        var drivers = MatchManager.Get.CombatDriver;
        var trainingMeterDriver = drivers.FindExtension<TrainingMeterDriver>();
        if (combo == null || inputs == null || Instance._player == null || trainingMeterDriver == null)
        {
            return;
        }

        List<List<ComboItem>> convertedCombo =
            new List<List<ComboItem>>()
            {
                combo.Select(s =>
                {
                    Plugin.Log.LogInfo($"combo part: {s} | {ComboQuickConverter.ConvertGia(s)}");
                    return new ComboItem()
                        { Ids = new() { s }, Notation = new() { ComboQuickConverter.ConvertGia(s) }, Repeat = 1 };
                }).ToList()
            };
        var exportClass = new ComboExport()
        {
            Title = $"{Instance._player.GetCharacterName()}_{Time.frameCount}",
            Combo = convertedCombo,
            Inputs = inputs,
            Character = Instance._player.GetCharacterName(),
            CharacterId = Instance._player.GetHeroIndex(),
            Dummy = Instance._dummy.GetCharacterName(),
            DummyId = Instance._dummy.GetHeroIndex(),
            PlayerPosition = new List<FixedPoint> { Instance._playerPosition.x, Instance._playerPosition.y, Instance._playerPosition.z },
            DummyPosition = new List<FixedPoint> { Instance._dummyPosition.x, Instance._dummyPosition.y, Instance._dummyPosition.z },
            SuperMeter = trainingMeterDriver.LocalPlayerSuperRefillLevel,
            MzMeter = trainingMeterDriver.LocalPlayerMZMeterLevel
        };
        var options = new JsonSerializerOptions { IncludeFields = true, WriteIndented = true };
        File.WriteAllText(Path.Join(BepInEx.Paths.GameRootPath, "output", $"{Instance._player.GetCharacterName()}_{Time.frameCount}.json"),
            JsonSerializer.Serialize(exportClass, options));
    }

    private void GetActiveCharacters()
    {
        var characters = Object.FindObjectsOfType<Character>();
        foreach (var character in characters)
        {
            if (character.IsActiveCharacter)
            {
                if (character.team == 0)
                {
                    Instance._player = character;
                    Instance._inputSystem = character.GetCharacterTeam().GetInputSystem();
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
