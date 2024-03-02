using System.Collections.Generic;
using GrimbaHack.Utility;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using UniverseLib.UI.Models;

namespace GrimbaHack.Modules.PlayerInput;

public sealed class PlayerInputController : ModuleBase
{
    private PlayerInputBehaviour Behaviour;
    private static GameObject playerInputButtonGroup;
    private static ButtonRef recordButton;
    private static ButtonRef playbackButton;
    private static ButtonRef exportButton;

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
                Instance.Behaviour.Reset();
            }
        });
    }

    public static void Record()
    {
        Instance.Behaviour.Record();
    }

    public static List<uint> GetInputs()
    {
        return Instance.Behaviour.inputs;
    }

    public static void Playback()
    {
       Instance.Behaviour.Playback();
    }
}
