using epoch.db;
using GrimbaHack.Data;
using GrimbaHack.Utility;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using nway.gameplay.ai;
using nway.gameplay.match;
using nway.gameplay.simulation;
using UnityEngine;

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
        Object.DontDestroyOnLoad(go);
        Instance.Behaviour = go.AddComponent<DummyExPunishBehaviour>();
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
}

public class DummyExPunishBehaviour : MonoBehaviour
{
    private static Character DummyCharacter;
    private static CommandRecordingDriver RecordController;
    private static CommandRecordingDriver.RecordingState DummyRecorder;
    private static bool DummyIsStunned;
    private static bool _ExTriggered;
    private Il2CppArrayBase<Character> _characters;

    public DummyExPunishBehaviour()
    {
        OnSimulationInitializeActionHandler.Instance.AddPostfix(() =>
        {
            if (DummyExPunish.Instance.Enabled)
            {
                var sceneStartup = SceneStartup.Get;

                var characters = sceneStartup.GamePlay._playerList;

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
        });
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
