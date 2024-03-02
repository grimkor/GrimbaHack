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

    public void SetState(PlayerInputBehaviourState state)
    {
        switch (state)
        {
            case PlayerInputBehaviourState.Idle:
                SetIdle();
                break;
            case PlayerInputBehaviourState.PreRecord:
                Record();
                break;
            case PlayerInputBehaviourState.Recording:
                _state = PlayerInputBehaviourState.Recording;
                break;
            case PlayerInputBehaviourState.Playback:
                Playback();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void SetIdle()
    {
        Instance.Behaviour.SetEnable(false);
        _state = PlayerInputBehaviourState.Idle;
    }

    public void Reset()
    {
        Instance.Behaviour.SetEnable(false);
        Instance.Behaviour.PlaybackCount = 0;
        _state = PlayerInputBehaviourState.Idle;
    }
    public static void Record()
    {
        _state = PlayerInputBehaviourState.PreRecord;
        Instance.Behaviour.Setup();
        Instance.Behaviour.SetEnable(true);
        Instance.Behaviour.Inputs = new();
    }

    public static List<uint> GetInputs()
    {
        return Instance.Behaviour.Inputs;
    }

    public static void Playback()
    {
        Instance.Behaviour.Setup();
        Instance.Behaviour.SetEnable(true);
        _state = PlayerInputBehaviourState.Playback;
        Instance.Behaviour.PlaybackCount = 0;
    }

    public PlayerInputBehaviourState GetState()
    {
        return _state;
    }
}
