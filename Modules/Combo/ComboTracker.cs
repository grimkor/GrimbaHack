using System;
using System.Collections.Generic;
using GrimbaHack.Utility;
using UnityEngine;
using UniverseLib.UI;
using Object = UnityEngine.Object;

namespace GrimbaHack.Modules.Combo;

public sealed class ComboTracker : ModuleBase
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
            Status.Enable = value;
            NextStep.Enable = value;
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

    private static List<string> _comboTracker = new List<string>();
    private static List<string> _comboRecorded = new List<string>();
    private static int _stepInCombo = 0;
    private static Character _playerCharacter;
    private static Character _dummyCharacter;
    private static LabelValueOverlayText Status = new("Status", "Ready", new Vector3(240, 240, 1));
    private static LabelValueOverlayText NextStep = new("Next Step", "-", new Vector3(240, 210, 1));
    private static ComboTrackerState _state;

    static ComboTracker()
    {
        Status.Enable = false;
        NextStep.Enable = false;
        Instance = new ComboTracker();
        OnSimulationInitializeActionHandler.Instance.AddCallback(() => { Instance.Setup(); });
        OnArmorTakeDamageCallbackHandler.Instance.AddCallback(info =>
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
                            NextStep.Value = _comboRecorded[_stepInCombo]
                                .Replace(info.attacker.GetCharacterName() + "_combat_", "");
                        }
                        else
                        {
                            NextStep.Value = "COMPLETED!";
                        }
                    }
                }
            }
        });

        OnUIComboCounterOnBreakComboCallbackHandler.Instance.AddCallback((playerId, comboCounter) =>
        {
            if (_state == ComboTrackerState.Recording)
            {
                Instance.SetState(ComboTrackerState.Comparing);
                return;
            }
            if (_state == ComboTrackerState.Comparing && _stepInCombo == _comboRecorded.Count)
            {
                Instance.SetState(ComboTrackerState.Idle);
            }
        });
    }

    private void Setup()
    {
        var characters = Object.FindObjectsOfType<Character>();
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
        Plugin.Log.LogInfo($"SetState: {state}");
        _state = state;
        switch (state)
        {
            case ComboTrackerState.Recording:
                Instance.SetRecording();
                break;
            case ComboTrackerState.Comparing:
                Instance.SetCompare();
                break;
            case ComboTrackerState.Idle:
                Status.Value = "Idle";
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
        Status.Value = "Recording";
    }

    private void SetCompare()
    {
        if (_comboTracker.Count > 0)
        {
            _comboRecorded = _comboTracker;
        }

        if (_comboRecorded.Count > 0)
        {
            Status.Value = "Comparing";
            _stepInCombo = 0;
            NextStep.Value = _comboRecorded[0].Replace(_playerCharacter.GetCharacterName() + "_combat_", "");
            _comboTracker = new();
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

    public static void CreateUIControls(GameObject contentRoot)
    {
        var toggle = UIFactory.CreateToggle(contentRoot, "ComboTrackerToggle", out var comboTrackerToggle,
            out var comboTrackerToggleLabel);
        comboTrackerToggle.isOn = false;
        comboTrackerToggle.onValueChanged.AddListener(new Action<bool>((value) => { Instance.Enabled = value; }));
        comboTrackerToggleLabel.text = "Track Combos";
        UIFactory.SetLayoutElement(comboTrackerToggle.gameObject, minHeight: 25, minWidth: 50);
    }
}