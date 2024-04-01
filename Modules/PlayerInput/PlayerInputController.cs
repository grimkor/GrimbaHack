using System.Collections.Generic;
using GrimbaHack.Utility;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GrimbaHack.Modules.PlayerInput;

public sealed class PlayerInputController : ModuleBase
{
    private PlayerInputBehaviour Behaviour;
    private GameObject playerInputButtonGroup;
    private PlayerInputBehaviourState _state;
    private Character _playerCharacter;
    private Character _dummyCharacter;
    public Vector3F playerPosition;
    public Vector3F dummyPosition;

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
        OnEnterTrainingMatchActionHandler.Instance.AddPostfix(() => { Instance.Behaviour.SetEnable(true); });
        OnSimulationInitializeActionHandler.Instance.AddPostfix(() =>
        {
            Instance.Reset();
            Instance.Behaviour.Setup();
        });
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
    }

    public static void PreRecord()
    {
        Instance._state = PlayerInputBehaviourState.PreRecord;
        Instance.Behaviour.Inputs = new();
        Instance.Behaviour.SetEnable(true);
    }

    public static List<uint> GetInputs()
    {
        return Instance.Behaviour.Inputs;
    }

    public static Vector3F GetPlayerStartPosition()
    {
        return Instance.playerPosition;
    }

    public static Vector3F GetDummyStartPosition()
    {
        return Instance.dummyPosition;
    }

    public static void Playback()
    {
        Instance.Behaviour.SetEnable(true);
        Instance._state = PlayerInputBehaviourState.Playback;
    }

    public PlayerInputBehaviourState GetState()
    {
        return _state;
    }

    public static void SetInputs(List<uint> inputs)
    {
        Instance.Behaviour.Inputs = inputs;
    }

    public static void SetCharacterPositions(Vector3F player, Vector3F dummy)
    {
        Instance.playerPosition = player;
        Instance.dummyPosition = dummy;
    }

    public static void RecordCharacterPositions()
    {
        Instance.playerPosition = GetPlayerCharacter().GetPosition();
        Instance.dummyPosition = GetDummyCharacter().GetPosition();
    }

    public static void SetCharacters(Character player, Character dummy)
    {
        Instance._playerCharacter = player;
        Instance._dummyCharacter = dummy;
    }

    public static Character GetPlayerCharacter()
    {
        return Instance._playerCharacter;
    }

    public static Character GetDummyCharacter()
    {
        return Instance._dummyCharacter;
    }

    public static void LoadSavedCharacterPositions()
    {
        GetPlayerCharacter().SetPosition(Instance.playerPosition);
        GetDummyCharacter().SetPosition(Instance.dummyPosition);
    }
}
