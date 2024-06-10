using System;
using System.Collections.Generic;
using GrimbaHack.Utility;
using Object = UnityEngine.Object;

namespace GrimbaHack.Modules.Combo;

public sealed class ComboTracker
{
    private ComboTracker()
    {
    }

    public static ComboTracker Instance { get; private set; }
    private bool _enabled;

    public bool Enabled
    {
        get => Instance._enabled;
        set
        {
            Instance._enabled = value;
            if (value)
            {
                Setup();
            }
            else
            {
                Instance.SetState(ComboTrackerState.Idle);
            }
        }
    }

    private static List<string> _comboTracker = new();
    private static List<string> _comboRecorded = new();
    private static int _stepInCombo;
    private static Character _playerCharacter;
    private static Character _dummyCharacter;
    private static ComboTrackerState _state;
    private List<Action> _onCompleteCallbacks = new();
    private List<Action<string, DamageInfo>> _onNextStepCallbacks = new();

    static ComboTracker()
    {
        Instance = new ComboTracker();
        // OnSimulationInitializeActionHandler.Instance.AddPostfix(() => { Instance.Setup(); });
        OnArmorTakeDamageCallbackHandler.Instance.AddPostfix(info =>
        {
            if (!Instance.Enabled || _playerCharacter == null || _dummyCharacter == null ||
                _state == ComboTrackerState.Idle)
            {
                return;
            }

            if (info.attacker.name == _playerCharacter?.name && info.victim.name == _dummyCharacter?.name)
            {
                if (_state == ComboTrackerState.Recording)
                {
                    _comboTracker.Add(info.attackName);
                    return;
                }

                if (_state == ComboTrackerState.Comparing && _stepInCombo < _comboRecorded.Count)
                {
                    if (_comboRecorded[_stepInCombo] == info.attackName)
                    {
                        _stepInCombo++;
                        if (_stepInCombo < _comboRecorded.Count)
                        {
                            Instance.OnNextStepActionHandler(_comboRecorded[_stepInCombo], info);
                        }
                        else
                        {
                            Instance.OnCompleteActionHandler();
                        }
                    }
                }
            }
        });

        OnUIComboCounterOnBreakComboCallbackHandler.Instance.AddCallback((playerId, comboCounter) =>
        {
            if (_state == ComboTrackerState.Recording)
            {
                Instance.SaveCombo();
                Instance.SetState(ComboTrackerState.Comparing);
                return;
            }

            if (_state == ComboTrackerState.Comparing && _stepInCombo == _comboRecorded.Count)
            {
                Instance.SetState(ComboTrackerState.Idle);
            }
        });
    }

    public void Setup()
    {
        if (!Instance.Enabled)
        {
            return;
        }

        var characters = SceneStartup.instance.GamePlay._playerList;
        foreach (var character in characters)
        {
            if (character.IsActiveCharacter)
            {
                if (character.team == 0)
                {
                    _playerCharacter = character;
                }
                else
                {
                    _dummyCharacter = character;
                }
            }
        }

        if (_comboRecorded.Count > 0)
        {
            SetState(ComboTrackerState.Comparing);
        }
        else
        {
            SetState(ComboTrackerState.Idle);
        }
    }

    public void SetState(ComboTrackerState state)
    {
        switch (state)
        {
            case ComboTrackerState.Recording:
                Instance.SetRecording();
                break;
            case ComboTrackerState.Comparing:
                Instance.SetCompare();
                break;
            case ComboTrackerState.Idle:
                UIComboTracker.Instance.SetStatusText("Idle");
                _state = ComboTrackerState.Idle;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void SetRecording()
    {
        _comboTracker = new();
        _comboRecorded = new();
        _state = ComboTrackerState.Recording;
    }

    private void SetCompare()
    {
        if (_comboRecorded.Count > 0)
        {
            _stepInCombo = 0;
            _state = ComboTrackerState.Comparing;
            UIComboTracker.Instance.SetStatusText("Trial mode");
            UIComboTracker.Instance.SetNextStepText(
                UIComboTracker.StripCharacterName(Instance.GetCombo()[0], _playerCharacter));
        }
        else
        {
            SetState(ComboTrackerState.Idle);
        }
    }

    public List<string> GetCombo()
    {
        return _comboRecorded;
    }

    public static Character GetPlayerCharacter()
    {
        return _playerCharacter;
    }

    private void SaveCombo()
    {
        if (_comboTracker.Count > 0)
        {
            _comboRecorded = _comboTracker;
            _comboTracker = new();
        }
    }

    public void OnNextStep(Action<string, DamageInfo> callback)
    {
        Instance._onNextStepCallbacks.Add(callback);
    }

    private void OnNextStepActionHandler(string nextStep, DamageInfo info)
    {
        foreach (Action<string, DamageInfo> callback in Instance._onNextStepCallbacks)
        {
            callback(nextStep, info);
        }
    }

    public void OnComplete(Action callback)
    {
        Instance._onCompleteCallbacks.Add(callback);
    }

    private void OnCompleteActionHandler()
    {
        foreach (var callback in Instance._onCompleteCallbacks)
        {
            callback();
        }
    }

    public void SetCombo(List<string> combo)
    {
        _comboRecorded = combo;
    }

    public ComboTrackerState GetState()
    {
        return _state;
    }
}
