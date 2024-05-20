using System;
using System.Collections.Generic;
using GrimbaHack.Utility;

namespace GrimbaHack.Modules.ComboTrial;

public class ComboTrialTracker
{
    public static ComboTrialTracker Instance = new();

    private ComboTrialTracker()
    {
    }

    static ComboTrialTracker()
    {
        OnArmorTakeDamageCallbackHandler.Instance.AddPostfix(Instance.OnTakeDamageHandler);
        OnUIComboCounterOnBreakComboCallbackHandler.Instance.AddCallback(Instance.OnComboBreakHandler);
    }

    private List<string> _combo = new();
    private int _stepInCombo;
    private bool _enabled;
    private string _playerName;
    private Action _onCompleteAction;
    private Action _onNextStepHandler;
    private Action _onFailAction;

    public void Init(List<string> combo, string playerName)
    {
        _combo = combo;
        _playerName = playerName;
    }

    public void SetEnable(bool enabled)
    {
        _enabled = enabled;
    }


    public void Reset()
    {
        _stepInCombo = 0;
    }

    private void AddOnNextStepAction(Action onNextStepAction)
    {
        _onNextStepHandler = onNextStepAction;
    }

    private void OnNextStepHandler()
    {
        if (_onNextStepHandler == null) return;
        _onNextStepHandler();
    }

    public void AddOnComplete(Action onCompleteAction)
    {
        _onCompleteAction = onCompleteAction;
    }

    public void AddOnFail(Action onFailAction)
    {
        _onFailAction = onFailAction;
    }

    private void OnFailAction()
    {
        if (_onFailAction != null)
        {
            _onFailAction();
        }
    }

    private void OnComplete()
    {
        if (_onCompleteAction == null) return;
        _onCompleteAction();
    }

    private void OnTakeDamageHandler(DamageInfo damageInfo)
    {
        if (!_enabled) return;
        if (damageInfo.attacker.name == _playerName)
        {
            if (_stepInCombo < _combo.Count)
            {
                if (_combo[_stepInCombo] == damageInfo.attackName)
                {
                    _stepInCombo++;
                    if (_stepInCombo < _combo.Count)
                    {
                        Instance.OnNextStepHandler();
                    }
                    else
                    {
                        Instance.OnComplete();
                    }
                }
            }
        }
    }

    private void OnComboBreakHandler(int playerId, int comboCounter)
    {
        if (!_enabled) return;
        if (_stepInCombo != _combo.Count - 1)
        {
            OnFailAction();
        }
    }
}
