using System;
using epoch.db;
using GrimbaHack.Data;
using GrimbaHack.Utility;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using nway.gameplay.ai;
using nway.gameplay.match;
using nway.gameplay.simulation;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;

namespace GrimbaHack.Modules;

public sealed class DummyExPunish : ModuleBase
{
    private DummyExPunish()
    {
    }

    public static DummyExPunish Instance { get; private set; }
    public DummyExPunishBehaviour Behaviour { get; set; }

    static DummyExPunish()
    {
        ClassInjector.RegisterTypeInIl2Cpp<DummyExPunishBehaviour>();
        Instance = new DummyExPunish();
        var go = new GameObject("DummyExPunishBehaviour");
        go.hideFlags = HideFlags.HideAndDontSave;
        GameObject.DontDestroyOnLoad(go);
        Instance.Behaviour = go.AddComponent<DummyExPunishBehaviour>();
        Instance.Enabled = false;
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
        var dummyExPunishGroup = UIFactory.CreateUIObject("DummyExPunishGroup", contentRoot);
        UIFactory.SetLayoutElement(dummyExPunishGroup);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(dummyExPunishGroup, false, false, true, true, padLeft: 25,
            spacing: 10, childAlignment: TextAnchor.MiddleLeft);
        UIFactory.CreateToggle(dummyExPunishGroup, "DummyExPunishToggle", out var dummyExPunishToggle,
            out var dummyExPunishToggleLabel);
        dummyExPunishToggle.isOn = false;
        dummyExPunishToggleLabel.text = "Set Dummy to EX when Hit/Block stun expires";
        dummyExPunishToggle.onValueChanged.AddListener(new Action<bool>((value) =>
        {
            Instance.Enabled = value;
        }));

        UIFactory.SetLayoutElement(dummyExPunishToggle.gameObject, minHeight: 25, minWidth: 50);
    }
}

public class DummyExPunishBehaviour : MonoBehaviour
{
    private static Character DummyCharacter;
    private static CommandRecordingDriver RecordController;
    private static CommandRecordingDriver.RecordingState DummyRecorder;
    private static bool DummyIsStunned = false;
    private static bool _ExTriggered = false;

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
                    DummyCharacter = character;
                }
            }
        }

        if (sceneStartup && DummyCharacter)
        {
            RecordController = sceneStartup.GamePlay?.recorder;
            DummyRecorder = RecordController?.dummyRecorder;
        }
    }

    [HarmonyPatch(typeof(SimulationManager), nameof(SimulationManager.Initialize))]
    public static class SimulationManagerInitializePatch
    {
        public static void Postfix()
        {
            if (DummyExPunish.Instance.Enabled)
            {
                var sceneStartup = FindObjectOfType<SceneStartup>();

                var characters = FindObjectsOfType<Character>();

                foreach (var character in characters)
                {
                    if (character.IsActiveCharacter)
                    {
                        if (character.team != 0)
                        {
                            DummyCharacter = character;
                        }
                    }
                }

                if (sceneStartup && DummyCharacter)
                {
                    RecordController = sceneStartup.GamePlay?.recorder;
                    DummyRecorder = RecordController?.dummyRecorder;
                }
            }
        }
    }

    private void Update()
    {
        if (!DummyCharacter || DummyRecorder == null || RecordController == null)
            return;
        if (_ExTriggered && DummyRecorder is { isPlaying: true } &&
            RecordController.GetCurrentFrame() == DummyRecorder.LatestFrame())
        {
            RecordController.StopPlayback();
            RecordController.Reset();
            _ExTriggered = false;
        }

        if (DummyIsStunned && !DummyCharacter.InBlockStun && !DummyCharacter.InHitStun)
        {
            RecordController.StopPlayback();
            DummyRecorder.Rewind();
            DummyRecorder.PrepareRecording();
            DummyRecorder.RecordInput((uint)DUMMY_INPUTS.EX); //EX
            DummyRecorder.FinishRecording();
            RecordController.StartPlayback();

            DummyIsStunned = false;
            _ExTriggered = true;
        }

        if (!DummyIsStunned && (DummyCharacter.InBlockStun || DummyCharacter.InHitStun))
        {
            DummyIsStunned = true;
        }
    }
}