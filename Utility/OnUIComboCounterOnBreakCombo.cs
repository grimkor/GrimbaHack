using System;
using System.Collections.Generic;
using HarmonyLib;
using nway.ui;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(UIComboCounter), nameof(UIComboCounter.OnBreakCombo))]
public class OnUIComboCounterOnBreakComboCallbackHandler
{
    private OnUIComboCounterOnBreakComboCallbackHandler()
    {
    }

    static OnUIComboCounterOnBreakComboCallbackHandler()
    {
        Instance = new OnUIComboCounterOnBreakComboCallbackHandler();
    }
    public static OnUIComboCounterOnBreakComboCallbackHandler Instance { get; set; }
    private List<Action<int, int>> callbacks = new();

    public void AddCallback(Action<int, int> callback)
    {
        Instance.callbacks.Add(callback);
    }
    
    public static void Prefix(int playerID, int comboCounter)
    {
        if (comboCounter == 0)
        {
            return;
        }
        foreach (Action<int, int> callback in Instance.callbacks)
        {
            callback(playerID, comboCounter);
        }
    }
}