using epoch.db;
using GrimbaHack.Data;
using GrimbaHack.Utility;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem.Collections.Generic;
using nway.gameplay.ai;
using nway.gameplay.match;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace GrimbaHack.Modules;

public sealed class ExtraPushblockOptions : ModuleBase
{
    private ExtraPushblockOptions()
    {
    }

    public static ExtraPushblockOptions Instance { get; private set; }

    public ExtraPushblockOptionsBehaviour Behaviour { get; set; }

    private Dropdown _extraPushblockDropdown;

    static ExtraPushblockOptions()
    {
        Instance = new ExtraPushblockOptions();
        ClassInjector.RegisterTypeInIl2Cpp<ExtraPushblockOptionsBehaviour>();
        var go = new GameObject("ExtraPushblockOptionsBehaviour");
        go.hideFlags = HideFlags.HideAndDontSave;
        GameObject.DontDestroyOnLoad(go);
        Instance.Behaviour = go.AddComponent<ExtraPushblockOptionsBehaviour>();
        Instance.Enabled = false;
        OnEnterTrainingMatchActionHandler.Instance.AddPostfix(() => Instance.Enabled = Instance._enabled);
        OnEnterMainMenuActionHandler.Instance.AddCallback(() => Instance.Enabled = false);
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

    public void SetPercentToPushblock(int percentIndex)
    {
            if (percentIndex >= -1 && percentIndex <= 100)
            {
                Behaviour.SetPercentToPushblock(percentIndex);
            }
    }
}

public class ExtraPushblockOptionsBehaviour : MonoBehaviour
{
    public ExtraPushblockOptionsBehaviour()
    {
        OnSimulationInitializeActionHandler.Instance.AddPostfix(() =>
        {
            if (MatchManager.instance.matchType != MatchType.TRAINING)
            {
                ExtraPushblockOptions.Instance.Enabled = false;
                return;
            }

            // Here to stop bugs where multiple hits can freeze the dummy
            _recordController?.Reset();
            _recordController?.StopPlayback();
            if (ExtraPushblockOptions.Instance.Enabled)
            {
                ExtraPushblockOptions.Instance.Behaviour.Setup();
            }
        });

        OnApplyBlockAndHitStunActionHandler.Instance.AddCallback((bool isHitStun, int originalFrames) =>
        {
            if (!isHitStun && ExtraPushblockOptions.Instance.Enabled && originalFrames > 0)
            {
                _recordController.StopPlayback(); // Stop Playback here to prevent locking the recorder playback
                DummyIsStunned = true;
                var random = new Random();
                var _originalFrames = originalFrames - 2; // Compensate for delay and prevent nothing happening
                var pushblockFrame = PercentToPushblock == -1 // -1 means random
                    ? random.Next(0, _originalFrames)
                    : (int)(_originalFrames * (PercentToPushblock / 100f));
                _dummyRecorder.inputs = new List<CommandRecordingDriver.InputChange>();
                _dummyRecorder.inputs.Add(
                    new CommandRecordingDriver.InputChange(pushblockFrame == 0 ? 1 : pushblockFrame,
                        (uint)DUMMY_INPUTS._5S));
                _recordController.Reset();
                _recordController.StartPlayback();
            }
        });
    }

    private static CommandRecordingDriver _recordController;
    private static CommandRecordingDriver.RecordingState _dummyRecorder;
    private static Character _dummyCharacter;
    public static bool Ready;
    public static bool DummyIsStunned;
    public static int PercentToPushblock = 100;

    public void SetPercentToPushblock(int percent)
    {
        PercentToPushblock = percent;
    }

    private void Update()
    {
        if (Ready)

        {
            if (DummyIsStunned &&
                _dummyRecorder.currentFrame == _dummyRecorder.LatestFrame())
            {
                _recordController.StopPlayback();
            }

            if (!_dummyCharacter.InBlockStun && !_dummyCharacter.InHitStun)
            {
                DummyIsStunned = false;
            }
        }
    }

    public void Setup()
    {
        var sceneStartup = FindObjectOfType<SceneStartup>();

        var characters = FindObjectsOfType<Character>();

        foreach (var character in characters)
        {
            if (character.IsActiveCharacter)
            {
                if (character.team != 0)
                {
                    _dummyCharacter = character;
                }
            }
        }

        if (sceneStartup && _dummyCharacter)
        {
            _recordController = sceneStartup.GamePlay?.recorder;
            _dummyRecorder = _recordController?.dummyRecorder;
            if (_dummyRecorder != null && _recordController != null)
            {
                Ready = true;
            }
        }
    }
}
