using System;
using System.Collections.Generic;
using GrimbaHack.Utility;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using UniverseLib.UI.Models;
using Object = UnityEngine.Object;

namespace GrimbaHack.Modules.PlayerInput;

public sealed class PlayerInputController : ModuleBase
{
    private PlayerInputBehaviour Behaviour;
    private static GameObject playerInputButtonGroup;
    private static ButtonRef recordButton;
    private static ButtonRef playbackButton;
    private static ButtonRef exportButton;
    private static PlayerInputBehaviourState _state;

    private PlayerInputController()
    {
    }

    public static PlayerInputController Instance { get; set; }

    static PlayerInputController()
    {
        Instance = new PlayerInputController();
        GameObject go = new GameObject("PlayerInputBehaviour");
        go.hideFlags = HideFlags.HideAndDontSave;
        Object.DontDestroyOnLoad(go);
        Instance.Behaviour = go.AddComponent<PlayerInputBehaviour>();
        Instance.Behaviour.SetEnable(false);
        OnEnterTrainingMatchActionHandler.Instance.AddCallback(() => { Instance.Behaviour.SetEnable(true); });
        OnSimulationInitializeActionHandler.Instance.AddCallback(() => { Instance.Behaviour.Setup(); });
        OnUIComboCounterOnBreakComboCallbackHandler.Instance.AddCallback((playerId, comboCount) =>
        {
            if (Instance.Behaviour.enabled)
            {
                Instance.Reset();
            }
        });
    }


    public void Idle()
    {
        _state = PlayerInputBehaviourState.Idle;
        Instance.Behaviour.SetEnable(false);
    }

    public void Record()
    {
        _state = PlayerInputBehaviourState.Recording;
    }

    public void Reset()
    {
        _state = PlayerInputBehaviourState.Idle;
        Instance.Behaviour.SetEnable(false);
        Instance.Behaviour.PlaybackCount = 0;
    }
    public static void PreRecord()
    {
        _state = PlayerInputBehaviourState.PreRecord;
        Instance.Behaviour.Inputs = new();
        Instance.Behaviour.SetEnable(true);
    }

    public static List<uint> GetInputs()
    {
        return Instance.Behaviour.Inputs;
    }

    public static void Playback()
    {
        Instance.Behaviour.SetEnable(true);
        _state = PlayerInputBehaviourState.Playback;
        Instance.Behaviour.PlaybackCount = 0;
    }

    public PlayerInputBehaviourState GetState()
    {
        return _state;
    }
}
