using System;
using System.Linq;
using epoch.db;
using GrimbaHack.Data;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem.Collections.Generic;
using nway.gameplay.ai;
using nway.gameplay.match;
using nway.gameplay.simulation;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;

namespace GrimbaHack.Modules;

public sealed class ExtraPushblockOptions : ModuleBase
{
    private ExtraPushblockOptions()
    {
    }

    public static ExtraPushblockOptions Instance { get; private set; }

    public ExtraPushblockOptionsBehaviour Behaviour { get; set; }

    private static Dropdown _extraPushblockDropdown;
    private static Toggle _extraPushblockToggle;

    public bool Enabled
    {
        get => Behaviour.enabled;
        set
        {
            var valueToSet = value && MatchManager.instance.matchType == MatchType.TRAINING;

            Behaviour.enabled = valueToSet;
            if (_extraPushblockToggle)
                _extraPushblockToggle.isOn = valueToSet;
            if (valueToSet)
                Behaviour.Setup();
        }
    }

    public void SetPercentToPushblock(int percentIndex)
    {
        if (percentIndex >= 0 && percentIndex <= Global.PercentOptions.Count - 1)
        {
            var percent = Global.PercentOptions[percentIndex].Value;
            if (percent >= -1 && percent <= 100)
            {
                Behaviour.SetPercentToPushblock(percent);
            }
        }
    }

    static ExtraPushblockOptions()
    {
        Instance = new ExtraPushblockOptions();
        ClassInjector.RegisterTypeInIl2Cpp<ExtraPushblockOptionsBehaviour>();
        var go = new GameObject("ExtraPushblockOptionsBehaviour");
        go.hideFlags = HideFlags.HideAndDontSave;
        GameObject.DontDestroyOnLoad(go);
        Instance.Behaviour = go.AddComponent<ExtraPushblockOptionsBehaviour>();
        Instance.Enabled = false;
    }

    public static void CreateUIControls(GameObject contentRoot)
    {
        GameObject extraPushblockGroup = UIFactory.CreateUIObject("ExtraPushblockOptionsGroup", contentRoot);
        UIFactory.SetLayoutElement(extraPushblockGroup);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(extraPushblockGroup, false, false, true, true, padLeft: 25,
            padRight: 0, spacing: 10, childAlignment: TextAnchor.MiddleLeft);

        // CREATE TOGGLE
        UIFactory.CreateToggle(extraPushblockGroup, "ExtraPushblockOptionsToggle", out _extraPushblockToggle,
            out Text extraPushblockToggleText, checkHeight: 20, checkWidth: 20);
        _extraPushblockToggle.onValueChanged.AddListener(new Action<bool>(enabled =>
        {
            var canEnable = enabled && MatchManager.instance.matchType == MatchType.TRAINING;
            _extraPushblockDropdown.gameObject.active = canEnable;
            Instance.Enabled = canEnable;
        }));

        extraPushblockToggleText.text = "Enable Enhanced Pushblock";

        // CREATE DROPDOWN
        UIFactory.CreateDropdown(extraPushblockGroup, "ExtraPushblockOptionsDropdown",
            out _extraPushblockDropdown,
            "Random",
            14,
            i => { Instance.SetPercentToPushblock(i); },
            Global.PercentOptions.ToArray().Select(stage => stage.Label).ToArray()
        );

        // LAYOUT ELEMENTS
        UIFactory.SetLayoutElement(_extraPushblockToggle.gameObject, minWidth: 50, minHeight: 25);
        UIFactory.SetLayoutElement(_extraPushblockDropdown.gameObject, minWidth: 120, minHeight: 25);
        _extraPushblockToggle.isOn = false;
        _extraPushblockDropdown.gameObject.active = false;
    }
}

public class ExtraPushblockOptionsBehaviour : MonoBehaviour
{
    private static CommandRecordingDriver _recordController;
    private static CommandRecordingDriver.RecordingState _dummyRecorder;
    private static Character _dummyCharacter;
    public static bool Ready;
    public static bool DummyIsStunned;
    public static int PercentToPushblock = -1;

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

    [HarmonyPatch(typeof(SimulationManager), nameof(SimulationManager.Initialize))]
    public static class SimulationManagerInitializePatch
    {
        public static void Postfix()
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
        }
    }

    [HarmonyPatch(typeof(Character), nameof(Character.ApplyBlockAndHitStun))]
    public class PatchApplyBlockAndHitStun
    {
        public static void Prefix(bool isHitStun, int originalFrames)
        {
            if (!isHitStun && ExtraPushblockOptions.Instance.Enabled && originalFrames > 0)
            {
                _recordController.StopPlayback(); // Stop Playback here to prevent locking the recorder playback
                DummyIsStunned = true;
                var random = new System.Random();
                var _originalFrames = originalFrames - 2; // Compensate for delay and prevent nothing happening
                var pushblockFrame = PercentToPushblock == -1 // -1 means random
                    ? random.Next(0, _originalFrames)
                    : (int)(_originalFrames * (PercentToPushblock / 100f));
                _dummyRecorder.inputs = new List<CommandRecordingDriver.InputChange>();
                _dummyRecorder.inputs.Add(
                    new CommandRecordingDriver.InputChange(pushblockFrame == 0 ? 1 : pushblockFrame, (uint)DUMMY_INPUTS._5S));
                _recordController.Reset();
                _recordController.StartPlayback();
            }
        }
    }
}